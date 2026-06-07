using UnityEngine;

public class BoidSpawner2 : MonoBehaviour
{
    public GameObject boidPrefab;
    public int boidCount = 100;
    public float spawnRadius = 20f;

    void Start()
    {
        for (int i = 0; i < boidCount; i++)
        {
            Vector3 position =
                transform.position +
                Random.insideUnitSphere * spawnRadius;

            Instantiate(
                boidPrefab,
                position,
                Random.rotation);
        }
    }
}