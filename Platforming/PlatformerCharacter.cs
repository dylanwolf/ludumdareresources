using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class PlatformerCharacter : MonoBehaviour {

	#region Platforming State
	public enum PlatformingState
	{
		Grounded,
		Jumping,
		Falling
	}

	[System.NonSerialized]
	public PlatformingState Platforming = PlatformingState.Grounded;
	#endregion

	#region Configuration
	[Header("Raycasting")]
	public Transform[] GroundRaycasts;

	[Header("Raycasting")]
	public Transform[] HeadRaycasts;

	[Header("Raycasting")]
	public LayerMask PlatformLayers; 

	[Header("Raycasting")]
	public float RaycastDistance = 0.05f;

	[Header("Movement Speed")]
	public float MaxSpeed = 5.0f;

	[Header("Movement Speed")]
	public float AccelerationRate = 1.0f;

	[Header("Movement Speed")]
	public float DecelerationRate = 2f;

	[Header("Movement Speed")]
	public float ReactionPercentage = 0.3f;

	[Header("Jump Speed")]
	public float MaxJumpTime = 0.6f;

	[Header("Jump Speed")]
	public float MaxJumpHeight = 4.0f;

	[Header("Animation")]
	public string JumpingAnimationParameter;

	[Header("Animation")]
	public string WalkingAnimationParameter;
	#endregion

	#region Components
	Animator anim;
	Rigidbody2D _rb;
	Transform _t;
	#endregion

	#region Properties
	protected float speedPct = 0;
	protected bool isGrounded = false;
	protected bool hitHead = false;
	protected Collider2D[] childColliders;
	protected RaycastHit2D[] raycastHits = new RaycastHit2D[255];
	protected int hitCount = 0;

	protected float inputDirection;
	protected float lastInputDirection;
	protected bool inputJump;

	public struct InputResponse
	{
		public float HorizontalDirection;
		public bool Jump;
	}

	protected float jumpStartY = 0;
	protected float jumpEndY = 0;
	protected float jumpVelocity = 0;
	protected bool isHoldingJump = false;

	protected bool startedJump;
	protected float jumpPosY = 0;
	protected float jumpT = 0;
	protected float jumpTSquared = 0;

	protected bool isAcceleratingFall = false;
	protected float fallVelocity = 0;

	protected int facing = 1;
	protected bool changed = false;
	protected bool foundMatch = false;
	protected Vector2 tmpVelocity;
	protected Vector3 tmpScale;
	protected bool paused = false;

	protected InputResponse? input;
	#endregion

	#region Abstract/Virtual Methods
	protected virtual bool IsGameActive() { return true; }
	protected virtual InputResponse? GetInput() { return null; }
	#endregion

	#region Unity Lifecycle
	protected virtual void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		_t = transform;
	}

	protected virtual void Start()
	{
		childColliders = GetComponentsInChildren<Collider2D>();
	}

	protected void FixedUpdate()
	{
		if (!IsGameActive())
		{
			if (_rb.velocity.x != 0 || _rb.velocity.y != 0)
				_rb.velocity = Vector3.zero;
			if (anim != null)
				anim.enabled = false;
			paused = true;
			return;
		}
		if (paused && anim != null)
			anim.enabled = true;

		tmpVelocity = _rb.velocity;

		// Test for current position
		isGrounded = false;
		changed = false;
		for (int i = 0; i < GroundRaycasts.Length; i++)
		{
			if (TestRaycastHit(GroundRaycasts[i], PlatformLayers, Vector2.down))
			{
				isGrounded = true;
				break;
			}
		}

		if (isGrounded && Platforming != PlatformingState.Grounded)
		{
			Platforming = PlatformingState.Grounded;
			changed = true;
		}
		else if (!isGrounded && Platforming == PlatformingState.Grounded)
		{
			DoFall();
			changed = true;
		}

		if (Platforming == PlatformingState.Jumping)
		{
			hitHead = false;
			for (int i = 0; i < HeadRaycasts.Length; i++)
			{
				if (TestRaycastHit(HeadRaycasts[i], PlatformLayers, Vector2.up))
				{
					hitHead = true;
					break;
				}
			}

			if (hitHead)
				DoFall();
		}

		// Apply movement
		tmpVelocity = _rb.velocity;
		tmpScale = _t.localScale;
		if (inputDirection != 0)
		{
			// Apply walking
			if (startedJump || speedPct < 1 || inputDirection != lastInputDirection)
			{
				speedPct += (Time.fixedDeltaTime * AccelerationRate);
				speedPct = Mathf.Clamp(speedPct, 0, 1);
				facing = (int)Mathf.Sign(inputDirection);
				tmpScale.x = Mathf.Abs(tmpScale.x) * facing;
				changed = true;
			}
			startedJump = false;
		}
		else
		{
			if (speedPct > 0)
			{
				speedPct -= (Time.fixedDeltaTime * DecelerationRate);
				speedPct = Mathf.Clamp(speedPct, 0, 1);
				changed = true;
			}
		}
		tmpVelocity.x = facing * Mathf.Sin(speedPct * 0.5f * Mathf.PI) * MaxSpeed;

		// Apply jumping
		if (inputJump)
			DoJump();

		if (Platforming == PlatformingState.Jumping)
		{
			tmpVelocity.y = jumpVelocity;
			changed = true;
		}

		// Apply falling
		if (Platforming == PlatformingState.Falling)
		{
			tmpVelocity.y = fallVelocity;
			changed = true;
		}

		_t.localScale = tmpScale;
		if (changed)
			_rb.velocity = tmpVelocity;

		if (anim != null)
		{
			anim.SetBool(JumpingAnimationParameter, Platforming == PlatformingState.Jumping || Platforming == PlatformingState.Falling);
			anim.SetBool(WalkingAnimationParameter, Platforming == PlatformingState.Grounded && _rb.velocity.x != 0);
		}
	}

	protected void Update()
	{
		lastInputDirection = inputDirection;

		input = GetInput();
		if (input.HasValue)
		{
			inputDirection = input.Value.HorizontalDirection;
			isHoldingJump = input.Value.Jump;

			if (inputDirection != 0)
			{
				if (speedPct <= 0)
					speedPct = 0;
				else if (inputDirection != lastInputDirection)
					speedPct *= 0.5f;
			}

			if (isHoldingJump && !wasHoldingJump)
			{
				inputJump = true;
			}
			wasHoldingJump = isHoldingJump;
		}
	}
	#endregion

	#region Coroutines
	const string FALL_COROUTINE = "FallingCoroutine";
	const string JUMP_COROUTINE = "JumpCoroutine";

	IEnumerator JumpCoroutine()
	{
		Platforming = PlatformingState.Jumping;
		jumpVelocity = 0;

		jumpStartY = _t.position.y;
		jumpEndY = jumpStartY + MaxJumpHeight;

		jumpTSquared = Mathf.Pow(MaxJumpTime, 2);

		while (_t.position.y < jumpEndY && Platforming == PlatformingState.Jumping && isHoldingJump)
		{
			// Handle pause
			if (!IsGameActive())
				yield return 0;

			// Adjust velocity
			jumpPosY = _t.position.y - jumpStartY;
			jumpT = Mathf.Sqrt(jumpTSquared - ((jumpPosY * jumpTSquared) / MaxJumpHeight));
			jumpVelocity = (2 * jumpT * MaxJumpHeight) / jumpTSquared;

			yield return 0;
		}

		if (Platforming != PlatformingState.Grounded)
			DoFall();
	}

	IEnumerator FallingCoroutine()
	{
		if (Platforming != PlatformingState.Falling)
		{
			isAcceleratingFall = true;
			Platforming = PlatformingState.Falling;
			fallVelocity = 0;
			jumpTSquared = Mathf.Pow(MaxJumpTime, 2);

			for (float t = 0; t < MaxJumpTime; t += Time.deltaTime)
			{
				if (!IsGameActive())
				{
					t -= Time.deltaTime;
					yield return 0;
				}

				if (Platforming != PlatformingState.Falling)
					break;

				t += Time.deltaTime;
				fallVelocity = -(2 * t * MaxJumpHeight) / jumpTSquared;
				yield return 0;
			}

			isAcceleratingFall = false;
		}
	}
	#endregion

	#region Trigger State Changes
	void DoJump()
	{
		if (Platforming == PlatformingState.Grounded)
		{
			StartCoroutine(JUMP_COROUTINE);
			startedJump = true;
		}
		inputJump = false;
	}

	void DoFall()
	{
		Debug.Log("Starting fall");
		if (!isAcceleratingFall)
			StartCoroutine(FALL_COROUTINE);
	}
	#endregion

	#region Raycasting
	RaycastHit2D?[] raycastHitCopies = new RaycastHit2D?[255];
	Collider2D tmpCollider;
	bool TestRaycastHit(Transform t, int layerMask, Vector2 direction)
	{
		hitCount = 0;
		hitCount = Physics2D.RaycastNonAlloc(t.position, direction, raycastHits, RaycastDistance, PlatformLayers);
		raycastHits.CopyTo(raycastHitCopies, 0);

		for (int i = 0; i < hitCount; i++)
		{
			if (!raycastHitCopies[i].HasValue || raycastHitCopies[i].Value.collider.isTrigger)
				break;

			foundMatch = false;
			for (int j = 0; j < childColliders.Length; j++)
			{
				tmpCollider = childColliders[j];
				if (tmpCollider != null && tmpCollider == raycastHitCopies[i].Value.collider)
				{
					foundMatch = true;
					break;
				}
			}
			raycastHitCopies[i] = null;

			if (!foundMatch)
			{
				return true;
			}
		}
		return false;
	}

	bool wasHoldingJump = false;
	#endregion
}
