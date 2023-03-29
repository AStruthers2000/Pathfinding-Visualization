using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS_Pathfinding
{
    public static BFS_Pathfinding Instance { get; private set; }

    private Grid<BFS_PathNode> grid;
    private Queue<BFS_PathNode> queue;
    private List<BFS_PathNode> exploredNodes;

    private int queueOrder = 1;

    public BFS_Pathfinding(int width, int height, float gridSize, Vector3 offset)
    {
        Instance = this;
        grid = new Grid<BFS_PathNode>(width, height, gridSize, offset, (Grid<BFS_PathNode> g, int x, int y) => new BFS_PathNode(g, x, y));
    }

    public Grid<BFS_PathNode> GetGrid()
    {
        return grid;
    }

    public int GetQueueOrder()
    {
        return queueOrder;
    }

    public List<BFS_PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        BFS_PathNode startNode = grid.GetGridObject(startX, startY);
        BFS_PathNode endNode = grid.GetGridObject(endX, endY);

        queue = new Queue<BFS_PathNode>();
        exploredNodes = new List<BFS_PathNode>();

        queue.Enqueue(startNode);

        for(int x = 0; x < grid.GetWidth(); x++)
        {
            for(int y = 0; y < grid.GetHeight(); y++)
            {
                BFS_PathNode node = grid.GetGridObject(x, y);
                node.queueOrder = 0;
                node.cameFromNode = null;
            }
        }

        startNode.queueOrder = queueOrder;

        BFS_PathfindingDebugStepVisual.Instance.ClearSnapshots();
        BFS_PathfindingDebugStepVisual.Instance.TakeSnapshot(grid, startNode, queue, exploredNodes);

        while(queue.Count > 0)
        {
            BFS_PathNode currentNode = queue.Dequeue();
            if(currentNode == endNode)
            {
                //reached final node
                BFS_PathfindingDebugStepVisual.Instance.TakeSnapshot(grid, currentNode, queue, exploredNodes);
                BFS_PathfindingDebugStepVisual.Instance.TakeSnapshotFinalPath(grid, CalculatePath(endNode));
                return CalculatePath(endNode);
            }
            exploredNodes.Add(currentNode);

            foreach(BFS_PathNode neighborNode in GetNeighborList(currentNode))
            {
                if (exploredNodes.Contains(neighborNode) || neighborNode == null) continue;

                queueOrder++;
                neighborNode.queueOrder = queueOrder;
                if (!neighborNode.isWalkable)
                {
                    exploredNodes.Add(neighborNode);
                    continue;
                }

                exploredNodes.Add(neighborNode);
                neighborNode.cameFromNode = currentNode;
                queue.Enqueue(neighborNode);

                BFS_PathfindingDebugStepVisual.Instance.TakeSnapshot(grid, startNode, queue, exploredNodes);
            }
        }

        //Out of nodes in queue (searched through whole map, couldn't find a path)
        return null;
    }

    private List<BFS_PathNode> GetNeighborList(BFS_PathNode currentNode)
    {
        List<BFS_PathNode> neighborList = new List<BFS_PathNode>();

        //Left
        if (currentNode.x - 1 >= 0) neighborList.Add(GetNode(currentNode.x - 1, currentNode.y));


        //Right
        if (currentNode.x + 1 < grid.GetWidth()) neighborList.Add(GetNode(currentNode.x + 1, currentNode.y));

        //Down
        if (currentNode.y - 1 >= 0) neighborList.Add(GetNode(currentNode.x, currentNode.y - 1));

        //Up
        if (currentNode.y < grid.GetHeight()) neighborList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighborList;
    }

    public BFS_PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    private List<BFS_PathNode> CalculatePath(BFS_PathNode endNode)
    {
        List<BFS_PathNode> path = new List<BFS_PathNode>();
        path.Add(endNode);
        BFS_PathNode currentNode = endNode;
        while(currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }
}
