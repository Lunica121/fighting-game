using System.Collections;
using UnityEngine;

public class CharacterController3D : MonoBehaviour
{
    [SerializeField] private LayerMask platformLayerMask;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float jumpForce = 300f;

    private Animator animator;
    private Rigidbody rb;
    private bool isJumpPressed;
    private bool isGrounded;
    private bool isCrouching;
    private string currentAnimation;

    // Animation States
    private const string PLAYER_IDLE = "IDLE";
    private const string PLAYER_WALK = "WALK";
    private const string PLAYER_JUMP = "JUMP";
    private const string PLAYER_KICK = "LOW KICK";
    private const string PLAYER_PUNCH = "LOW PUNCH";
    private const string PLAYER_KO = "DEATH";
    private const string PLAYER_HIT = "HIT";
    //private const string PLAYER_BLOCK = "RYU_Block";
    private const string PLAYER_CROUCH = "CROUCH";

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Input
        float xAxis = Input.GetAxisRaw("Horizontal");
        isJumpPressed = Input.GetKeyDown(KeyCode.A);
        isCrouching = Input.GetKey(KeyCode.D);

        // Movement
        
        Vector3 moveVelocity = new Vector3(0, rb.velocity.y, -xAxis * walkSpeed);

        // Jump
        if (isJumpPressed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Crouch
        if (isCrouching)
        {
            ChangeAnimationState(PLAYER_CROUCH);
        }
        else if (isGrounded)
        {
            // Check for movement
            if (xAxis != 0f)
            {
                ChangeAnimationState(PLAYER_WALK);
            }
            else
            {
                ChangeAnimationState(PLAYER_IDLE);
            }
        }
        else
        {
            ChangeAnimationState(PLAYER_JUMP);
        }

        rb.velocity = moveVelocity;
    }

    private void FixedUpdate()
    {
        // Check if player touches ground
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, platformLayerMask);
    }

    // Change animation state
    private void ChangeAnimationState(string newAnimation)
    {
        if (currentAnimation == newAnimation) return;
        animator.Play(newAnimation);
        currentAnimation = newAnimation;
    }
}
