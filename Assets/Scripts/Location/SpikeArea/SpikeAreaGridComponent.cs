using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeAreaGridComponent : MonoBehaviour
{
	[System.Serializable]
	public class SpikeAreaGridNodeClass
	{
		public Transform transform;
		public SpriteRenderer renderer;
		public int state = 0;
		public float time = 0;

		public SpikeAreaGridNodeClass(SpriteRenderer renderer)
        {
			this.renderer = renderer;
			transform = renderer.transform;
		}
	}
	[SerializeField]
	public Vector2Int gridCount;
	[SerializeField]
	protected float nodeSize;
	[HideInInspector]
	[SerializeField]
	public SpikeAreaGridNodeClass[] nodes;

	public int Length => gridCount.x * gridCount.y;

	[ContextMenu("Create Grid")]
	public void CreateGrid()
	{
		ClearGrid();
		nodes = new SpikeAreaGridNodeClass[Length];
		int id = 0;
		Vector3 position = Vector3.zero;
		Vector3 gridSize = new Vector3(gridCount.x * nodeSize, gridCount.y * nodeSize, 0.0f);
		for (int y = 0; y < gridCount.y; y++)
		{
			for (int x = 0; x < gridCount.x; x++)
			{
				position.Set(x * nodeSize + nodeSize / 2.0f - gridSize.x / 2.0f, y * nodeSize + nodeSize / 2.0f - gridSize.y / 2.0f, 0);
				position += transform.position;
				id = x + y * gridCount.x;
				GameObject go = new GameObject("node" + id);
				go.transform.position = position;
				go.transform.SetParent(transform,true);
				SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
				nodes[id] = new SpikeAreaGridNodeClass(renderer);
			}
		}
	}
	[ContextMenu("Clear Grid")]
	public void ClearGrid()
	{
		nodes = null;
        for (int i = transform.childCount-1; i >= 0; i--)
        {
			if (Application.isPlaying)
				Destroy(transform.GetChild(i).gameObject);

			else
				DestroyImmediate(transform.GetChild(i).gameObject);
        }
	}	

	


    private void OnDrawGizmosSelected()
	{
		if (nodes != null && nodes.Length > 0)
		{
			Gizmos.color = Color.cyan;
			Vector3 gridSize = new Vector3(gridCount.x * nodeSize, gridCount.y * nodeSize, 0.0f);
			Gizmos.DrawWireCube(transform.position, gridSize);

			for (int i = 0; i < nodes.Length; i++)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawWireCube(nodes[i].transform.position,
										new Vector3(nodeSize, nodeSize, 0.0f));
			}
		}
		else
		{
			Gizmos.color = Color.cyan;
			Vector3 gridSize = new Vector3(gridCount.x * nodeSize, gridCount.y * nodeSize, 0.0f);
			Gizmos.DrawWireCube(transform.position, gridSize);
			Gizmos.color = Color.magenta;
			for (int x = 0; x < gridCount.x; x++)
			{
				for (int y = 0; y < gridCount.y; y++)
				{
					Gizmos.DrawWireCube(transform.position - gridSize / 2.0f +
										new Vector3(nodeSize * x, nodeSize * y, 0.0f) +
										new Vector3(nodeSize / 2.0f, nodeSize / 2.0f, 0.0f),
										new Vector3(nodeSize, nodeSize, 0.0f));
				}
			}
		}
	}
}