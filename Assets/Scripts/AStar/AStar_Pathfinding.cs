using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AStar_Pathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public static AStar_Pathfinding Instance { get; private set; }

    private Grid<AStar_PathNode> grid;
    private List<AStar_PathNode> openList;
    private List<AStar_PathNode> closedList;
    private int nodesVisited = 1;

    public AStar_Pathfinding(int width, int height, float gridSize, Vector3 offset)
    {
        Instance = this;
        grid = new Grid<AStar_PathNode>(width, height, gridSize, offset, (Grid<AStar_PathNode> g, int x, int y) => new AStar_PathNode(g, x, y));
    }

    public Grid<AStar_PathNode> GetGrid()
    {
        return grid;
    }

    public int GetNodesVisited()
    {
        return nodesVisited;
    }

    public List<AStar_PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        AStar_PathNode startNode = grid.GetGridObject(startX, startY);
        AStar_PathNode endNode = grid.GetGridObject(endX, endY);

        if(startNode == null || endNode == null)
        {
            return null;
        }

        openList = new List<AStar_PathNode> { startNode };
        closedList = new List<AStar_PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                AStar_PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        AStar_PathfindingDebugStepVisual.Instance.ClearSnapshots();
        AStar_PathfindingDebugStepVisual.Instance.TakeSnapshot(grid, startNode, openList, closedList);

        while(openList.Count > 0)
        {
            AStar_PathNode currentNode = GetLowestFCostNode(openList);
            if(currentNode == endNode)
            {
                //reached final node
                AStar_PathfindingDebugStepVisual.Instance.TakeSnapshot(grid, currentNode, openList, closedList);
                AStar_PathfindingDebugStepVisual.Instance.TakeSnapshotFinalPath(grid, CalculatePath(endNode));
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(AStar_PathNode neighborNode in GetNeighborList(currentNode))
            {
                if (closedList.Contains(neighborNode) || neighborNode == null) continue;
                if (!neighborNode.isWalkable)
                {
                    closedList.Add(neighborNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighborNode);
                if(tentativeGCost < neighborNode.gCost)
                {
                    neighborNode.cameFromNode = currentNode;
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.hCost = CalculateDistanceCost(neighborNode, endNode);
                    neighborNode.CalculateFCost();

                    if (!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
                AStar_PathfindingDebugStepVisual.Instance.TakeSnapshot(grid, currentNode, openList, closedList);
                nodesVisited++;
            }
        }

        //Out of nodes in openList (searched through whole map, couldn't find a path
        return null;
    }

    private List<AStar_PathNode> GetNeighborList(AStar_PathNode currentNode)
    {
        List<AStar_PathNode> neighborList = new List<AStar_PathNode>();
        if(currentNode.x - 1 >= 0)
        {
            //Left
            neighborList.Add(GetNode(currentNode.x - 1, currentNode.y));

            //Left Down
            if (currentNode.y - 1 >= 0) neighborList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));

            //Left Up
            if (currentNode.y + 1 < grid.GetHeight()) neighborList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }

        if(currentNode.x + 1 < grid.GetWidth())
        {
            //Right
            neighborList.Add(GetNode(currentNode.x + 1, currentNode.y));

            //Right Down
            if (currentNode.y - 1 >= 0) neighborList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));

            //Right Up
            if (currentNode.y + 1 < grid.GetHeight()) neighborList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }

        //Down
        if (currentNode.y - 1 >= 0) neighborList.Add(GetNode(currentNode.x, currentNode.y - 1));

        //Up
        if (currentNode.y < grid.GetHeight()) neighborList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighborList;
    }

    public AStar_PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }


    private List<AStar_PathNode> CalculatePath(AStar_PathNode endNode)
    {
        List<AStar_PathNode> path = new List<AStar_PathNode>();
        path.Add(endNode);
        AStar_PathNode currentNode = endNode;
        while(currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }
    private int CalculateDistanceCost(AStar_PathNode a, AStar_PathNode b)
    {
        int xDist = Mathf.Abs(a.x - b.x);
        int yDist = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDist - yDist);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDist, yDist) + MOVE_STRAIGHT_COST * remaining;
    }

    private AStar_PathNode GetLowestFCostNode(List<AStar_PathNode> pathNodeList)
    {
        AStar_PathNode lowestFCostNode = pathNodeList[0];
        for(int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }
}
