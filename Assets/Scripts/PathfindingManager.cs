using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathfindingManager : MonoBehaviour
{
    [SerializeField] private AStar_PathfindingDebugStepVisual AStar_debugVisual;
    [SerializeField] private AStar_PathfindingVisual AStar_visual;
    [SerializeField] private BFS_PathfindingDebugStepVisual BFS_debugVisual;
    [SerializeField] private BFS_PathfindingVisual BFS_visual;
    [SerializeField] private GameObject StartObject;
    [SerializeField] private GameObject FinishObject;
    [SerializeField] private GameObject WallObject;

    [SerializeField] GameObject startButton;
    [SerializeField] GameObject finishButton;
    [SerializeField] GameObject wallButton;
    [SerializeField] GameObject RunButton;

    private AStar_Pathfinding AStar;
    private BFS_Pathfinding BFS;

    [SerializeField] private Vector3 bfsGridOffset;
    [SerializeField] private float gridSize = 10f;

    private bool isPathfindingStarted = false;

    private GameObject Start;
    private GameObject Finish;
    private GameObject FakeStart;
    private GameObject FakeFinish;

    private List<GameObject> wallSpawns = new List<GameObject>();

    private enum PlacingObject
    {
        Default = 0,
        Start = 1,
        Finish = 2,
        Wall = 3
    };

    private PlacingObject currentSelection = PlacingObject.Default;

    public void StartPathfinding(int gridX, int gridY, Vector3 bfsOffset)
    {
        bfsGridOffset = bfsOffset;

        //bfsGridOffset *= gridSize;

        AStar = new AStar_Pathfinding(gridX, gridY, gridSize, Vector3.zero);
        BFS = new BFS_Pathfinding(gridX, gridY, gridSize, bfsGridOffset);

        AStar_debugVisual.Setup(AStar.GetGrid());
        AStar_visual.SetGrid(AStar.GetGrid());

        BFS_debugVisual.Setup(BFS.GetGrid());
        BFS_visual.SetGrid(BFS.GetGrid());

        isPathfindingStarted = true;
    }

    public void UpdateSelection(int selection)
    {
        currentSelection = (PlacingObject)selection;
    }

    public void OnClick_RunAlgs()
    {
        if (Start != null && Finish != null)
        {
            startButton.GetComponent<Button>().interactable = false;
            finishButton.GetComponent<Button>().interactable = false;
            wallButton.GetComponent<Button>().interactable = false;
            RunButton.GetComponent<Button>().interactable = false;
            UpdateSelection(0);

            AStar.GetGrid().GetXY(Start.transform.position, out int startX, out int startY);
            AStar.GetGrid().GetXY(Finish.transform.position, out int endX, out int endY);

            UI_Visualization vis = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UI_Visualization>();
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            List<AStar_PathNode> AStar_path = AStar.FindPath(startX, startY, endX, endY);
            watch.Stop();
            double AStar_time = watch.Elapsed.TotalSeconds;
            

            watch.Restart();
            List<BFS_PathNode> BFS_path = BFS.FindPath(startX, startY, endX, endY);
            watch.Stop();
            double BFS_time = watch.Elapsed.TotalSeconds;

            vis.UpdateStats(true, AStar.GetNodesVisited(), AStar_path.Count, AStar_time);
            vis.UpdateStats(false, BFS.GetQueueOrder(), BFS_path.Count, BFS_time);

            AStar_debugVisual.autoShowSnapshots = true;
            BFS_debugVisual.autoShowSnapshots = true;

            if (AStar_path != null)
            {
                
                for (int i = 0; i < AStar_path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(AStar_path[i].x, AStar_path[i].y) * 10f + Vector3.one * 5f, new Vector3(AStar_path[i + 1].x, AStar_path[i + 1].y) * 10f + Vector3.one * 5f, Color.green, int.MaxValue);
                }
            }
          
            if (BFS_path != null)
            {
                
                for (int i = 0; i < BFS_path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(BFS_path[i].x, BFS_path[i].y) * 10f + Vector3.one * 5f, new Vector3(BFS_path[i + 1].x, BFS_path[i + 1].y) * 10f + Vector3.one * 5f, Color.red, int.MaxValue);
                }
            }
        }
        else
        {
            Debug.Log("No start or end point yet");
        }
    }

    private void Update()
    {
        if (isPathfindingStarted)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
                AStar.GetGrid().GetXY(mouseWorldPosition, out int AStar_x, out int AStar_y);
                BFS.GetGrid().GetXY(mouseWorldPosition + bfsGridOffset, out int BFS_x, out int BFS_y);
                if (AStar.GetNode(AStar_x, AStar_y) != null)
                {
                    switch (currentSelection)
                    {
                        case PlacingObject.Default:
                            Debug.Log("Nothing selected");
                            break;
                        case PlacingObject.Start:
                            Debug.Log("Placing start");
                            if(Start != null)
                            {
                                Destroy(Start);
                                Destroy(FakeStart);
                            }
                            Start = Instantiate(StartObject, AStar.GetGrid().GetWorldPosition(AStar_x, AStar_y) + new Vector3(1, 1, 0) * AStar.GetGrid().GetCellSize() * 0.5f, Quaternion.identity);
                            FakeStart = Instantiate(StartObject, BFS.GetGrid().GetWorldPosition(BFS_x, BFS_y) + new Vector3(1, 1, 0) * BFS.GetGrid().GetCellSize() * 0.5f, Quaternion.identity);
                            break;
                        case PlacingObject.Finish:
                            Debug.Log("Placing finish");
                            if (Finish != null)
                            {
                                Destroy(Finish);
                                Destroy(FakeFinish);
                            }
                            Finish = Instantiate(FinishObject, AStar.GetGrid().GetWorldPosition(AStar_x, AStar_y) + new Vector3(1, 1, 0) * AStar.GetGrid().GetCellSize() * 0.5f, Quaternion.identity);
                            FakeFinish = Instantiate(FinishObject, BFS.GetGrid().GetWorldPosition(BFS_x, BFS_y) + new Vector3(1, 1, 0) * BFS.GetGrid().GetCellSize() * 0.5f, Quaternion.identity);
                            break;
                        case PlacingObject.Wall:
                            Debug.Log("Placing wall");

                            AStar.GetNode(AStar_x, AStar_y).SetIsWalkable(!AStar.GetNode(AStar_x, AStar_y).isWalkable);
                            BFS.GetNode(BFS_x, BFS_y).SetIsWalkable(!BFS.GetNode(BFS_x, BFS_y).isWalkable);

                            Vector3 a_spawnPos = AStar.GetGrid().GetWorldPosition(AStar_x, AStar_y) + new Vector3(1, 1, 0) * AStar.GetGrid().GetCellSize() * 0.5f;
                            Vector3 b_spawnPos = BFS.GetGrid().GetWorldPosition(BFS_x, BFS_y) + new Vector3(1, 1, 0) * BFS.GetGrid().GetCellSize() * 0.5f;

                            bool shouldCreate = true;
                            List<GameObject> wallsToDelete = new List<GameObject>();
                            foreach(GameObject w in wallSpawns)
                            {
                                if(w.transform.position == a_spawnPos || w.transform.position == b_spawnPos)
                                {
                                    wallsToDelete.Add(w);
                                    shouldCreate = false;
                                }
                            }
                            foreach(GameObject w in wallsToDelete)
                            {
                                Destroy(w);
                                wallSpawns.Remove(w);
                            }

                            if (shouldCreate)
                            {
                                GameObject w1 = Instantiate(WallObject, a_spawnPos, Quaternion.identity);
                                GameObject w2 = Instantiate(WallObject, b_spawnPos, Quaternion.identity);

                                wallSpawns.Add(w1);
                                wallSpawns.Add(w2);
                            }
                            break;
                        default:
                            Debug.Log("How did we end up here?");
                            break;
                    }
                }
            }
        }
    }
}
