using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heatmap_Visual : MonoBehaviour
{
    /*
    private Grid grid;
    private Mesh mesh;
    private bool updateMesh;
    public void SetGrid(Grid grid)
    {
        this.grid = grid;
        UpdateHeatmap();
        grid.OnGridValueChanged += Grid_OnGridValueChanged;
    }

    private void Grid_OnGridValueChanged(object sender, Grid.OnGridValueChangedEventArgs e)
    {
        updateMesh = true;
    }

    private void LateUpdate()
    {
        if (updateMesh)
        {
            updateMesh = false;
            UpdateHeatmap();
        }
    }

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void UpdateHeatmap()
    {
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();

                int gridVal = grid.GetValue(x, y);
                float gridValNormalized = (float)gridVal / Grid.HEATMAP_MAX_VALUE;
                Vector2 gridValUV = new Vector2(gridValNormalized, 0f);

                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + quadSize * 0.5f, 0f, quadSize, gridValUV, gridValUV);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
    */
}
