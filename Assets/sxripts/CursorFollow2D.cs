using UnityEngine;
using UnityEngine.InputSystem;

public class CursorFollow2D : MonoBehaviour
{
    public Camera cam;
    public float moveSpeed = 10f;

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        Vector3 dir = worldPos - transform.position;

        transform.position += dir * moveSpeed * Time.deltaTime;
    }
}