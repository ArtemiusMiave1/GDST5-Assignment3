using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    [Header("Spawn")]
    public NucleusBoid3D boidPrefab;

    public int boidCount = 50;

    public float spawnRadius = 5f;

    [Header("References")]
    public Transform centerPoint;

    //[HideInInspector]
    public List<NucleusBoid3D> boids =
        new List<NucleusBoid3D>();

    void Start()
    {
        SpawnBoids();
    }

    void SpawnBoids()
    {
        for (int i = 0; i < boidCount; i++)
        {
            Vector3 pos =
                transform.position +
                Random.insideUnitSphere * spawnRadius;

            NucleusBoid3D boid =
                Instantiate(
                    boidPrefab,
                    pos,
                    Quaternion.identity
                );

            boid.spawner = this;
            boid.centerPoint = centerPoint;

            boids.Add(boid);
        }
    }
}


//using UnityEngine;

//public class BoidSpawner : MonoBehaviour
//{
//    public OrbitBoid boidPrefab;

//    public Transform centerPoint;

//    public int count = 50;

//    void Start()
//    {
//        for (int i = 0; i < count; i++)
//        {
//            Vector2 pos =
//                Random.insideUnitCircle * 4f;

//            OrbitBoid boid =
//                Instantiate(
//                    boidPrefab,
//                    pos,
//                    Quaternion.identity
//                );

//            boid.centerPoint = centerPoint;
//        }
//    }
//}