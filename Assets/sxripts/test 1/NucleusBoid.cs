using System.Collections.Generic;
using UnityEngine;

public class NucleusBoid : MonoBehaviour
{
    public static List<NucleusBoid> allBoids = new List<NucleusBoid>();

    [Header("Center")]
    public Transform centerPoint;

    [Header("Nucleus Shape")]
    public float maxRadius = 4f;
    public float coreRadius = 1.5f;

    [Header("Forces")]
    public float centerPullStrength = 8f;
    public float separationStrength = 6f;
    public float damping = 0.98f;

    [Header("Pulse")]
    public float pulseStrength = 3f;
    public float pulseSpeed = 2f;

    [Header("Noise")]
    public float noiseStrength = 0.4f;

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
        velocity = Random.insideUnitCircle;
    }

    void Update()
    {
        Vector2 pos = transform.position;

        Vector2 toCenter =
            (Vector2)centerPoint.position - pos;

        float dist = toCenter.magnitude;
        Vector2 dir = toCenter.normalized;

        //--------------------------------
        // 1. STRONG CENTER PULL (NUCLEUS CORE)
        //--------------------------------

        float pullFactor =
            Mathf.Clamp01(dist / maxRadius);

        Vector2 centerForce =
            dir * pullFactor * centerPullStrength;

        //--------------------------------
        // 2. OUTER BOUNDARY PRESSURE
        //--------------------------------

        Vector2 boundaryForce = Vector2.zero;

        if (dist > maxRadius)
        {
            boundaryForce =
                dir * (dist - maxRadius) * 10f;
        }

        //--------------------------------
        // 3. BOID SEPARATION (electron spacing)
        //--------------------------------

        Vector2 separation = Vector2.zero;

        foreach (var other in allBoids)
        {
            if (other == this) continue;

            float d =
                Vector2.Distance(pos, other.transform.position);

            if (d < coreRadius)
            {
                Vector2 away =
                    (pos - (Vector2)other.transform.position);

                separation += away.normalized / Mathf.Max(d, 0.01f);
            }
        }

        separation *= separationStrength;

        //--------------------------------
        // 4. PULSE (nucleus breathing)
        //--------------------------------

        float pulse =
            Mathf.Sin(Time.time * pulseSpeed);

        Vector2 pulseForce =
            dir * pulse * pulseStrength;

        //--------------------------------
        // 5. NOISE (quantum jitter)
        //--------------------------------

        Vector2 noise =
            Random.insideUnitCircle * noiseStrength;

        //--------------------------------
        // COMBINE
        //--------------------------------

        Vector2 acceleration =
            centerForce +
            boundaryForce +
            separation +
            pulseForce +
            noise;

        velocity += acceleration * Time.deltaTime;
        velocity *= damping;

        transform.position +=
            (Vector3)(velocity * Time.deltaTime);
    }
}