using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform player;
    public Camera minimapCamera;

    void LateUpdate()
    {
        if (player != null && minimapCamera != null)
        {
            Vector3 newPosition = player.position;
            newPosition.y = transform.position.y;
            transform.position = newPosition;

            transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);

            minimapCamera.transform.position = player.position + new Vector3(0f, 20f, 0f); 
            minimapCamera.transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        }
    }
}