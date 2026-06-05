using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip music;

    [SerializeField] AudioSource musicSource;
    void Start()
    {
        musicSource.clip = music;
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}