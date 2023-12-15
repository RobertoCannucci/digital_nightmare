using UnityEngine;

public class AudioManagerMainMenu : MonoBehaviour
{
    [Header("music")]
    [SerializeField] public AudioSource musicSource;

    public AudioClip background;

    public void Start(){
        //musicSource.clip = background;
        //musicSource.Play();
    }
}
