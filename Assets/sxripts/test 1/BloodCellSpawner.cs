using System.Collections;
using UnityEngine;

public class BloodCellSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject bloodCellPrefab;

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
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float waitTime =
                Random.Range(minSpawnTime, maxSpawnTime);

            yield return new WaitForSeconds(waitTime);

            if (currentCount < maxCells)
            {
                SpawnCell();
            }
        }
    }

    void SpawnCell()
    {
        Vector3 randomPos =
            transform.position +
            Random.insideUnitSphere * spawnRadius;

        GameObject cell =
            Instantiate(bloodCellPrefab, randomPos, Quaternion.identity);

        currentCount++;

        // Optional: track destruction safely
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