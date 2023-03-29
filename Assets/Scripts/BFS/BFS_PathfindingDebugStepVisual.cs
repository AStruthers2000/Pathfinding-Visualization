using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CodeMonkey.Utils;
using System;

public class BFS_PathfindingDebugStepVisual : MonoBehaviour
{
    public static BFS_PathfindingDebugStepVisual Instance { get; private set; }

    [SerializeField] private Transform pfPathfindingDebugStepVisualNode;
    private List<Transform> visualNodeList;
    private List<GridSnapshotAction> gridSnapshotActionList;
    public bool autoShowSnapshots;
    private float autoShowSnapshotsTimer;
    private Transform[,] visualNodeArray;

    private void Awake()
    {
        Instance = this;
        visualNodeList = new List<Transform>();
        gridSnapshotActionList = new List<GridSnapshotAction>();
    }

    public void Setup(Grid<BFS_PathNode> grid)
    {
        visualNodeArray = new Transform[grid.GetWidth(), grid.GetHeight()];

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {

                //Vector3 gridPosition = new Vector3(x, y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f ;
                Vector3 gridPosition = grid.GetWorldPosition(x, y) + Vector3.one * grid.GetCellSize() * 0.5f;
                Transform visualNode = CreateVisualNode(gridPosition);
                visualNodeArray[x, y] = visualNode;
                visualNodeList.Add(visualNode);
            }
        }
        HideNodeVisuals();
    }

    private void Update()
    {
        if (autoShowSnapshots)
        {
            float autoShowSnapshotsTimerMax = .05f;
            autoShowSnapshotsTimer -= Time.deltaTime;
            if (autoShowSnapshotsTimer <= 0f)
            {
                autoShowSnapshotsTimer += autoShowSnapshotsTimerMax;
                ShowNextSnapshot();
                if (gridSnapshotActionList.Count == 0)
                {
                    autoShowSnapshots = false;
                }
            }
        }
    }

    private void ShowNextSnapshot()
    {
        if (gridSnapshotActionList.Count > 0)
        {
            GridSnapshotAction gridSnapshotAction = gridSnapshotActionList[0];
            gridSnapshotActionList.RemoveAt(0);
            gridSnapshotAction.TriggerAction();
        }
    }

    public void ClearSnapshots()
    {
        Debug.Log("Should be clearing all snapshots");
        gridSnapshotActionList.Clear();

        visualNodeList.Clear();
        HideNodeVisuals();
    }

    public void TakeSnapshot(Grid<BFS_PathNode> grid, BFS_PathNode current, Queue<BFS_PathNode> openList, List<BFS_PathNode> closedList)
    {
        GridSnapshotAction gridSnapshotAction = new GridSnapshotAction();
        gridSnapshotAction.AddAction(HideNodeVisuals);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                BFS_PathNode pathNode = grid.GetGridObject(x, y);

                int queueOrder = pathNode.queueOrder;
                Vector3 gridPosition = new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f;
                bool isCurrent = pathNode == current;
                bool isInOpenList = openList.Contains(pathNode);
                bool isInClosedList = closedList.Contains(pathNode);
                int tmpX = x;
                int tmpY = y;

                gridSnapshotAction.AddAction(() =>
                {
                    Transform visualNode = visualNodeArray[tmpX, tmpY];
                    SetupVisualNode(visualNode, queueOrder);

                    Color backgroundColor = UtilsClass.GetColorFromString("636363");

                    if (isInClosedList)
                    {
                        backgroundColor = new Color(1, 0, 0);
                    }
                    if (isInOpenList)
                    {
                        backgroundColor = UtilsClass.GetColorFromString("009AFF");
                    }
                    if (isCurrent)
                    {
                        backgroundColor = new Color(0, 1, 0);
                    }

                    visualNode.Find("sprite").GetComponent<SpriteRenderer>().color = backgroundColor;
                });
            }
        }

        gridSnapshotActionList.Add(gridSnapshotAction);
    }

    public void TakeSnapshotFinalPath(Grid<BFS_PathNode> grid, List<BFS_PathNode> path)
    {
        GridSnapshotAction gridSnapshotAction = new GridSnapshotAction();
        gridSnapshotAction.AddAction(HideNodeVisuals);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                BFS_PathNode pathNode = grid.GetGridObject(x, y);

                int queueOrder = pathNode.queueOrder;
                Vector3 gridPosition = new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f;
                bool isInPath = path.Contains(pathNode);
                int tmpX = x;
                int tmpY = y;

                gridSnapshotAction.AddAction(() =>
                {
                    Transform visualNode = visualNodeArray[tmpX, tmpY];
                    SetupVisualNode(visualNode, queueOrder);

                    Color backgroundColor;

                    if (isInPath)
                    {
                        backgroundColor = new Color(0, 1, 0);
                    }
                    else
                    {
                        backgroundColor = UtilsClass.GetColorFromString("636363");
                    }

                    visualNode.Find("sprite").GetComponent<SpriteRenderer>().color = backgroundColor;
                });
            }
        }

        gridSnapshotActionList.Add(gridSnapshotAction);
    }

    private void HideNodeVisuals()
    {
        foreach (Transform visualNodeTransform in visualNodeList)
        {
            SetupVisualNode(visualNodeTransform, 9999);
        }
    }

    private Transform CreateVisualNode(Vector3 position)
    {
        Transform visualNodeTransform = Instantiate(pfPathfindingDebugStepVisualNode, position, Quaternion.identity);
        return visualNodeTransform;
    }

    private void SetupVisualNode(Transform visualNodeTransform, int queueOrder)
    {
        if (queueOrder < 1000)
        {
            visualNodeTransform.Find("queueOrder").GetComponent<TextMeshPro>().SetText(queueOrder.ToString());
        }
        else
        {
            visualNodeTransform.Find("queueOrder").GetComponent<TextMeshPro>().SetText("");
        }
    }

    private class GridSnapshotAction
    {

        private Action action;

        public GridSnapshotAction()
        {
            action = () => { };
        }

        public void AddAction(Action action)
        {
            this.action += action;
        }

        public void TriggerAction()
        {
            action();
        }

    }
}
