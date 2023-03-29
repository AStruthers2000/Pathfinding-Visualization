using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Visualization : MonoBehaviour
{
    [SerializeField] GameObject AreYouSure;
    [SerializeField] GameObject WidthEnterBox;
    [SerializeField] GameObject HeightEnterBox;
    [SerializeField] GameObject ConfigBox;
    [SerializeField] GameObject VisualizationPanel;
    [SerializeField] GameObject WarningTitle;
    [SerializeField] GameObject AStarTitle;
    [SerializeField] GameObject BFSTitle;
    [SerializeField] GameObject AStarStats;
    [SerializeField] GameObject BFSStats;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject finishButton;
    [SerializeField] GameObject wallButton;
    [SerializeField] private GameObject RunButton;

    private const int MAX_GRID_SIZE = 20;
    private const int MIN_GRID_SIZE = 3;

    private const float cellSize = 10f;

    private bool StartedVisualization = false;

    private void Start()
    {
        startButton.GetComponent<Button>().interactable = true;
        finishButton.GetComponent<Button>().interactable = true;
        wallButton.GetComponent<Button>().interactable = true;
        RunButton.GetComponent<Button>().interactable = true;

        if (GridConfiguration.isHardReset)
        {
            AreYouSure.SetActive(false);
            ConfigBox.SetActive(true);
            VisualizationPanel.SetActive(false);
        }
        else
        {
            AreYouSure.SetActive(false);
            ConfigBox.SetActive(false);
            VisualizationPanel.SetActive(true);

            HeightEnterBox.GetComponent<TMP_InputField>().text = GridConfiguration.height.ToString();
            WidthEnterBox.GetComponent<TMP_InputField>().text = GridConfiguration.width.ToString();
            GridConfiguration.isHardReset = true;
            OnClick_CreateGrid();

        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            AreYouSure.SetActive(true);
            ConfigBox.SetActive(false);
            VisualizationPanel.SetActive(false);
        }
    }

    public void OnClick_MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("HomeScreen");
    }

    public void OnClick_Cancel()
    {
        Time.timeScale = 1;
        AreYouSure.SetActive(false);
        if (StartedVisualization)
        {
            ConfigBox.SetActive(false);
            VisualizationPanel.SetActive(true);
        }
        else
        {
            ConfigBox.SetActive(true);
            VisualizationPanel.SetActive(false);
        }
    }

    public void OnWidthValueSubmitted()
    {
        string val = WidthEnterBox.GetComponent<TMP_InputField>().text;
        if(val == "")
        {
            val = "10";
        }
        WidthEnterBox.GetComponent<TMP_InputField>().text = Mathf.Clamp(int.Parse(val), MIN_GRID_SIZE, MAX_GRID_SIZE).ToString();
    }

    public void OnHeightValueSubmitted()
    {
        string val = HeightEnterBox.GetComponent<TMP_InputField>().text;
        Debug.Log(val);
        if (val == "")
        {
            val = "10";
        }
        HeightEnterBox.GetComponent<TMP_InputField>().text = Mathf.Clamp(int.Parse(val), MIN_GRID_SIZE, MAX_GRID_SIZE).ToString();
    }

    public void OnClick_CreateGrid()
    {
        ConfigBox.SetActive(false);
        VisualizationPanel.SetActive(true);

        StartedVisualization = true;
        Time.timeScale = 1;

        int gridX = int.Parse(WidthEnterBox.GetComponent<TMP_InputField>().text);
        int gridY = int.Parse(HeightEnterBox.GetComponent<TMP_InputField>().text);

        GridConfiguration.width = gridX;
        GridConfiguration.height = gridY;

        Vector3 offset = new Vector3(gridX * cellSize + cellSize * 2, 0, 0);

        Vector3 midpoint = new Vector3(((2 * gridX * cellSize) + cellSize * 2) / 2, (gridY * cellSize) / 2, -10);
        Camera.main.GetComponent<CameraController>().SetCameraPositionAndZoom(midpoint, ((gridX + gridY) / 2) * cellSize * 2);

        WarningTitle.transform.position = new Vector3(midpoint.x, midpoint.y * 2 + 80, 0);
        AStarTitle.transform.position = new Vector3((gridX * cellSize) / 2, WarningTitle.transform.position.y - 45, 0);
        BFSTitle.transform.position = new Vector3(AStarTitle.transform.position.x * 2 + ((gridX * cellSize) / 2) + cellSize * 2, WarningTitle.transform.position.y - 45, 0);

        AStarStats.transform.position = new Vector3(0, AStarTitle.transform.position.y - 25, 0);
        BFSStats.transform.position = new Vector3(offset.x, BFSTitle.transform.position.y - 25, 0);

        PathfindingManager p = GameObject.FindGameObjectWithTag("PathfindingManager").GetComponent<PathfindingManager>();
        p.StartPathfinding(gridX, gridY, offset);

    }

    public void ToggleStart()
    {
        startButton.GetComponent<Button>().interactable = !startButton.GetComponent<Button>().interactable;
        finishButton.GetComponent<Button>().interactable = true;
        wallButton.GetComponent<Button>().interactable = true;
    }

    public void ToggleFinish()
    {
        finishButton.GetComponent<Button>().interactable = !finishButton.GetComponent<Button>().interactable;
        startButton.GetComponent<Button>().interactable = true;
        wallButton.GetComponent<Button>().interactable = true;
    }

    public void ToggleWall()
    {
        wallButton.GetComponent<Button>().interactable = !wallButton.GetComponent<Button>().interactable;
        startButton.GetComponent<Button>().interactable = true;
        finishButton.GetComponent<Button>().interactable = true;
    }

    public void OnClick_HardReset()
    {
        GridConfiguration.isHardReset = true;
        SceneManager.LoadScene("Visualization");
    }

    public void OnClick_SoftReset()
    {
        GridConfiguration.isHardReset = false;
        SceneManager.LoadScene("Visualization");
    }

    public void UpdateStats(bool aStar, int nodesVisited, int nodesFinal, double executionTime)
    {
        string msg = string.Format("Nodes Visited: {0}\nNodes In Final Path: {1}\nExecution Time (seconds): {2}", nodesVisited, nodesFinal, executionTime);
        if (aStar)
        {
            AStarStats.GetComponent<TextMeshProUGUI>().text = msg;
        }
        else
        {
            BFSStats.GetComponent<TextMeshProUGUI>().text = msg;
        }
    }
}

public static class GridConfiguration
{
    public static int width, height;
    public static bool isHardReset = true; 
}

