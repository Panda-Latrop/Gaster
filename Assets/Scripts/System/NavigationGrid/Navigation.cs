using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Navigation : MonoBehaviour
{
    public NavigationGrid grid;
    
    public bool FindPath(Vector3 startPos, Vector3 targetPos, ref List<Vector3> path)
    {
        path.Clear();
        bool pathSuccess = false;
        Node startNode = default;
        Node targetNode = default;

        /*Debug.Log("1 " + grid.NodeFromWorld(startPos, ref startNode) + " " +
                  "2 " + grid.NodeFromWorld(targetPos, ref targetNode) + " " +
                  "3 " + (!startNode.blocked && !targetNode.blocked) + " " +
                  "4 " + (startNode.id != targetNode.id) + " ");*/


        if (grid.NodeFromWorld(startPos, ref startNode) &&
            grid.NodeFromWorld(targetPos, ref targetNode) &&
            !startNode.blocked && !targetNode.blocked &&
            startNode.id != targetNode.id)
        {
            Heap<Node> openSet = new Heap<Node>(grid.Length);
            HashSet<Node> closedSet = new HashSet<Node>();
            Node[] neighbours = new Node[8];
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();               

                closedSet.Add(currentNode);

                if (currentNode.id == targetNode.id)
                {
                    pathSuccess = true;
                    break;
                }
                for (int i = 0; i < grid.GetNeighbours(currentNode, ref neighbours); i++)
                {
                    if (neighbours[i].blocked || closedSet.Contains(neighbours[i]))
                    {
                        continue;
                    }
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbours[i]);
                    if (newMovementCostToNeighbour < neighbours[i].gCost || !openSet.Contains(neighbours[i]))
                    {
                        neighbours[i].gCost = newMovementCostToNeighbour;
                        neighbours[i].hCost = GetDistance(neighbours[i], targetNode);
                        neighbours[i].parent = currentNode.id;

                        if (!openSet.Contains(neighbours[i]))
                            openSet.Add(neighbours[i]);
                    }
                }
            }
        }   
        if (pathSuccess)
        {
            //Debug.Log("1 ");
            RetracePath(startNode, targetNode,ref path);
            return true;
        }
        else
        {
            //Debug.Log("2 ");
            return false;
        }
    }

    void RetracePath(Node startNode, Node endNode, ref List<Vector3> path)
    {
        Vector2 directionOld = Vector2.zero;
        //int addedOld = 0;
        List<Node> nodes = new List<Node>();
        Node currentNode = endNode;

        //nodes.Add(grid.GetNode(startNode.id));
        while (currentNode.id != startNode.id)
        {
            nodes.Add(currentNode);
            currentNode = grid.GetNode(currentNode.parent);
        }
        for (int i = 1; i < nodes.Count-1; i++)
        {
            //path.Add(nodes[i].position);
            Vector2 directionNew = new Vector2(nodes[i - 1].idX - nodes[i].idX, nodes[i - 1].idY - nodes[i].idY);
             if (directionNew != directionOld)
             {
                //if (addedOld != i - 1)
                 path.Add(nodes[i - 1].position);
                 path.Add(nodes[i].position);
                 //addedOld = i;

             }
             directionOld = directionNew;
            
        }
        if (path.Count == 0)
            path.Add(endNode.position);
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.idX - nodeB.idX);
        int dstY = Mathf.Abs(nodeA.idY - nodeB.idY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }


}
