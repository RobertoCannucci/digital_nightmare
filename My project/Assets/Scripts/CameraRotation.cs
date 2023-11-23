using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float Sensitivity
    {
        get { return sensitivity; }
        set { sensitivity = value; }
    }
    private Animator playerAnimator;
    float sensitivity = 2f;
    float yRotationLimit = 52.8f;
    public bool lockedCamera = false;
    public bool leaningLeft = false;
    public bool leaningRight = false;

    Vector3 rotation = Vector3.zero;
    const string xAxis = "Mouse X"; //Strings in direct code generate garbage, storing and re-using them creates no garbage
    const string yAxis = "Mouse Y";

    private void Start()
    {
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }
    void Update()
    {
        if (!lockedCamera)
        {
            float cameraRotZOffset = 0f;
            float cameraPosXOffset = 0f;
            transform.position = playerAnimator.GetBoneTransform(HumanBodyBones.Head).position;
            if (leaningLeft)
            {
                cameraRotZOffset = 18.9f;
                cameraPosXOffset = -2.1f;
            }
            if (leaningRight)
            {
                cameraPosXOffset = 2.1f;
                cameraRotZOffset = -18.9f;
            }
            transform.localPosition = new Vector3(transform.localPosition.x + cameraPosXOffset, transform.localPosition.y, transform.localPosition.z);
            //rotation.x += Input.GetAxis(xAxis) * sensitivity;
            rotation.y += Input.GetAxis(yAxis) * sensitivity;
            rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, 90);
            var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
            var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);
            var zQuat = Quaternion.AngleAxis(rotation.z + cameraRotZOffset, Vector3.forward);

            transform.localRotation = xQuat * yQuat * zQuat;
        }
    }
}
