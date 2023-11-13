using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalBottleScript : MonoBehaviour
{
    public AudioClip collisionAudio;
    public bool canPlaySound = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (canPlaySound)
        {
            AudioSource.PlayClipAtPoint(collisionAudio, collision.transform.position);
            canPlaySound = false;
        }
    }
}
