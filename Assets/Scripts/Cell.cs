using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cell
{
    public Point position;
    private bool isAlive;

    public Cell(Point position, bool isAlive = false)
    {
        this.position = new Point(position.x, position.y);
        this.isAlive = isAlive;
    }

    public bool IsAlive { get => isAlive; set => isAlive = value; }
}