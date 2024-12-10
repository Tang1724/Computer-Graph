using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private CharacterController cc;
    public float moveSpeed;
    public float jumpSpeed;

    private Vector3 moveDirection;
    private Vector3 velocity;
    public float gravity;

    public Transform groundCheck;
    public float checkRadius;
    public LayerMask groundLayer;
    public bool isGrounded;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        CheckGroundStatus();
        ProcessInput();
        ApplyGravity();
        Move();
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, checkRadius, groundLayer);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Reset the downward velocity when grounded
        }
    }

    private void ProcessInput()
    {
        float horizontalMove = Input.GetAxis("Horizontal") * moveSpeed;
        float verticalMove = Input.GetAxis("Vertical") * moveSpeed;
        moveDirection = transform.forward * verticalMove + transform.right * horizontalMove;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = jumpSpeed;
        }
    }

    private void ApplyGravity()
    {
        velocity.y -= gravity * Time.deltaTime;
    }

    private void Move()
    {
        cc.Move(moveDirection * Time.deltaTime); // Handle horizontal movement
        cc.Move(velocity * Time.deltaTime); // Apply vertical velocity
    }
}
