using UnityEngine;

public class ReplaceSelfOnTouch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        BoidSpawner spawner =
            FindFirstObjectByType<BoidSpawner>();

        if (spawner != null)
        {
            spawner.SpawnBoid(transform.position);
        }

        Destroy(gameObject);
    }
}