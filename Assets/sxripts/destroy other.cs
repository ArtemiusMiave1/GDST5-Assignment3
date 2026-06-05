using UnityEngine;

public class DestroyOther : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Virus"))
        {
            NucleusBoid3D boid = other.GetComponent<NucleusBoid3D>();

            if (boid != null)
            {
                LineToTarget line = boid.GetComponent<LineToTarget>();

                if (line != null)
                {
                    Destroy(line); // remove visual line
                }

                Destroy(boid.gameObject); // destroy full boid
            }
        }
    }
}