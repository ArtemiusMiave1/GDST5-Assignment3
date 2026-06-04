using UnityEngine;

public class BloodCellDrift : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotationSpeed = 50f;
    public float axisChangeSpeed = 1f;

    private Vector3 currentAxis;

    void Start()
    {
        currentAxis = Random.onUnitSphere;
    }

    void Update()
    {
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;

        Vector3 targetAxis = new Vector3(
            Mathf.PerlinNoise(Time.time * axisChangeSpeed, 0f) - 0.5f,
            Mathf.PerlinNoise(0f, Time.time * axisChangeSpeed) - 0.5f,
            Mathf.PerlinNoise(Time.time * axisChangeSpeed, Time.time * axisChangeSpeed) - 0.5f
        ).normalized;

        currentAxis = Vector3.Lerp(
            currentAxis,
            targetAxis,
            Time.deltaTime
        ).normalized;

        transform.Rotate(
            currentAxis,
            rotationSpeed * Time.deltaTime,
            Space.World);
    }
}