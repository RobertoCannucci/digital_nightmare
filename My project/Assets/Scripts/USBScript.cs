using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USBScript : MonoBehaviour
{
    public GameObject Monster;
    public Light USBLight;
    private float timer = 0f;
    private float interval = 1.5f;
    public bool pickedUp = false;

    void Start()
    {
        Monster = GameObject.FindGameObjectWithTag("Monster");
        USBLight = GameObject.FindGameObjectWithTag("USBLight").GetComponent<Light>();
    }

    void Update()
    {
        if (pickedUp)
        {
            float distanceFromMonster = (transform.position - Monster.transform.position).magnitude;

            if (distanceFromMonster < 5f)
            {
                USBLight.color = Color.red;
                interval = 0.3f;
            }
            else if (distanceFromMonster < 15f)
            {
                USBLight.color = Color.yellow;
                interval = 0.6f;
            }
            else
            {
                USBLight.color = Color.green;
                interval = 1f;
            }

            // https://gamedevbeginner.com/how-to-make-a-light-flicker-in-unity/
            timer += Time.deltaTime;
            if (timer > interval)
            {
                USBLight.enabled = !USBLight.enabled;
                timer -= interval;
            }
        }
    }
}
