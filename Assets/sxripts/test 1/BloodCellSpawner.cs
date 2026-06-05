using System.Collections;
using UnityEngine;

public class BloodCellSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject bloodCellPrefab;
    public GameObject infectedBloodCellPrefab;

    [Header("References")]
    public BoidSpawner boidSpawner;

    [Header("Spawn Area")]
    public float spawnRadius = 5f;

    [Header("Timing")]
    public float minSpawnTime = 0.5f;
    public float maxSpawnTime = 2f;

    [Header("Options")]
    public int maxCells = 100;

    private int currentCount = 0;

    void Start()
    {
        if (boidSpawner == null)
        {
            boidSpawner = FindFirstObjectByType<BoidSpawner>();
        }

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float waitTime =
                Random.Range(minSpawnTime, maxSpawnTime);

            yield return new WaitForSeconds(waitTime);

            SpawnCell();

        }
    }

    void SpawnCell()
    {
        Vector3 randomPos =
            transform.position +
            Random.insideUnitSphere * spawnRadius;

        GameObject prefabToSpawn = bloodCellPrefab;

        if (boidSpawner != null)
        {
            float infectionChance =
                ((float)boidSpawner.infectedCells*(float)0.8) /
                boidSpawner.infectedTarget;

            if (Random.value < infectionChance)
            {
                prefabToSpawn = infectedBloodCellPrefab;
            }
        }

        GameObject cell =
            Instantiate(
                prefabToSpawn,
                randomPos,
                Quaternion.identity
            );

        currentCount++;

        StartCoroutine(TrackCell(cell));
    }

    IEnumerator TrackCell(GameObject cell)
    {
        while (cell != null)
        {
            yield return null;
        }

        currentCount--;
    }
}