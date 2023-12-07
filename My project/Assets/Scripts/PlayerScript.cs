using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript Instance { get; private set; }
    public List<string> collectedNotes;

    private float pickUpRange = 3.75f;

    public int BatteryInventory = 0;
    private bool Flashbanging = false;
    public bool UVMode = false;
    public bool UVUnlocked = false;

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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        rightHandInventory.Add(null);
 
    }

    void Update()
    {
        Cursor.lockState = CursorLockMode.Confined;
        ProcessInput();
        if (!GameManager.Instance.isGamePaused && hoveringObj == null)
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
    void ProcessInput()
    {
        if (GameManager.Instance == null)
        {
            return;
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            GameManager.Instance.TogglePauseGame();
        }
        else if (!GameManager.Instance.isGamePaused)
        {
            if (Input.GetKeyDown(KeyCode.F) && !(Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E)))
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
                            StartCoroutine(GameManager.Instance.DisplayText("Object is too far"));
                        }
                    }
                    if (hit.collider.tag == "door"){
                        if (hit.collider.gameObject.GetComponent<DoorScript>().opening == false){
                            hit.collider.gameObject.GetComponent<DoorScript>().opening = true;
                        }else{
                            hit.collider.gameObject.GetComponent<DoorScript>().opening = false;
                        }
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.C) && UVUnlocked && rightHandObj.tag.Contains("FlashLight"))
            {
                UVMode = !UVMode;
                Light LightComponent = null;
                if (UVMode)
                {
                    rightHandObj.transform.GetChild(0).gameObject.SetActive(false);
                    rightHandObj.transform.GetChild(2).gameObject.SetActive(true);
                    LightComponent = rightHandObj.transform.GetChild(2).GetComponent<Light>();
                }
                else
                {
                    rightHandObj.transform.GetChild(0).gameObject.SetActive(true);
                    rightHandObj.transform.GetChild(2).gameObject.SetActive(false);
                    LightComponent = rightHandObj.transform.GetChild(0).GetComponent<Light>();
                }
                if (LightComponent.intensity == 0)
                {
                    ToggleFlashLight();
                }
            }
            if (Input.GetKeyDown(KeyCode.G) && !Flashbanging)
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
            if (Input.GetKeyDown(KeyCode.Mouse1) && !Flashbanging)
            {
                if (rightHandObj != null && rightHandObj.tag.Contains("FlashLight"))
                {
                    if (BatteryInventory > 0)
                    {
                        Flashbang();
                    }
                    else
                    {
                        StartCoroutine(GameManager.Instance.DisplayText("No batteries in inventory for flashbang"));
                    }
                }
                else
                {
                    StartCoroutine(GameManager.Instance.DisplayText("Not holding flashlight for flashbang"));
                }
            }
            if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.E))
            {
                Camera.main.GetComponent<CameraRotation>().leaningLeft = false;
                Camera.main.GetComponent<CameraRotation>().leaningRight = false;
                leftHandIK.SetActive(true);
                rightHandIK.SetActive(true);
            }
            else if (Input.GetKey(KeyCode.Q) && hoveringObj == null)
            {
                Camera.main.GetComponent<CameraRotation>().leaningLeft = true;
                Camera.main.GetComponent<CameraRotation>().leaningRight = false;
                leftHandIK.SetActive(false);
                rightHandIK.SetActive(false);
            }
            else if (Input.GetKey(KeyCode.E) && hoveringObj == null)
            {
                Camera.main.GetComponent<CameraRotation>().leaningLeft = false;
                Camera.main.GetComponent<CameraRotation>().leaningRight = true;
                leftHandIK.SetActive(false);
                rightHandIK.SetActive(false);
            }
            else
            {
                Camera.main.GetComponent<CameraRotation>().leaningLeft = false;
                Camera.main.GetComponent<CameraRotation>().leaningRight = false;
                leftHandIK.SetActive(true);
                rightHandIK.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (rightHandObj != null && rightHandObj.tag.Contains("FlashLight") && !Flashbanging)
                {
                    ToggleFlashLight();
                }
            }
        }
    }
    void Flashbang()
    {
        GameManager.Instance.RemoveBattery();
        rightHandObj.transform.GetChild(0).GetComponent<Light>().intensity = 0;
        rightHandObj.transform.GetChild(1).GetComponent<Light>().intensity = 4.5f;
        Flashbanging = true;
        if (UVUnlocked)
        {
            UVMode = false;
            rightHandObj.transform.GetChild(2).gameObject.SetActive(false);
        }

        GameObject monster = GameObject.FindGameObjectWithTag("Monster");
        // 1) Find Vector from monster to player
        Vector3 monsterDirection = monster.transform.position - transform.position;
        // 2) If monster is in range
        if (monsterDirection.magnitude < 55)
        {
            // 3) Getting angle between monster and player

            Vector3 normMonsterDirection = Vector3.Normalize(monsterDirection);
            // (can only find angle with normalized vectors)
            float dotProduct = Vector3.Dot(transform.forward, normMonsterDirection);
            float angle = Mathf.Acos(dotProduct);
            float degreeAngle = angle * Mathf.Rad2Deg;
            // If player is within monster's angle of sight
            if (degreeAngle < 45)
            {
                // Create ray from monster to player
                Ray ray = new Ray(transform.position, normMonsterDirection);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // If ray hits player
                    if (hit.collider.tag == "Monster")
                    {
                        // hit.GetComponent<MonsterScript>().GetStunned();
                        Debug.Log("stunned monster");
                    }
                }
            }
        }

        StartCoroutine(TurnFlashbangOffSlow());
        StartCoroutine(TurnRegularLightOnSlow());

    }
    public IEnumerator TurnFlashbangOffSlow()
    {
        Light flashbangLight = rightHandObj.transform.GetChild(1).GetComponent<Light>();
        System.Threading.Thread.Sleep(750);
        while (flashbangLight.intensity > 0.0f)
        {
            flashbangLight.intensity -= Time.deltaTime / (3 / 4.5f);
            yield return null;
        }
    }
    public IEnumerator TurnRegularLightOnSlow()
    {
        Light regularLight = rightHandObj.transform.GetChild(0).GetComponent<Light>();
        System.Threading.Thread.Sleep(750);
        while (regularLight.intensity <= 2.7f)
        {
            regularLight.intensity += Time.deltaTime / (3 / 2.7f);
            yield return null;
        }
        Flashbanging = false;
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
        Light LightComponent = null;
        if (UVMode)
        {
            LightComponent = rightHandObj.transform.GetChild(2).GetComponent<Light>();
        }
        else
        {
            LightComponent = rightHandObj.transform.GetChild(0).GetComponent<Light>();
        }
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