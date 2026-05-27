using UnityEngine;

public class FollowMouse3D : MonoBehaviour
{
    public Vector3 distance;
    public float test;

    
    void Start()
    {
        distance.z = 8f;
        float test = 8f;
    }
    void Update()
    {
        distance.x = Input.mousePosition.x;
        distance.y = Input.mousePosition.y;
        transform.position = Camera.main.ScreenToWorldPoint(distance);
        test += Input.GetAxis("Mouse ScrollWheel")*10;
        distance.z = test;
    }
}
