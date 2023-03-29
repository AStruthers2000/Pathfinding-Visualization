using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS_PathfindingVisual : MonoBehaviour
{
    private Grid<BFS_PathNode> grid;
    private Mesh mesh;
    private bool updateMesh;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void SetGrid(Grid<BFS_PathNode> grid)
    {
        this.grid = grid;
        UpdateVisual();
        grid.OnGridObjectChanged += Grid_OnGridValueChanged;
    }

    private void Grid_OnGridValueChanged(object sender, Grid<BFS_PathNode>.OnGridObjectChangedEventArgs e)
    {
        updateMesh = true;
    }

    private void LateUpdate()
    {
        if (updateMesh)
        {
            updateMesh = false;
            UpdateVisual();
        }
    }

    private void UpdateVisual()
    {
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();

                BFS_PathNode pathNode = grid.GetGridObject(x, y);

                if (pathNode.isWalkable)
                {
                    quadSize = Vector3.zero;
                    //quadSize = new Vector3(2, 2, 0) * grid.GetCellSize();
                }

                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, Vector2.zero, Vector2.zero);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}
