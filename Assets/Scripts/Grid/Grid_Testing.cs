using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Grid_Testing : MonoBehaviour
{
    [SerializeField] private Heatmap_Visual heatmapVisual;
    [SerializeField] private Heatmap_VisualBool heatmapVisualBool;
    [SerializeField] private Heatmap_GenericVisual genericHeatmap;
    private Grid<HeatMapGridObject> grid;
    private void Start()
    {
        grid = new Grid<HeatMapGridObject>(30, 20, 10, new Vector3(-50, -50), (Grid<HeatMapGridObject> g, int x, int y) => new HeatMapGridObject(g, x, y));
        //heatmapVisualBool.SetGrid(grid);
    }

    
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 position = UtilsClass.GetMouseWorldPosition();
            HeatMapGridObject hm = grid.GetGridObject(position);
            if(hm != null)
            {
                hm.AddValue(5);
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            Debug.Log(grid.GetGridObject(UtilsClass.GetMouseWorldPosition()));
        }
    }
    
}


public class HeatMapGridObject
{
    private const int MIN = 0;
    private const int MAX = 100;
    private int x, y;
    private int value;
    private Grid<HeatMapGridObject> grid;

    public HeatMapGridObject(Grid<HeatMapGridObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }
    public void AddValue(int addValue)
    {
        value += addValue;
        value = Mathf.Clamp(value, MIN, MAX);
        grid.TriggerGridObjectChanged(x, y);
    }

    public float GetValueNormalized()
    {
        return (float)value / MAX;
    }

    public override string ToString()
    {
        return value.ToString();
    }
}