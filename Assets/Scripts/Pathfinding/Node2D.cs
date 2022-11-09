using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node2D: IHeapItem<Node2D> 
{
    public int gCost, hCost;

    public int GridX, GridY;
    public Node2D parent;
    int heapIndex;

    public Node2D( int gridX, int gridY)
    {
        GridX = gridX;
        GridY = gridY;
    }

    public int FCost
    {
        get
        {
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

	public int CompareTo(Node2D nodeToCompare) {
		int compare = FCost.CompareTo(nodeToCompare.FCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}