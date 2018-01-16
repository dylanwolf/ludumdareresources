using UnityEngine;
using System.Collections;

public class FakeParallax : MonoBehaviour {

	public float CameraYMin = 0;
	public float CameraYMax = 0;

	public float BackdropYMin = 0;
	public float BackdropYMax = 0;

	Transform _t;
	Transform _ct;

	void Awake()
	{
		_t = transform;
		_ct = Camera.main.transform;
	}

	float pct;
	Vector3 tmpPos;
	void Update()
	{
		pct = (_ct.position.y - CameraYMin) / (CameraYMax - CameraYMin);
		pct = (pct < 0) ? 0 : ((pct > 1) ? 1 : pct);

		tmpPos = _t.localPosition;
		tmpPos.y = ((BackdropYMax - BackdropYMin) * pct) + BackdropYMin;
		_t.localPosition = tmpPos;
	}
}
