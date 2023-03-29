using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CodeMonkey.Utils;

public class AStar_PathfindingDebugStepVisual : MonoBehaviour
{
    [SerializeField] Sprite start; 
    [SerializeField] Sprite finish;
    [SerializeField] Sprite wall;
    

    public static AStar_PathfindingDebugStepVisual Instance { get; private set; }

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

    public void Setup(Grid<AStar_PathNode> grid)
    {
        visualNodeArray = new Transform[grid.GetWidth(), grid.GetHeight()];

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                //Vector3 gridPosition = new Vector3(x, y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f;
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
            float autoShowSnapshotsTimerMax = 0.05f;
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

    public void TakeSnapshot(Grid<AStar_PathNode> grid, AStar_PathNode current, List<AStar_PathNode> openList, List<AStar_PathNode> closedList)
    {
        GridSnapshotAction gridSnapshotAction = new GridSnapshotAction();
        gridSnapshotAction.AddAction(HideNodeVisuals);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                AStar_PathNode pathNode = grid.GetGridObject(x, y);

                int gCost = pathNode.gCost;
                int hCost = pathNode.hCost;
                int fCost = pathNode.fCost;
                Vector3 gridPosition = new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f;
                bool isCurrent = pathNode == current;
                bool isInOpenList = openList.Contains(pathNode);
                bool isInClosedList = closedList.Contains(pathNode);
                bool isStart = pathNode.isStart;
                bool isFinish = pathNode.isFinish;
                bool isWalkable = pathNode.isWalkable;
                int tmpX = x;
                int tmpY = y;

                gridSnapshotAction.AddAction(() =>
                {
                    Transform visualNode = visualNodeArray[tmpX, tmpY];
                    SetupVisualNode(visualNode, gCost, hCost, fCost);

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
                    if (isStart)
                    {
                        backgroundColor = UtilsClass.GetColorFromString("2E70B2");
                        //visualNode.Find("sprite").GetComponent<SpriteRenderer>().sprite = start;
                    }
                    if (isFinish)
                    {
                        backgroundColor = UtilsClass.GetColorFromString("EA2380");
                        //visualNode.Find("sprite").GetComponent<SpriteRenderer>().sprite = finish;
                    }


                    visualNode.Find("sprite").GetComponent<SpriteRenderer>().color = backgroundColor;
                });
            }
        }

        gridSnapshotActionList.Add(gridSnapshotAction);
    }

    public void TakeSnapshotFinalPath(Grid<AStar_PathNode> grid, List<AStar_PathNode> path)
    {
        GridSnapshotAction gridSnapshotAction = new GridSnapshotAction();
        gridSnapshotAction.AddAction(HideNodeVisuals);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                AStar_PathNode pathNode = grid.GetGridObject(x, y);

                int gCost = pathNode.gCost;
                int hCost = pathNode.hCost;
                int fCost = pathNode.fCost;
                Vector3 gridPosition = new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f;
                bool isInPath = path.Contains(pathNode);
                int tmpX = x;
                int tmpY = y;

                gridSnapshotAction.AddAction(() =>
                {
                    Transform visualNode = visualNodeArray[tmpX, tmpY];
                    SetupVisualNode(visualNode, gCost, hCost, fCost);

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
            SetupVisualNode(visualNodeTransform, 9999, 9999, 9999);
        }
    }

    private Transform CreateVisualNode(Vector3 position)
    {
        Transform visualNodeTransform = Instantiate(pfPathfindingDebugStepVisualNode, position, Quaternion.identity);
        return visualNodeTransform;
    }

    private void SetupVisualNode(Transform visualNodeTransform, int gCost, int hCost, int fCost)
    {
        if (fCost < 1000)
        {
            visualNodeTransform.Find("gCostText").GetComponent<TextMeshPro>().SetText(gCost.ToString());
            visualNodeTransform.Find("hCostText").GetComponent<TextMeshPro>().SetText(hCost.ToString());
            visualNodeTransform.Find("fCostText").GetComponent<TextMeshPro>().SetText(fCost.ToString());
        }
        else
        {
            visualNodeTransform.Find("gCostText").GetComponent<TextMeshPro>().SetText("");
            visualNodeTransform.Find("hCostText").GetComponent<TextMeshPro>().SetText("");
            visualNodeTransform.Find("fCostText").GetComponent<TextMeshPro>().SetText("");
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

