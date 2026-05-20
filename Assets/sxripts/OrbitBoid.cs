using System.Collections.Generic;
using UnityEngine;

public class OrbitBoid : MonoBehaviour
{
    // Shared list for ALL boids
    public static List<OrbitBoid> allBoids =
        new List<OrbitBoid>();

    [Header("Center")]
    public Transform centerPoint;

    public float maxDistance = 5f;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float maxForce = 5f;

    [Header("Orbit")]
    public float orbitStrength = 3f;
    public float returnStrength = 8f;

    [Header("Flocking")]
    public float neighborRadius = 2f;
    public float separationRadius = 0.75f;

    public float cohesionStrength = 1f;
    public float separationStrength = 4f;
    public float alignmentStrength = 1f;

    [Header("Pulse")]
    public float pulseStrength = 2f;
    public float pulseSpeed = 2f;

    Vector2 velocity;

    void OnEnable()
    {
        allBoids.Add(this);
    }

    void OnDisable()
    {
        allBoids.Remove(this);
    }

    void Start()
    {
        velocity =
            Random.insideUnitCircle * moveSpeed;
    }

    void Update()
    {
        Vector2 position = transform.position;

        Vector2 cohesion = Vector2.zero;
        Vector2 alignment = Vector2.zero;
        Vector2 separation = Vector2.zero;

        int count = 0;

        //--------------------------------
        // CHECK NEIGHBORS
        //--------------------------------

        foreach (OrbitBoid other in allBoids)
        {
            if (other == this)
                continue;

            float dist =
                Vector2.Distance(position, other.transform.position);

            if (dist < neighborRadius)
            {
                cohesion += (Vector2)other.transform.position;
                alignment += other.velocity;

                count++;

                if (dist < separationRadius)
                {
                    Vector2 away =
                        position - (Vector2)other.transform.position;

                    separation += away.normalized / dist;
                }
            }
        }

        //--------------------------------
        // FLOCK AVERAGES
        //--------------------------------

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

        //--------------------------------
        // CENTER FORCES
        //--------------------------------

        Vector2 toCenter =
            ((Vector2)centerPoint.position - position);

        float distance =
            toCenter.magnitude;

        Vector2 centerDir =
            toCenter.normalized;

        // Orbit movement
        Vector2 orbitDir =
            new Vector2(-centerDir.y, centerDir.x);

        Vector2 orbitForce =
            orbitDir * orbitStrength;

        //--------------------------------
        // RETURN FORCE
        //--------------------------------

        Vector2 returnForce = Vector2.zero;

        if (distance > maxDistance)
        {
            float extra =
                distance - maxDistance;

            returnForce =
                centerDir * extra * returnStrength;
        }

        //--------------------------------
        // PULSE
        //--------------------------------

        float pulse =
            Mathf.Sin(Time.time * pulseSpeed);

        Vector2 pulseForce =
            -centerDir * pulse * pulseStrength;

        //--------------------------------
        // FINAL MOVEMENT
        //--------------------------------

        Vector2 acceleration =
            cohesion +
            alignment +
            separation +
            orbitForce +
            returnForce +
            pulseForce;

        acceleration =
            Vector2.ClampMagnitude(acceleration, maxForce);

        velocity += acceleration * Time.deltaTime;

        velocity =
            Vector2.ClampMagnitude(velocity, moveSpeed);

        transform.position +=
            (Vector3)(velocity * Time.deltaTime);
    }
}