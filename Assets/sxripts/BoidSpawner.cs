using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    public OrbitBoid boidPrefab;

    public Transform centerPoint;

    public int count = 50;

    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 pos =
                Random.insideUnitCircle * 4f;

            OrbitBoid boid =
                Instantiate(
                    boidPrefab,
                    pos,
                    Quaternion.identity
                );

            boid.centerPoint = centerPoint;
        }
    }
}