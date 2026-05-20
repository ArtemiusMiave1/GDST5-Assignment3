using System.Collections.Generic;
using UnityEngine;

public class PulsatingBoid : MonoBehaviour
{
    [Header("Boids")]
    public List<PulsatingBoid> allBoids;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float maxForce = 5f;
    public float neighborRadius = 2.5f;
    public float separationRadius = 1f;

    [Header("Flocking")]
    public float cohesionStrength = 2f;
    public float alignmentStrength = 1f;
    public float separationStrength = 3f;

    [Header("Pulse")]
    public Transform centerPoint;
    public float pulseStrength = 4f;
    public float pulseSpeed = 2f;

    [Header("Noise")]
    public float noiseStrength = 0.5f;

    Vector2 velocity;

    void Start()
    {
        velocity = Random.insideUnitCircle;
    }

    void Update()
    {
        Vector2 position = transform.position;

        Vector2 cohesion = Vector2.zero;
        Vector2 alignment = Vector2.zero;
        Vector2 separation = Vector2.zero;

        int count = 0;

        foreach (PulsatingBoid other in allBoids)
        {
            if (other == this)
                continue;

            float distance =
                Vector2.Distance(position, other.transform.position);

            if (distance < neighborRadius)
            {
                cohesion += (Vector2)other.transform.position;
                alignment += other.velocity;

                count++;

                if (distance < separationRadius)
                {
                    Vector2 away =
                        position - (Vector2)other.transform.position;

                    separation += away.normalized / distance;
                }
            }
        }

        // Average neighbor data
        if (count > 0)
        {
            cohesion /= count;

            cohesion =
                (cohesion - position).normalized
                * cohesionStrength;

            alignment /= count;

            alignment =
                alignment.normalized
                * alignmentStrength;

            separation =
                separation.normalized
                * separationStrength;
        }

        // Pulsating force
        Vector2 fromCenter =
            (position - (Vector2)centerPoint.position).normalized;

        float pulse =
            Mathf.Sin(Time.time * pulseSpeed);

        Vector2 pulseForce =
            fromCenter * pulse * pulseStrength;

        // Random wobble
        Vector2 noise =
            Random.insideUnitCircle * noiseStrength;

        // Combine all forces
        Vector2 acceleration =
            cohesion +
            alignment +
            separation +
            pulseForce +
            noise;

        acceleration =
            Vector2.ClampMagnitude(acceleration, maxForce);

        velocity += acceleration * Time.deltaTime;

        velocity =
            Vector2.ClampMagnitude(velocity, moveSpeed);

        transform.position +=
            (Vector3)(velocity * Time.deltaTime);

        // Rotate toward movement
        if (velocity.sqrMagnitude > 0.01f)
        {
            float angle =
                Mathf.Atan2(velocity.y, velocity.x)
                * Mathf.Rad2Deg;

            transform.rotation =
                Quaternion.Euler(0, 0, angle - 90f);
        }

        // Optional scale pulse
        float scale =
            1f + pulse * 0.15f;

        transform.localScale =
            Vector3.one * scale;
    }
}