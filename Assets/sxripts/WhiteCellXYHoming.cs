using UnityEngine;

public class BloodCellXYHoming : MonoBehaviour
{
    [Header("Detection")]
    public float zDetectionDistance = 50f;

    [Header("Movement")]
    public float moveSpeed = 5f;

    private Transform target;
    private bool isHoming = false;

    void Update()
    {
        if (BoidSpawner.instance == null) return;

        // ----------------------------
        // DISTANCE CHECK (Z TRIGGER)
        // ----------------------------
        if (!isHoming)
        {
            TryFindHomingTrigger();
            return;
        }

        // ----------------------------
        // ENSURE VALID TARGET
        // ----------------------------
        if (target == null)
        {
            PickRandomTarget();
        }

        if (BoidSpawner.instance.boids.Count == 0)
            return;

        Vector3 direction = target.position - transform.position;
        direction.z = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            direction.Normalize();
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    void TryFindHomingTrigger()
    {
        foreach (var boid in BoidSpawner.instance.boids)
        {
            if (boid == null) continue;

            float zDistance = Mathf.Abs(boid.transform.position.z - transform.position.z);

            if (zDistance <= zDetectionDistance)
            {
                isHoming = true;
                PickRandomTarget();
                return;
            }
        }
    }

    void PickRandomTarget()
    {
        var list = BoidSpawner.instance.boids;

        // remove nulls first (dead boids)
        list.RemoveAll(b => b == null);

        if (list.Count == 0)
        {
            target = null;
            return;
        }

        int index = Random.Range(0, list.Count);
        target = list[index].transform;
    }
}