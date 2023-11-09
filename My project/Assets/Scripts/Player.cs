using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Vector3 playerVelocity;
    Vector3 move;
    public float speed;
    public float walkSpeed = 5;
    public float runSpeed = 8; 
    public float gravity = -9.18f;
    
    private CharacterController controller;
    private Animator animator;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }
    public void Update()
    {
        ProcessMovement();
    }

    public void LateUpdate()
    {
        UpdateAnimator();
    }

    // Update is called once per frame
    void UpdateAnimator()
    {
        if (move != Vector3.zero)
        {
            animator.SetFloat("Speed", speed);
        }
        else
        {
            animator.SetFloat("Speed", speed);
        }
    }
    void ProcessMovement()
    {
        move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }
    }
}
