using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This class is intended to be used as a basis for modifications.
//
// Blocks are positioned centered on this object's transform.
// BlockTypes start at 1.

public abstract class Match3Board : MonoBehaviour {

	#region Properties
	public static Match3Board Current;

	public string BlockObjectPoolName;
	public Sprite[] BlockSprites;
	public Vector2 TileSize;
	public int Rows;
	public int Cols;

	Transform _t;

	[System.NonSerialized]
	public Match3Block[,] Blocks;
	#endregion

	#region Unity Lifecycle
	void Awake()
	{
		Current = this;
		Blocks = new Match3Block[Rows, Cols];
		_t = transform;
	}

	void Start()
	{
		GenerateBoard();
	}
	#endregion

	#region Methods for override
	protected virtual void GenerateBoard() { }
	protected virtual void ScoreBlock(Match3Block block) { }
	#endregion

	public void Reset()
	{
		ClearBoard();
		GenerateBoard();
	}

	Match3Block block;
	public Match3Block InstantiateBlock(int blockType, int row, int col)
	{
		block = ObjectPools.Spawn<Match3Block>(
			BlockObjectPoolName,
			GetWorldPosition(col, row),
			_t);

		block.SetBlockInfo(this, blockType, col, row, BlockSprites[blockType - 1]);
		Blocks[row, col] = block;
		return block;
	}

	public void ClearBoard()
	{
		for (int r = 0; r < Rows; r++)
		{
			for (int c = 0; c < Cols; c++)
			{
				ClearBlock(r, c);
			}
		}
	}

	public void ClearBlock(Match3Block block)
	{
		ClearBlock(block.BlockY, block.BlockX);
	}

	public void ClearBlock(int row, int col)
	{
		if (Blocks[row, col] == null)
			return;

		Blocks[row, col].Despawn();
		ObjectPools.Despawn(Blocks[row, col].gameObject);
		Blocks[row, col] = null;
	}

	public Vector3 GetLocalPosition(int x, int y)
	{
		return new Vector3(
				(x - ((Cols - 1) / 2.0f)) * TileSize.x,
				(y - ((Rows - 1) / 2.0f)) * TileSize.y,
				0
			);
	}

	public Vector3 GetWorldPosition(int x, int y)
	{
		return GetLocalPosition(x, y) + _t.position;
	}

	Match3Block[,] boardCopy;
	public void CopyBoard()
	{
		if (boardCopy == null)
			boardCopy = new Match3Block[Rows, Cols];

		for (int r = 0; r < Rows; r++)
		{
			for (int c = 0; c < Cols; c++)
			{
				boardCopy[r, c] = Blocks[r, c];
			}
		}
	}

	Match3Block currentlyTesting;
	public void TestBlock(int x, int y)
	{
		// Already tested
		if (boardCopy[y, x] == null)
		{
			return;
		}

		// Start testing
		else if (currentlyTesting == null)
		{
			currentlyTesting = boardCopy[y, x];
			boardCopy[y, x] = null;
			collector.Add(currentlyTesting);
		}

		// Not the same shape... leave and come back to this later
		else if (currentlyTesting.BlockType != boardCopy[y, x].BlockType)
		{
			return;
		}

		// Matching shape
		else
		{
			collector.Add(boardCopy[y, x]);
			boardCopy[y, x] = null;
		}

		// Test around the block
		if (x > 0)
			TestBlock(x - 1, y);
		if (y > 0)
			TestBlock(x, y - 1);
		if (x < Cols - 1)
			TestBlock(x + 1, y);
		if (y < Rows - 1)
			TestBlock(x, y + 1);
	}

	List<Match3Block> collector = new List<Match3Block>();
	public void TestForClear()
	{
		CopyBoard();
		collector.Clear();

		// Test all blocks
		for (int r = 0; r < Rows; r++)
		{
			for (int c = 0; c < Cols; c++)
			{
				TestBlock(c, r);

				// Destroy blocks if we matched 3 of a kind
				if (collector.Count >= 3)
				{
					// Destroy all the blocks
					while (collector.Count > 0)
					{
						ScoreBlock(collector[0]);
						ClearBlock(collector[0]);
						collector.RemoveAt(0);
					}
				}

				// Finish testing this block
				currentlyTesting = null;
				if (collector.Count > 0)
					collector.Clear();
			}
		}
	}

	public void TestForClear(int x, int y)
	{
		CopyBoard();
		collector.Clear();

		// Only the currently moved block can trigger a clear
		TestBlock(x, y);

		// Destroy blocks if we matched 3 of a kind
		if (collector.Count >= 3)
		{
			// Destroy all the blocks
			while (collector.Count > 0)
			{
				ScoreBlock(collector[0]);
				ClearBlock(y, x);
				collector.RemoveAt(0);
			}
		}

		// Finish testing this block
		currentlyTesting = null;
		collector.Clear();
	}
}

