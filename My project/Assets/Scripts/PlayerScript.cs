using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject leftHandIK;
    public GameObject leftHandObj;
    private Vector3 leftHandObjPos = new Vector3(0.005f, -0.149f, 0.48f);
    private Quaternion leftHandObjRot = Quaternion.Euler(new Vector3(115.79f, 0, -53.3f));
    public GameObject rightHandIK;
    public GameObject rightHandObj;
    private Vector3 rightHandObjPos = new Vector3(-0.038f, -0.201f, 0.388f);
    private Quaternion rightHandObjRot = Quaternion.Euler(new Vector3(76.619f, 193.493f, -118.233f));
    private Vector3 standingControllerCenter = new Vector3(0, 1.9f, 0);
    private float standingControllerHeight = 3.8f;
    private Vector3 crouchingControllerCenter = new Vector3(0, 1.5f, 0);
    private float crouchingControllerHeight = 2.8f;
    public Vector3 gravity;
    public Vector3 playerVelocity;
    public bool groundedPlayer;
    public float mouseSensitivy = 5.0f;
    private float gravityValue = -9.81f;
    private CharacterController controller;
    private float crouchSpeed = 2.5f;
    private float walkSpeed = 5;
    private float runSpeed = 8;
    private Animator animator;
    private Vector3 movement;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        UpdateRotation();
        ProcessMovement();
        ProcessInput();
    }
    public void LateUpdate()
    {
        UpdateAnimator();
    }
    void ProcessInput()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            // Pause Menu
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag.Contains("PickUp"))
                {
                    PickUpObject(hit.collider.gameObject);
                }
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (rightHandObj != null && rightHandObj.tag.Contains("FlashLight"))
            {
                ToggleFlashLight();
            }
        }
    }
    void PickUpObject(GameObject pickUp)
    {
        if (pickUp.tag.Contains("RightHand"))
        {
            rightHandObj = pickUp;
            rightHandObj.GetComponent<Rigidbody>().isKinematic = true;
            rightHandObj.transform.parent = rightHandIK.transform;
            rightHandObj.transform.localPosition = rightHandObjPos;
            rightHandObj.transform.localRotation = rightHandObjRot;

            if (pickUp.tag.Contains("FlashLight"))
            {
                ToggleFlashLight();
            }
        }
        if (pickUp.tag.Contains("LeftHand"))
        {
            leftHandObj = pickUp;
            leftHandObj.GetComponent<Rigidbody>().isKinematic = true;
            leftHandObj.transform.parent = leftHandIK.transform;
            leftHandObj.transform.localPosition = leftHandObjPos;
            leftHandObj.transform.localRotation = leftHandObjRot;
        }
    }
    void ToggleFlashLight()
    {
        Light LightComponent = rightHandObj.transform.GetChild(0).GetComponent<Light>();
        if (LightComponent.intensity == 0)
        {
            LightComponent.intensity = 2.7f;
        }
        else
        {
            LightComponent.intensity = 0;
        }
    }
    void UpdateRotation()
    {
        transform.Rotate(0, Input.GetAxis("Mouse X") * mouseSensitivy, 0, Space.Self);
    }
    void ProcessMovement()
    {
        // Moving the character forward according to the speed
        float speed = GetMovementSpeed();

        // Get the camera's forward and right vectors
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Make sure to flatten the vectors so that they don't contain any vertical component
        cameraForward.y = 0;
        cameraRight.y = 0;

        // Normalize the vectors to ensure consistent speed in all directions
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate the movement direction based on input and camera orientation
        Vector3 moveDirection = (cameraForward * Input.GetAxis("Vertical")) + (cameraRight * Input.GetAxis("Horizontal"));

        // Apply the movement direction and speed
        movement = moveDirection.normalized * speed * Time.deltaTime;

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer)
        {
            gravity.y = -1.0f;
        }
        else
        {
            gravity.y += gravityValue * Time.deltaTime;
        }
        playerVelocity = gravity * Time.deltaTime + movement;
        controller.Move(playerVelocity);
    }

    float GetMovementSpeed()
    {
        if (Input.GetButton("Fire3"))// Left shift
        {
            return runSpeed;
        }
        else if (Input.GetKey("left ctrl"))
        {
            return crouchSpeed;
        }
        else
        {
            return walkSpeed;
        }
    }

    void UpdateAnimator()
    {
        if (movement != Vector3.zero)
        {
            if (GetMovementSpeed() == walkSpeed)
            {
                animator.SetFloat("Speed", 0.5f);
            }
            else
            {
                animator.SetFloat("Speed", 1.0f);
            }
        }
        else
        {
            animator.SetFloat("Speed", 0.0f);
        }

        if (Input.GetKey("left ctrl"))
        {
            animator.SetBool("Crouching", true);
            controller.center = crouchingControllerCenter;
            controller.height = crouchingControllerHeight;
        }
        else
        {
            animator.SetBool("Crouching", false);
            controller.center = standingControllerCenter;
            controller.height = standingControllerHeight;
        }

    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {

            // Set the look target position, if one has been assigned
            //if (lookObj != null)
            //{
            //    animator.SetLookAtWeight(1);
            //    animator.SetLookAtPosition(Camera.main.transform.position + Camera.main.transform.forward);
            //}

            // Set the right hand target position and rotation, if one has been assigned
            if (leftHandObj != null)
            {
                animator.SetBool("LeftHandHolding", true);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIK.transform.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIK.transform.rotation);
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetLookAtWeight(0);
            }

            if (rightHandObj != null)
            {
                animator.SetBool("RightHandHolding", true);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIK.transform.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandIK.transform.rotation);
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            }



        }
    }
}
