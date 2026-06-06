using UnityEngine;

public class DestroySelfWhenPassing : MonoBehaviour
{
    public float destroyZ = -50f;
    MusicPlayer musicPlayer;

    private void Awake()
    {
        musicPlayer = FindFirstObjectByType<MusicPlayer>();
    }
    void Update()
    {
        if (transform.position.z < destroyZ)
        {
            musicPlayer.PlaySFX(musicPlayer.destroy);
            Destroy(gameObject);
        }
    }
}