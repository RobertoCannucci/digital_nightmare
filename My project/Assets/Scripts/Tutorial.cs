using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && gameObject.tag == "WalkingTutorial") {
            StartCoroutine(GameManager.Instance.DisplayText("Use WASD to move around"));
        }

        if(other.tag == "Player" && gameObject.tag == "LeaningTutorial") {
            StartCoroutine(GameManager.Instance.DisplayText("Use Q or E to lean"));
        }

        if(other.tag == "Player" && gameObject.tag == "RunningTutorial") {
            StartCoroutine(GameManager.Instance.DisplayText("Use shift to run"));
        }

        if(other.tag == "Player" && gameObject.tag == "BottleTutorial") {
            StartCoroutine(GameManager.Instance.DisplayText("Use F to pick up items"));
        }

        if(other.tag == "Player" && gameObject.tag == "ThrowTutorial") {
            StartCoroutine(GameManager.Instance.DisplayText("Use Space to throw items"));
        }

        if(other.tag == "Player" && gameObject.tag == "DropTutorial") {
            StartCoroutine(GameManager.Instance.DisplayText("Use R to drop items"));
        }

        if(other.tag == "Player" && gameObject.tag == "DoorTutorial") {
            StartCoroutine(GameManager.Instance.DisplayText("Use F to open doors"));
        }

        if(other.tag == "Player" && gameObject.tag == "EndLevel") {
            GameManager.Instance.NextLevel();
            //PlayerScript.Instance.transform.position = new Vector3(-0.15f, 1.87f, 3.29f);
        }
    }
}