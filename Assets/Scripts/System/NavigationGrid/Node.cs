using UnityEngine;
using System.Collections;
using SimpleJSON;

[System.Serializable]
public class Node : IHeapItem<Node> {
	
	public bool blocked;
	float posX, posY, posZ;

	public int id, idX, idY;

	public int gCost;
	public int hCost;
	public int parent;
	int heapIndex;

	public Vector3 position => new Vector3(posX, posY, posZ);
	public Node(bool blocked, Vector3 pos,int id, int idX, int idY) {
		this.blocked = blocked;
		posX = pos.x;
		posY = pos.y;
		posZ = pos.z;
		this.id = id;
		this.idX = idX;
		this.idY = idY;

	}

	public int fCost {
		get {
			return gCost + hCost;
		}
	}

	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}
