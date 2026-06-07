using UnityEngine;

public class Boid : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float maxSteeringForce = 3f;

    [Header("Neighbour Detection")]
    public float neighbourRadius = 5f;
    public LayerMask boidLayer;

    [Header("Behaviour Weights")]
    public float separationWeight = 3f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 2f;
    public float containmentWeight = 10f;

    [Header("Containment")]
    public Transform flockCenter;
    public float boundsRadius = 20f;

    private Vector3 velocity;

    void Start()
    {
        velocity = Random.onUnitSphere * speed;
    }

    void Update()
    {
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;

        int count = 0;

        Collider[] neighbours = Physics.OverlapSphere(
            transform.position,
            neighbourRadius,
            boidLayer
        );

        foreach (Collider neighbour in neighbours)
        {
            if (neighbour.gameObject == gameObject)
                continue;

            Boid other = neighbour.GetComponent<Boid>();
            if (other == null) continue;

            Vector3 offset = transform.position - other.transform.position;
            float distance = offset.magnitude;

            if (distance > 0)
                separation += offset.normalized / distance;

            alignment += other.velocity;
            cohesion += other.transform.position;

            count++;
        }

        if (count > 0)
        {
            alignment /= count;
            alignment = alignment.normalized * speed - velocity;

            cohesion /= count;
            cohesion = (cohesion - transform.position).normalized * speed - velocity;

            separation = separation.normalized * speed - velocity;
        }

        // ----------------------------
        // STRONG CONTINUOUS CONTAINMENT
        // ----------------------------
        Vector3 containment = Vector3.zero;

        if (flockCenter != null)
        {
            Vector3 toCenter = flockCenter.position - transform.position;

            float distance = toCenter.magnitude;
            float t = distance / boundsRadius;

            // always pulls inward, stronger when farther away
            containment = toCenter.normalized * (t * t);
        }

        // ----------------------------
        // FINAL ACCELERATION
        // ----------------------------
        Vector3 acceleration =
            separation * separationWeight +
            alignment * alignmentWeight +
            cohesion * cohesionWeight +
            containment * containmentWeight;

        acceleration = Vector3.ClampMagnitude(acceleration, maxSteeringForce);

        velocity += acceleration * Time.deltaTime;
        velocity = velocity.normalized * speed;

        transform.position += velocity * Time.deltaTime;

        if (velocity.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(velocity);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, neighbourRadius);

        if (flockCenter != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(flockCenter.position, boundsRadius);
        }
    }
}