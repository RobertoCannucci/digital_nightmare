using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public GameObject door;
    private float openRotation;
    private float closeRotation;
    private float speed = 5;
    public bool opening;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 currentRotation = door.transform.localEulerAngles;
        closeRotation = currentRotation.y;
        openRotation = currentRotation.y + 90;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentRotation = door.transform.localEulerAngles;
        if(opening) {
            if(currentRotation.y < openRotation) {
                door.transform.localEulerAngles = Vector3.Lerp(currentRotation, new Vector3(currentRotation.x, openRotation, currentRotation.z), speed * Time.deltaTime);
            }
        }
        else {
            if(currentRotation.y > closeRotation) {
                door.transform.localEulerAngles = Vector3.Lerp(currentRotation, new Vector3(currentRotation.x, closeRotation, currentRotation.z), speed * Time.deltaTime);
            }
        }
    }
}
