using UnityEngine;
using System.Collections;

// This class is intended to be used as a basis for modifications.
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Match3Block : MonoBehaviour {

	protected Match3Board _board;
	protected SpriteRenderer _sr;

	void Awake()
	{
		_sr = GetComponent<SpriteRenderer>();
	}

	public void SetBlockInfo(Match3Board board, int blockType, int blockX, int blockY, Sprite sprite)
	{
		_board = board;
		BlockX = blockX;
		BlockY = blockY;
		_sr.sprite = sprite;
		BlockType = blockType;
		_sr.enabled = true;
	}

	public void Despawn()
	{
		_sr.enabled = false;
	}

	public int BlockX;
	public int BlockY;
	public int BlockType;
}
