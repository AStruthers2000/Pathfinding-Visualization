using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar_PathNode
{
    private Grid<AStar_PathNode> grid;
    public int x, y;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable;
    public bool isStart;
    public bool isFinish;
    public AStar_PathNode cameFromNode;

    public AStar_PathNode(Grid<AStar_PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
        grid.TriggerGridObjectChanged(x, y);
    }

    public void SetIsStart(bool isStart)
    {
        this.isStart = isStart;
        grid.TriggerGridObjectChanged(x, y);
    }

    public void SetIsFinish(bool isFinish)
    {
        this.isFinish = isFinish;
        grid.TriggerGridObjectChanged(x, y);
    }

    public override string ToString()
    {
        return x + ", " + y;
    }
}
