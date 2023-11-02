using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
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
        //UpdateRotation();
        ProcessMovement();
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            // Pause Menu
        }
    }
    public void LateUpdate()
    {
        UpdateAnimator();
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
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            transform.forward = moveDirection;
        }
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
        else if (Input.GetKeyDown("left ctrl"))// Left shift
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
        bool isGrounded = controller.isGrounded;

        if (movement != Vector3.zero)
        {
            if (GetMovementSpeed() == crouchSpeed)
            {
                animator.SetBool("Crouching", true);
                animator.SetFloat("Speed", 1.0f);
            }
            else if (GetMovementSpeed() == runSpeed)
            {
                animator.SetFloat("Speed", 1.0f);
            }
            else
            {
                animator.SetFloat("Speed", 0.5f);
            }
        }
        else
        {
            animator.SetFloat("Speed", 0.0f);
        }
        animator.SetBool("IsGrounded", isGrounded);

    }
}
