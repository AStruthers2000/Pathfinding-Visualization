using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS_PathNode
{
    private Grid<BFS_PathNode> grid;
    public int x, y;

    public int queueOrder;

    public bool isWalkable;
    public BFS_PathNode cameFromNode;

    public BFS_PathNode(Grid<BFS_PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
        grid.TriggerGridObjectChanged(x, y);
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ") = " + queueOrder;
    }
}
