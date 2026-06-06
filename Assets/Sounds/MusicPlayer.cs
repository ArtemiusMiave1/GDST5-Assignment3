using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [Header("Audio source")]
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource musicSource;
    [Header("audio clip")]
    public AudioClip music;
    public AudioClip create;
    public AudioClip destroy;
    void Start()
    {
        musicSource.clip = music;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}