using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using SimpleJSON;

public class NavigationGrid : MonoBehaviour {


	[SerializeField]
	protected Vector2Int gridCount;
	[SerializeField]
	protected float nodeSize;
	[HideInInspector]
	protected List<Node> nodes;
	[SerializeField]
	protected LayerMask blockLayer;
	protected FileMaster fileMaster = new FileMaster();


    public void Start()
    {
        try
        {
			LoadGrid();
		}
        catch (System.Exception)
        {
			Debug.LogWarning("Not Found " + directory + SceneManager.GetActiveScene().name.ToLower() + ".grid");
			CreateGrid();
			SaveGrid();			
		}
	}

	protected string directory => Application.dataPath + "/StreamingAssets/navigation/grids/";
	//protected string directory => Application.dataPath + "/Resources/navigation/grids/";

	[ContextMenu("Log")]
	protected void Log()
    {
		Debug.Log(nodes.Count + nodes[0].position.ToString());
    }

	[ContextMenu("Save Grid")]
	protected void SaveGrid()
	{
		fileMaster.WriteTo(directory + SceneManager.GetActiveScene().name.ToLower() + ".grid", nodes);
	#if UNITY_EDITOR
		UnityEditor.AssetDatabase.Refresh();
	#endif
	}
	[ContextMenu("Load Grid")]
	protected void LoadGrid()
	{
		nodes = fileMaster.ReadFrom<List<Node>>(directory + SceneManager.GetActiveScene().name.ToLower() + ".grid");
	}


	public int Length
	{
		get {
			return gridCount.x * gridCount.y;
		}
	}

    [ContextMenu("Create Grid")]
	public void CreateGrid() {
		nodes = new List<Node>(Length);
		bool blocked = false;
		int id = -1;
		Vector3 position = Vector3.zero;
		Vector3 gridSize = new Vector3(gridCount.x * nodeSize, gridCount.y * nodeSize, 0.0f);
		for (int y = 0; y < gridCount.y; y ++) {
			for (int x = 0; x < gridCount.x; x ++) {
				position.Set(x * nodeSize + nodeSize / 2.0f - gridSize.x/2.0f, y * nodeSize + nodeSize/2.0f - gridSize.y / 2.0f, 0);
				position += transform.position;
				id = x + y * gridCount.x;
				blocked = Physics2D.OverlapCircle(position, nodeSize/2.0f*0.9f, blockLayer);
				nodes.Add(new Node(blocked, position,id, x,y));
			}
		}
	}

	[ContextMenu("Clear Grid")]
	protected void ClearGrid()
    {
		nodes = new List<Node>();
	}

	public int GetNeighbours(Node node, ref Node[] neighbours) {

		int count = 0;
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.idX + x;
				int checkY = node.idY + y;

				if (checkX >= 0 && checkX < gridCount.x && checkY >= 0 && checkY < gridCount.y) {
					neighbours[count] = nodes[checkX + checkY * gridCount.x];
					count++;
				}
			}
		}
		return count;
	}
	
	public Node GetNode(int id)
    {
		return nodes[id];
	}

	public bool NodeFromWorld(Vector3 position, ref Node node) {

		float halfNodeSize = nodeSize / 2.0f;
		Vector3 gridSize = new Vector3(gridCount.x * nodeSize, gridCount.y * nodeSize, 0.0f);
		Vector3 posShift = position + gridSize / 2.0f - transform.position - (Vector3.one * halfNodeSize);
		int x = Mathf.RoundToInt(posShift.x / nodeSize);
		int y = Mathf.RoundToInt(posShift.y / nodeSize);
		int id = x + y * gridCount.x;
		if (x >= 0 && x < gridCount.x && y >= 0 && y < gridCount.y && id < nodes.Count)
		{
			node = nodes[id];
			return true;
        }
        else
        {
			return false;
        }
	}

    private void OnDrawGizmosSelected()
    {
		if (nodes != null && nodes.Count > 0)
		{
			Gizmos.color = Color.cyan;
			Vector3 gridSize = new Vector3(gridCount.x * nodeSize, gridCount.y * nodeSize, 0.0f);
			Gizmos.DrawWireCube(transform.position, gridSize);

			for (int i = 0; i < nodes.Count; i++)
			{
				if (nodes[i].blocked)
					Gizmos.color = Color.red;
				else
					Gizmos.color = Color.green;
				Gizmos.DrawWireCube(nodes[i].position,
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