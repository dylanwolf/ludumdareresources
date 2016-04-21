using UnityEngine;
using System.Collections;

[AddComponentMenu("LudumDareResources/2D Toolkit/Tiled Sprite Scoller")]
[RequireComponent(typeof(tk2dTiledSprite))]
public class TileScroller : MonoBehaviour {

	private Vector2 current;
	private Vector2 max;
	public Vector2 ScrollSize;
	public Vector2 ScrollSpeed;

	private tk2dTiledSprite sprite;

	void Start()
	{
		sprite = GetComponent<tk2dTiledSprite>();
		max = sprite.dimensions;
	}

	private Vector2 tmpVector2;
	void Update()
	{
		tmpVector2 = sprite.dimensions;

		if (ScrollSize.x > 0)
		{
			current.x += ScrollSpeed.x * Time.deltaTime;
			if (current.x > ScrollSize.x) { current.x %= ScrollSize.x; }
			tmpVector2.x = max.x - current.x;
		}
		if (ScrollSize.y > 0)
		{
			current.y += ScrollSpeed.y * Time.deltaTime;
			if (current.y > ScrollSize.y) { current.y %= ScrollSize.y; }
			tmpVector2.y = max.y - current.y;
		}

		sprite.dimensions = tmpVector2;
	}
}
