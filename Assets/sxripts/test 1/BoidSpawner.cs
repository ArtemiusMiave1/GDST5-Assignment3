using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoidSpawner : MonoBehaviour
{
    public static BoidSpawner instance;

    public Color healthyColor = Color.red;
    public Color infectedColor = new Color(0.2f, 0f, 0.2f);

    public int infectedCells = 0;
    public int infectedTarget = 100;

    [Header("Spawn")]
    public NucleusBoid3D boidPrefab;
    public int boidCount = 50;
    public float spawnRadius = 5f;

    [Header("References")]
    public Transform centerPoint;

    [Header("Line Target (IMPORTANT)")]
    public Transform directorObject;

    [Header("Boids")]
    public List<NucleusBoid3D> boids = new List<NucleusBoid3D>();

    [Header("UI")]
    public TextMeshProUGUI countText;
    public TextMeshProUGUI infectedText;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
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

            SpawnBoid(pos);
        }
    }

    public NucleusBoid3D SpawnBoid(Vector3 position)
    {
        NucleusBoid3D boid =
            Instantiate(boidPrefab, position, Quaternion.identity);

        // ----------------------------
        // CORE REFERENCES
        // ----------------------------
        boid.spawner = this;
        boid.centerPoint = centerPoint;

        boids.Add(boid);

        // ----------------------------
        // LINE ATTACHMENT (NEW PART)
        // ----------------------------
        LineToTarget line = boid.GetComponent<LineToTarget>();

        if (line == null)
        {
            line = boid.gameObject.AddComponent<LineToTarget>();
        }

        if (boids.Count > 1)
        {
            int randomIndex = Random.Range(0, boids.Count);
            line.target = boids[randomIndex].transform;
        }
        else
        {
            line.target = directorObject; // fallback
        }

        return boid;
    }

    public void RemoveBoid(NucleusBoid3D boid)
    {
        if (boids.Contains(boid))
        {
            boids.Remove(boid);
        }
    }

    void Update()
    {
        countText.text = "Viruses = " + boids.Count;

        infectedText.text =
            "Infection = " + Mathf.Min(infectedCells, infectedTarget) + "%";

        float infectionPercent =
            (float)infectedCells / infectedTarget;

    }
}