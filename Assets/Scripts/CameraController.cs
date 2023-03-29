using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed;
    public float scrollSpeed;
    Camera main;
    float deltaZoom;

    private const float minCameraZoom = 30f;
    private const float maxCameraZoom = 150f;

    private void Start()
    {
        Application.targetFrameRate = 60;
        main = Camera.main;
        main.orthographicSize = 100f;
    }

    private void FixedUpdate()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            deltaZoom = Mathf.Clamp(main.orthographicSize + (-Input.mouseScrollDelta.y * scrollSpeed * Time.deltaTime), minCameraZoom, maxCameraZoom);
            main.orthographicSize = deltaZoom;
        }

        float deltaX = Input.GetAxisRaw("Horizontal");
        float deltaY = Input.GetAxisRaw("Vertical");

        Vector3 delta = new Vector3(deltaX, deltaY, 0);

        if (delta.magnitude > 1f)
        {
            delta.Normalize();
        }

        if (delta.sqrMagnitude > 0)
        {
            Vector3 move = moveSpeed * Time.deltaTime * delta;
            Vector3 newPos = transform.position + move;
            if (newPos.x > 0 && newPos.x < 420 && newPos.y > 0 && newPos.y < 200)
            {
                transform.position += move;
            }
        }
    }

    public void SetCameraPositionAndZoom(Vector3 position, float zoom)
    {
        transform.position = position;
        main.orthographicSize = Mathf.Clamp(zoom, minCameraZoom, maxCameraZoom); ;
    }
}
