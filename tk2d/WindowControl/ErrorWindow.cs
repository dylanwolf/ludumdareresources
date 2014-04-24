using UnityEngine;
using System.Collections;

[RequireComponent(typeof(tk2dSlicedSprite))]
public class ErrorWindow : MonoBehaviour {

	public static ErrorWindow Current;

	public tk2dTextMesh textMesh;
	private tk2dSlicedSprite window;

	private const float MaxTimer = 5;
	private const float FadeTimer = 0.5f;
	private float Timer = 0;

	enum WindowState
	{
		Hidden,
		FadeIn,
		Show,
		FadeOut
	}

	private WindowState state;
	private Color tmpColor;

	void Start () {
		window = GetComponent<tk2dSlicedSprite>();
		Current = this;
		state = WindowState.Hidden;
		Hide_Internal();
	}

	void SetTransparency(float a)
	{
		tmpColor = window.color;
		tmpColor.a = a;
		window.color = tmpColor;

		tmpColor = textMesh.color;
		tmpColor.a = a;
		textMesh.color = tmpColor;
	}

	void Update() {
		if (Timer > 0)
		{
			Timer -= Time.deltaTime;
			if (state == WindowState.FadeIn)
			{
				SetTransparency((FadeTimer - Timer)/ FadeTimer);
				if (Timer < 0)
				{
					SetTransparency(1);
					state = WindowState.Show;
					Timer = MaxTimer;
				}
			}
			else if (state == WindowState.Show)
			{
				if (Timer < 0)
				{
					state = WindowState.FadeOut;
					Timer = FadeTimer;
				}
			}
			else if (state == WindowState.FadeOut)
			{
				SetTransparency(Timer / FadeTimer);
				if (Timer < 0)
				{
					Hide_Internal();
				}
			}
		}
	}

	public static void Show(string message)
	{
		if (Current != null)
		{
			Current.Show_Internal(message);
		}
	}

	protected void Show_Internal(string message)
	{
		SetTransparency(0);
		textMesh.text = message;
		Timer = FadeTimer;
		state = WindowState.FadeIn;
		textMesh.renderer.enabled = true;
		window.renderer.enabled = true;
	}

	public static void Hide()
	{
		if (Current != null)
		{
			Current.Hide_Internal();
		}
	}

	protected void Hide_Internal()
	{
		state = WindowState.Hidden;
		textMesh.renderer.enabled = false;
		window.renderer.enabled = false;
	}
}
