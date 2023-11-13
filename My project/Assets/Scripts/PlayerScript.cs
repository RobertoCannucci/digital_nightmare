using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public bool isGamePaused = false;

    public List<string> collectedNotes;

    public Text displayTextUI;
    public Canvas pauseMenu;

    private float pickUpRange = 3.75f;

    public GameObject leftHandIK;
    public GameObject leftHandObj;
    public Vector3 leftHandObjPos = new Vector3(-0.349f, -0.03f, 0.2f);
    //private Vector3 leftHandObjPos = new Vector3(0.005f, -0.149f, 0.48f);
    public Quaternion leftHandObjRot = Quaternion.Euler(new Vector3(115.79f, 0, -53.3f));

    public List<GameObject> rightHandInventory = new List<GameObject>();
    public int rightHandIdx = 0;
    public GameObject rightHandIK;
    public GameObject rightHandObj;
    public Vector3 rightHandObjPos = new Vector3(-0.038f, -0.201f, 0.388f);
    public Quaternion rightHandObjRot = Quaternion.Euler(new Vector3(76.619f, 193.493f, -118.233f));

    public GameObject hoveringIK;
    public GameObject hoveringObj;

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
        rightHandInventory.Add(null);
    }

    void Update()
    {
        Cursor.lockState = CursorLockMode.Confined;
        ProcessInput();
        if (!isGamePaused && hoveringObj == null)
        {
            ProcessMovement();
            UpdateRotation();
            Cursor.visible = false;
        }
    }
    public void LateUpdate()
    {
        if (hoveringObj == null)
        {
            UpdateAnimator();
        }
    }
    public void TogglePauseGame()
    {
        if (isGamePaused)
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            pauseMenu.gameObject.SetActive(false);
            Camera.main.GetComponent<CameraRotation>().lockedCamera = false;
            Cursor.visible = false;
        }
        else
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
            pauseMenu.gameObject.SetActive(true);
            Camera.main.GetComponent<CameraRotation>().lockedCamera = true;
            Cursor.visible = true;
        }
        isGamePaused = !isGamePaused;
    }
    void ProcessInput()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            TogglePauseGame();
        }
        else if (!isGamePaused)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag.Contains("PickUp"))
                    {
                        Vector3 playerPosToObjectPos = hit.transform.position - transform.position;
                        if (playerPosToObjectPos.magnitude <= pickUpRange)
                        {
                            hit.collider.gameObject.GetComponent<PickUpItemScript>().PickUpObject();
                        }
                        else
                        {
                            StartCoroutine(DisplayText("Object is too far"));
                        }
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                CycleRightHand();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                DetachItemFromHand(leftHandObj);
                leftHandObj = null;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ThrowLeftHand();
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (rightHandObj != null && rightHandObj.tag.Contains("FlashLight"))
                {
                    ToggleFlashLight();
                }
            }
        }
    }
    // https://forum.unity.com/threads/fading-in-out-gui-text-with-c-solved.380822/
    IEnumerator DisplayText(string textToDisplay)
    {
        displayTextUI.text = textToDisplay;
        displayTextUI.color = new Color(displayTextUI.color.r, displayTextUI.color.g, displayTextUI.color.b, 1);
        System.Threading.Thread.Sleep(200);
        while (displayTextUI.color.a > 0.0f)
        {
            displayTextUI.color = new Color(displayTextUI.color.r, displayTextUI.color.g, displayTextUI.color.b, displayTextUI.color.a - (Time.deltaTime / 2.5f));
            yield return null;
        }
    }
    void ThrowLeftHand()
    {
        if (leftHandObj != null)
        {
            if (leftHandObj.tag.Contains("MetalBottle"))
            {
                leftHandObj.GetComponent<MetalBottleScript>().canPlaySound = true;
            }
            GameObject throwingObject = leftHandObj;
            DetachItemFromHand(leftHandObj);
            leftHandObj = null;
            throwingObject.transform.position = hoveringIK.transform.position;
            throwingObject.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 10, ForceMode.VelocityChange);
        }
    }
    void DetachItemFromHand(GameObject obj)
    {
        if (obj != null)
        {
            obj.GetComponent<Rigidbody>().isKinematic = false;
            obj.transform.parent = null;
        }
    }
    public void CycleRightHand()
    {
        rightHandIdx++;
        if (rightHandIdx >= rightHandInventory.Count)
        {
            rightHandIdx = 0;
        }
        if (rightHandObj != null)
        {
            rightHandObj.SetActive(false);
        }
        rightHandObj = rightHandInventory[rightHandIdx];
        if (rightHandInventory[rightHandIdx] != null)
        {
            rightHandObj.SetActive(true);
            AttachItemToHand(rightHandObj, rightHandIK, rightHandObjPos, rightHandObjRot);
        }
    }
    public void AttachItemToHand(GameObject obj, GameObject handIK, Vector3 handObjPos, Quaternion handObjRot)
    {
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.transform.parent = handIK.transform;
        obj.transform.localPosition = handObjPos;
        obj.transform.localRotation = handObjRot;
    }
    public void ToggleFlashLight()
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
                animator.SetBool("LeftHandHolding", false);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
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
                animator.SetBool("RightHandHolding", false);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            }
        }
    }
}
