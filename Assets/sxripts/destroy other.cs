using UnityEngine;

public class DestroyOther : MonoBehaviour
{
    public MusicPlayer musicPlayer;
    public int trigger = 0;

    private void Awake()
    {
        musicPlayer = FindFirstObjectByType<MusicPlayer>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Virus"))
        {
            NucleusBoid3D boid = other.GetComponent<NucleusBoid3D>();
            if (trigger == 0) musicPlayer.PlaySFX(musicPlayer.destroy);
            trigger++;
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