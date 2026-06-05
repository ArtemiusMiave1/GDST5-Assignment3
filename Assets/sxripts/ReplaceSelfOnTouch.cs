using System.Collections;
using UnityEngine;

public class ReplaceSelfOnTouch : MonoBehaviour
{
    public float darkenTime = 1f;

    public bool infected = false;
    private Renderer rend;

    [Header("References")]
    public BoidSpawner boidSpawner;
    void Start()
    {
        rend = GetComponent<Renderer>();
        if (boidSpawner == null)
        {
            boidSpawner = FindFirstObjectByType<BoidSpawner>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (infected) return;
        if (other.CompareTag("Virus"))
        {
            infected = true;

            BoidSpawner spawner = FindFirstObjectByType<BoidSpawner>();

            if (spawner != null)
            {
                spawner.SpawnBoid(transform.position);
            }

            StartCoroutine(DarkenOverTime());

            boidSpawner.infectedCells++;
        }
    }

    IEnumerator DarkenOverTime()
    {
        Color startColor = rend.material.color;
        Color endColor = startColor * 0.2f; // much darker

        float timer = 0f;

        while (timer < darkenTime)
        {
            timer += Time.deltaTime;

            rend.material.color =
                Color.Lerp(
                    startColor,
                    endColor,
                    timer / darkenTime
                );

            yield return null;
        }

        rend.material.color = endColor;
    }
}