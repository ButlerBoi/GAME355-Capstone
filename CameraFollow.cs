using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        
    public float smoothSpeed = 0.125f; 
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 10f;


    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            transform.position = smoothedPosition;
        }
    }
    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Zoom(scroll);
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
