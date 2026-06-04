using UnityEngine;

public class FollowMouse3D : MonoBehaviour
{
    public Vector3 distance;
    public float test;

    void Start()
    {
        test = 10f;
        distance.z = test;
    }

    void Update()
    {
        distance.x = Input.mousePosition.x;
        distance.y = Input.mousePosition.y;

        test += Input.GetAxis("Mouse ScrollWheel") * 10f;

        // Clamp between 10 and 68
        test = Mathf.Clamp(test, 10f, 68f);

        distance.z = test;

        transform.position =
            Camera.main.ScreenToWorldPoint(distance);
    }
}