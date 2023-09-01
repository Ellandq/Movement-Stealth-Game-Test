using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 position;
    public Node parent;
    public float gCost;
    public float hCost;

    public Node(Vector3 position)
    {
        this.position = position;
    }

    public float FCost => gCost + hCost;
}
