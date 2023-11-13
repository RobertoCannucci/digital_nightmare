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

    Vector2 rotation = Vector2.zero;
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
            transform.position = playerAnimator.GetBoneTransform(HumanBodyBones.Head).position;
            //rotation.x += Input.GetAxis(xAxis) * sensitivity;
            rotation.y += Input.GetAxis(yAxis) * sensitivity;
            rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, 90);
            var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
            var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

            transform.localRotation = xQuat * yQuat; //Quaternions seem to rotate more consistently than EulerAngles. Sensitivity seemed to change slightly at certain degrees using Euler. transform.localEulerAngles = new Vector3(-rotation.y, rotation.x, 0);
        }
    }
}
