using System.Collections.Generic;
using UnityEngine;

public class NucleusSpawner : MonoBehaviour
{
    [Header("Spawn")]
    public NucleusAlignment3D boidPrefab;
    public int count = 100;
    public float spawnRadius = 10f;

    [Header("Runtime Boids")]
    public List<NucleusAlignment3D> boids = new List<NucleusAlignment3D>();

    [Header("Flock Core")]
    public Transform centerPoint;

    void Start()
    {
        SpawnBoids();
    }

    public void SpawnBoids()
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;

            NucleusAlignment3D boid =
                Instantiate(boidPrefab, pos, Random.rotation);

            boid.spawner = this;
            boid.centerPoint = centerPoint;

            boids.Add(boid);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}