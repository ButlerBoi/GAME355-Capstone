using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 10f;

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        Zoom(scroll);

        // Zoom in with 'E' key
        if (Input.GetKey(KeyCode.E))
        {
            Zoom(1f);
        }

        // Zoom out with 'Q' key
        if (Input.GetKey(KeyCode.Q))
        {
            Zoom(-1f);
        }

        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f).normalized;

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    void Zoom(float increment)
    {
        // Calculate new zoom level
        float newSize = Camera.main.orthographicSize - increment * zoomSpeed * Time.deltaTime;
        newSize = Mathf.Clamp(newSize, minZoom, maxZoom);

        // Apply new zoom level
        Camera.main.orthographicSize = newSize;
    }
}
