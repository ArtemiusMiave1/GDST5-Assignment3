using UnityEngine;

public class NucleusAlignment3D : MonoBehaviour
{
    [HideInInspector]
    public NucleusSpawner spawner;

    [Header("Speed")]
    public float minSpeed = 1f;
    public float maxSpeed = 5f;

    [Header("Center")]
    public Transform centerPoint;
    public float maxRadius = 4f;

    [Header("Forces")]
    public float centerPullStrength = 6f;
    public float separationStrength = 6f;
    public float alignmentStrength = 4f;

    [Header("Aesthetic")]
    public float wanderStrength = 2f;
    public float swirlStrength = 1.5f;
    public float noiseStrength = 0.3f;

    [Header("Stability")]
    public float damping = 0.98f;
    public float neighbourRadius = 3f;
    public float separationRadius = 1.5f;

    [HideInInspector]
    public Vector3 velocity;

    void Start()
    {
        velocity = Random.insideUnitSphere * minSpeed;
    }

    Vector3 GetWander(Vector3 pos)
    {
        float t = Time.time * 0.5f;

        float x = Mathf.PerlinNoise(pos.y + t, pos.z);
        float y = Mathf.PerlinNoise(pos.x, pos.z + t);
        float z = Mathf.PerlinNoise(pos.x + t, pos.y);

        return (new Vector3(x, y, z) * 2f - Vector3.one).normalized;
    }

    void Update()
    {
        if (spawner == null || centerPoint == null) return;

        Vector3 pos = transform.position;

        Vector3 toCenter = centerPoint.position - pos;
        float dist = toCenter.magnitude;
        Vector3 dir = toCenter.normalized;

        // ----------------------------
        // CENTER PULL
        // ----------------------------
        float pullFactor = Mathf.Clamp01(dist / maxRadius);
        Vector3 centerForce = dir * pullFactor * centerPullStrength;

        // ----------------------------
        // ALIGNMENT (now using NucleusSpawner)
        // ----------------------------
        Vector3 avgVelocity = Vector3.zero;
        int count = 0;

        foreach (var other in spawner.boids)
        {
            if (other == this || other == null) continue;

            float d = Vector3.Distance(pos, other.transform.position);

            if (d < neighbourRadius)
            {
                avgVelocity += other.velocity;
                count++;
            }
        }

        Vector3 alignment = Vector3.zero;

        if (count > 0)
        {
            avgVelocity /= count;

            alignment =
                (avgVelocity.normalized * maxSpeed - velocity)
                * alignmentStrength;
        }

        // ----------------------------
        // SEPARATION
        // ----------------------------
        Vector3 separation = Vector3.zero;

        foreach (var other in spawner.boids)
        {
            if (other == this || other == null) continue;

            float d = Vector3.Distance(pos, other.transform.position);

            if (d < separationRadius && d > 0.001f)
            {
                Vector3 away = pos - other.transform.position;
                separation += away.normalized / d;
            }
        }

        separation *= separationStrength;

        // ----------------------------
        // SWIRL
        // ----------------------------
        Vector3 swirl =
            Vector3.Cross(dir, Vector3.up) * swirlStrength;

        // ----------------------------
        // WANDER + NOISE
        // ----------------------------
        Vector3 wander = GetWander(pos) * wanderStrength;
        Vector3 noise = Random.insideUnitSphere * noiseStrength;

        // ----------------------------
        // COMBINE
        // ----------------------------
        Vector3 acceleration =
            centerForce +
            alignment +
            separation +
            swirl +
            wander +
            noise;

        velocity += acceleration * Time.deltaTime;
        velocity *= damping;

        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        if (velocity.magnitude < minSpeed)
            velocity = velocity.normalized * minSpeed;

        transform.position += velocity * Time.deltaTime;
    }
}