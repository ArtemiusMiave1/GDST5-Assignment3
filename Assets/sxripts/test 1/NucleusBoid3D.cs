using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NucleusBoid3D : MonoBehaviour
{
    [HideInInspector]
    public BoidSpawner spawner;

    float pushTimer = 0f;
    public float pushDuration = 0.15f;

    public float minSpeed = 1f;
    public float maxSpeed = 5f;

    [Header("test")]
    public static bool triggerPush;
    public static Vector3 pushOrigin;

    [Header("Mouse Interaction")]
    public float clickPushStrength = 10f;
    public float clickRadiusMultiplier = 1f;

    [Header("Wander")]
    public float wanderStrength = 3f;
    public float wanderSpeed = 1f;

    [Header("Center")]
    public Transform centerPoint;

    [Header("Shape")]
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

    Vector3 velocity;

    void OnEnable()
    {
        //spawner.boids.Add(this);
    }

    void OnDisable()
    {
        spawner.boids.Remove(this);
    }

    void Start()
    {
        velocity = Random.insideUnitSphere;
        spawner.boids.Add(this);
    }

    Vector3 GetWanderDirection(Vector3 pos)
    {
        float t = Time.time * wanderSpeed;

        float x = Mathf.PerlinNoise(pos.y * 0.5f + t, pos.z * 0.5f);
        float y = Mathf.PerlinNoise(pos.x * 0.5f + t, pos.z * 0.5f);
        float z = Mathf.PerlinNoise(pos.x * 0.5f + t, pos.y * 0.5f);

        Vector3 dir = new Vector3(x, y, z) * 2f - Vector3.one;

        return dir.normalized;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            triggerPush = true;
            pushOrigin = centerPoint.position;
            pushTimer = pushDuration;
        }

        Vector3 clickForce = Vector3.zero;

        if (triggerPush)
        {
            Vector3 pushDir =
                (transform.position - pushOrigin);

            float pushDist = pushDir.magnitude;

            if (pushDist > 0.01f)
            {
                pushDir.Normalize();

                float falloff = 1;
                //1f / (1f + pushDist * clickRadiusMultiplier);

                clickForce =
                    pushDir * clickPushStrength * falloff;
            }
        }

        Vector3 pos = transform.position;

        Vector3 toCenter =
            centerPoint.position - pos;

        float dist = toCenter.magnitude;
        Vector3 dir = toCenter.normalized;

        //--------------------------------
        // EXTRA STUFF (3D SWIRL + WANDER)
        //--------------------------------

        Vector3 swirl =
            Vector3.Cross((pos - centerPoint.position).normalized, Vector3.up)
            * wanderStrength;

        Vector3 wander =
            GetWanderDirection(pos) * wanderStrength;

        //--------------------------------
        // CENTER PULL (3D)
        //--------------------------------

        float pullFactor =
            Mathf.Clamp01(dist / maxRadius);

        Vector3 centerForce =
            dir * pullFactor * centerPullStrength;

        //--------------------------------
        // BOUNDARY PRESSURE
        //--------------------------------

        Vector3 boundaryForce = Vector3.zero;

        if (dist > maxRadius)
        {
            boundaryForce =
                dir * (dist - maxRadius) * 10f;
        }

        //--------------------------------
        // SEPARATION (3D SPHERE)
        //--------------------------------

        Vector3 separation = Vector3.zero;

        foreach (var other in spawner.boids)
        {
            if (other == this) continue;

            float d =
                Vector3.Distance(pos, other.transform.position);

            if (d < coreRadius)
            {
                Vector3 away =
                    pos - other.transform.position;

                separation += away.normalized / Mathf.Max(d, 0.01f);
            }
        }

        separation *= separationStrength;

        //--------------------------------
        // PULSE (3D radial breathing)
        //--------------------------------

        float pulse =
            Mathf.Sin(Time.time * pulseSpeed);

        Vector3 pulseForce =
            dir * pulse * pulseStrength;

        //--------------------------------
        // NOISE (3D jitter)
        //--------------------------------

        Vector3 noise =
            Random.insideUnitSphere * noiseStrength;

        //--------------------------------
        // COMBINE
        //--------------------------------


        Vector3 acceleration =
    centerForce +
    boundaryForce +
    separation +
    pulseForce +
    noise +
    wander +
    swirl +
    clickForce;

        acceleration += swirl + clickForce;
        velocity += acceleration * Time.deltaTime;
        velocity *= damping;

        transform.position += velocity * Time.deltaTime;
        
        //velocity += acceleration * Time.deltaTime;
        //velocity *= damping;
        //acceleration += swirl;
        //acceleration += clickForce;
        //velocity += clickForce;

        //transform.position +=
        //    velocity * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        if (velocity.magnitude < minSpeed)
        {
            velocity = velocity.normalized * minSpeed;
        }

        if (triggerPush)
        {
            pushTimer -= Time.deltaTime;

            if (pushTimer <= 0f)
                triggerPush = false;
        }
    }
}



//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class NucleusBoid3D : MonoBehaviour
//{
//    [Header("test")]
//    public static List<NucleusBoid3D> allBoids = new List<NucleusBoid3D>();
//    public static bool triggerPush;
//    public static Vector3 pushOrigin;

//    [Header("Mouse Interaction")]
//    public float clickPushStrength = 10f;
//    public float clickRadiusMultiplier = 1f;

//    [Header("Wander")]
//    public float wanderStrength = 3f;
//    public float wanderSpeed = 1f;

//    [Header("Center")]
//    public Transform centerPoint;

//    [Header("Shape")]
//    public float maxRadius = 4f;
//    public float coreRadius = 1.5f;

//    [Header("Forces")]
//    public float centerPullStrength = 8f;
//    public float separationStrength = 6f;
//    public float damping = 0.98f;

//    [Header("Pulse")]
//    public float pulseStrength = 3f;
//    public float pulseSpeed = 2f;

//    [Header("Noise")]
//    public float noiseStrength = 0.4f;

//    Vector3 velocity;

//    void OnEnable()
//    {
//        allBoids.Add(this);
//    }

//    void OnDisable()
//    {
//        allBoids.Remove(this);
//    }

//    void Start()
//    {
//        velocity = Random.insideUnitSphere;
//    }

//    Vector3 GetWanderDirection(Vector3 pos)
//    {
//        float t = Time.time * wanderSpeed;

//        float x = Mathf.PerlinNoise(pos.y * 0.5f + t, pos.z * 0.5f);
//        float y = Mathf.PerlinNoise(pos.x * 0.5f + t, pos.z * 0.5f);
//        float z = Mathf.PerlinNoise(pos.x * 0.5f + t, pos.y * 0.5f);

//        Vector3 dir = new Vector3(x, y, z) * 2f - Vector3.one;

//        return dir.normalized;
//    }

//    void Update()
//    {
//        if (Mouse.current.leftButton.wasPressedThisFrame) // Left click
//        {
//            triggerPush = true;
//            pushOrigin = centerPoint.position;
//        }

//        Vector3 clickForce = Vector3.zero;

//        if (triggerPush)
//        {
//            Vector3 pushDir =
//                (transform.position - pushOrigin);

//            float pushDist = pushDir.magnitude;

//            if (pushDist > 0.01f)
//            {
//                print(transform.position);
//                print(pushDir);
//                pushDir.Normalize();

//                float falloff = 1;
//                    //1f / (1f + pushDist * clickRadiusMultiplier);

//                clickForce =
//                    pushDir * clickPushStrength * falloff;
//            }
//        }

//        Vector3 pos = transform.position;

//        Vector3 toCenter =
//            centerPoint.position - pos;

//        float dist = toCenter.magnitude;
//        Vector3 dir = toCenter.normalized;

//        //--------------------------------
//        // EXTRA STUFF (3D SWIRL + WANDER)
//        //--------------------------------

//        Vector3 swirl =
//            Vector3.Cross((pos - centerPoint.position).normalized, Vector3.up)
//            * wanderStrength;

//        Vector3 wander =
//            GetWanderDirection(pos) * wanderStrength;

//        //--------------------------------
//        // CENTER PULL (3D)
//        //--------------------------------

//        float pullFactor =
//            Mathf.Clamp01(dist / maxRadius);

//        Vector3 centerForce =
//            dir * pullFactor * centerPullStrength;

//        //--------------------------------
//        // BOUNDARY PRESSURE
//        //--------------------------------

//        Vector3 boundaryForce = Vector3.zero;

//        if (dist > maxRadius)
//        {
//            boundaryForce =
//                dir * (dist - maxRadius) * 10f;
//        }

//        //--------------------------------
//        // SEPARATION (3D SPHERE)
//        //--------------------------------

//        Vector3 separation = Vector3.zero;

//        foreach (var other in allBoids)
//        {
//            if (other == this) continue;

//            float d =
//                Vector3.Distance(pos, other.transform.position);

//            if (d < coreRadius)
//            {
//                Vector3 away =
//                    pos - other.transform.position;

//                separation += away.normalized / Mathf.Max(d, 0.01f);
//            }
//        }

//        separation *= separationStrength;

//        //--------------------------------
//        // PULSE (3D radial breathing)
//        //--------------------------------

//        float pulse =
//            Mathf.Sin(Time.time * pulseSpeed);

//        Vector3 pulseForce =
//            dir * pulse * pulseStrength;

//        //--------------------------------
//        // NOISE (3D jitter)
//        //--------------------------------

//        Vector3 noise =
//            Random.insideUnitSphere * noiseStrength;

//        //--------------------------------
//        // COMBINE
//        //--------------------------------


//        Vector3 acceleration =
//            centerForce +
//            boundaryForce +
//            separation +
//            pulseForce +
//            noise +
//            wander;

//        velocity += acceleration * Time.deltaTime;
//        velocity *= damping;
//        acceleration += swirl;
//        acceleration += clickForce;
//        velocity += clickForce;

//        transform.position +=
//            velocity * Time.deltaTime;

//        if (triggerPush)
//            triggerPush = false;
//    }
//}