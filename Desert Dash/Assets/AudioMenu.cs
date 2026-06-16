using UnityEngine;
public class AudioMenu : MonoBehaviour
{
    [Header("----------Audio Source----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [Header("----------Audio Clip----------")]

    public AudioClip background;
    public AudioClip movement;
    public AudioClip collectcoins;
    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }
    public void playSFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
