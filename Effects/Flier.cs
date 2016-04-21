using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("LudumDareResources/Effects/Flier")]
[RequireComponent(typeof(SpriteRenderer))]
public class Flier : MonoBehaviour {

	void Awake()
	{
		_r = GetComponent<SpriteRenderer>();
		_t = transform;
	}

	public float Speed = 0.05f;
	float time = 0;

	SpriteRenderer _r;
	Transform _t;
	Vector3 tmpPos;
	Vector3 originalPos;
	Vector3 targetPos;
	float distance = 0;
	GameObject trgt;
	FlierMessage[] payld;

	const string FLY_COROUTINE = "FlyCoroutine";
	IEnumerator FlyCoroutine()
	{
		time = 0;
		while (time < (Speed * distance))
		{
			time += (Time.deltaTime / Time.timeScale);
			tmpPos = Vector3.Lerp(
				originalPos,
				targetPos,
				time / (Speed * distance));
			tmpPos.z = _t.position.z;
			_t.position = tmpPos;
			yield return null;
		}

		Deliver();
		Despawn();
	}

	void Deliver()
	{
		if (trgt != null && payld != null)
		{
			for (int i = 0; i < payld.Length; i++)
			{
				trgt.SendMessage(payld[i].Message, payld[i].Parameter);
			}
		}
	}

	public void Fly(Vector3 original, Vector3 target)
	{
		StopAllCoroutines();

		originalPos = original;
		originalPos.z = _t.position.z;

		targetPos = target;
		targetPos.z = _t.position.z;

		distance = Vector3.Distance(originalPos, targetPos);

		StartCoroutine(FLY_COROUTINE);
	}

	public void Despawn()
	{
		DestroyObject(gameObject);
	}

	[Serializable]
	public class FlierMessage
	{
		public string Message;
		public object Parameter;
	}

	public void Initialize(Sprite sprite, Vector3 originalPos, Vector3 targetPos, GameObject targetObject, FlierMessage[] payload)
	{
		_t.position = originalPos;
		_r.sprite = sprite;
		trgt = targetObject;
		payld = payload;
		Fly(originalPos, targetPos);
	}
}