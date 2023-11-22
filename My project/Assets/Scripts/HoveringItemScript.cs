using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoveringItemScript : MonoBehaviour
{
    public GameObject hoveringIK;
    public bool isPickedUp = false;
    public bool isDropping = false;
    public Vector3 hoveringObjOriginalPos;
    public Vector3 hoveringObjOriginalForward;

    private void Start()
    {
        hoveringIK = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().hoveringIK;
    }
    private void FixedUpdate()
    {
        if (isPickedUp)
        {
            PickUpHovering();
        }
        else if (isDropping)
        {
            DropHovering();
        }
    }

    void PickUp()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        hoveringObjOriginalPos = transform.position;
        hoveringObjOriginalForward = transform.forward;
        transform.parent = hoveringIK.transform;
        isPickedUp = true;
        isDropping = false;
        Camera.main.GetComponent<CameraRotation>().lockedCamera = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().hoveringObj = gameObject;
    }

    void PutDown()
    {
        transform.parent = null;
        isPickedUp = false;
        isDropping = true;
        Camera.main.GetComponent<CameraRotation>().lockedCamera = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>().hoveringObj = null;
    }

    void PickUpHovering()
    {
        if (transform.position != hoveringIK.transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, hoveringIK.transform.position, Time.deltaTime * 15);
        }
        if (transform.forward != hoveringIK.transform.forward)
        {
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, hoveringIK.transform.forward, Time.deltaTime * 5, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    void DropHovering()
    {
        if (transform.position != hoveringObjOriginalPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, hoveringObjOriginalPos, Time.deltaTime * 15);
        }
        if (transform.forward != hoveringObjOriginalForward)
        {
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, hoveringObjOriginalForward, Time.deltaTime * 5, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
        if (transform.position == hoveringObjOriginalPos && transform.forward == hoveringObjOriginalForward)
        {
            isDropping = false;
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void PlayerInput()
    {
        if (!isPickedUp && !isDropping)
        {
            PickUp();
        }
        else if (isPickedUp && !isDropping)
        {
            PutDown();
        }
    }
}
