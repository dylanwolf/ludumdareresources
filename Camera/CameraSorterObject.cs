using System;
using UnityEngine;
using System.Collections;

[AddComponentMenu("LudumDareResources/Camera/2.5D Camera Sprite Sorter")]
[RequireComponent(typeof(SpriteRenderer))]
public class CameraSorterObject : MonoBehaviour
{
	// Defines how much the Y position within the camera bounds should be scaled up to be the Sort Order
	const float GRANULARITY = 100;

	// Defines how far a sprite must be outside camera bounds to be ignored
	const float MARGIN_MULTIPLIER = 2;

	public Camera SortingCamera;

	SpriteRenderer _r;
	Transform _t;
	Transform _ct;

	void Start()
	{
		_r = GetComponent<SpriteRenderer>();
		_t = transform;

		if (SortingCamera == null)
			ChangeCamera(Camera.main);
		else
			ChangeCamera(SortingCamera);
	}

	public void ChangeCamera(Camera camera)
	{
		SortingCamera = camera;
		_ct = camera.transform;
	}


	float minY;
	float maxY;
	float posY;
    void Update()
	{
		minY = _ct.position.y - (SortingCamera.orthographicSize * MARGIN_MULTIPLIER);
		maxY = _ct.position.y + (SortingCamera.orthographicSize * MARGIN_MULTIPLIER);

		if (_t.position.y < minY || _t.position.y > maxY)
			return;

		_r.sortingOrder = (int)((SortingCamera.orthographicSize - _t.position.y) * GRANULARITY);
	}
}
