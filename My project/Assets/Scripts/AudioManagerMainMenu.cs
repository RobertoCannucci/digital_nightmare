using UnityEngine;

public class AudioManagerMainMenu : MonoBehaviour
{
    [Header("music")]
    [SerializeField] AudioSource musicSource;

    public AudioClip background;

    public void Start(){
        musicSource.clip = background;
        musicSource.Play();
    }
}
