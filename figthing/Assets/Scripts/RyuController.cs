using UnityEngine;
using System.Collections;

public class RyuController : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float jumpForce = 300f;
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private GameObject hitPrefab;

    private Animator animator;
    private Rigidbody rb;
    private bool isJumpPressed;
    private bool isAttacking;
    private bool isBlocking;
    private bool isHit;
    private string currentAnimation;

    // Animation States
    private const string PLAYER_IDLE = "IDLE";
    private const string PLAYER_WALK = "WALK";
    private const string PLAYER_JUMP = "JUMP";
    private const string PLAYER_LOW_KICK = "LOW KICK";
    private const string PLAYER_LOW_PUNCH = "LOW PUNCH";
    private const string PLAYER_HIGH_KICK = "HIGH KICK";
    private const string PLAYER_HIGH_PUNCH = "HIGH PUNCH";
    private const string PLAYER_KO = "DEATH";
    private const string PLAYER_HIT = "HIT";
    //private const string PLAYER_BLOCK = "RYU_Block";

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Input
        float xAxis = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
            isJumpPressed = true;

        if (Input.GetKeyDown(KeyCode.C))
            isBlocking = true;

        if (Input.GetKeyUp(KeyCode.C))
            isBlocking = false;

        if (Input.GetKeyDown(KeyCode.U))
            Attack(PLAYER_LOW_PUNCH);

        if (Input.GetKeyDown(KeyCode.J))
            Attack(PLAYER_LOW_KICK);
    }

    private void FixedUpdate()
    {
        // Ground check
        isHit = false;
        RaycastHit hit;
        isHit = Physics.Raycast(transform.position, Vector3.down, out hit, 0.1f, groundLayerMask);

        // Movement
        float xAxis = Input.GetAxisRaw("Horizontal");
        Vector3 moveVelocity = new Vector3(xAxis * walkSpeed, rb.velocity.y, 0f);

        if (xAxis < 0)
            transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        else if (xAxis > 0)
            transform.rotation = Quaternion.Euler(0f, 90f, 0f);

        // Jumping
        if (isJumpPressed && isHit)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumpPressed = false;
        }

        // Animation handling
        if (isHit)
        {
            ChangeAnimationState(PLAYER_IDLE);
        }
        else
        {
            if (xAxis != 0)
                ChangeAnimationState(PLAYER_WALK);
            else
                ChangeAnimationState(PLAYER_IDLE);
        }

        rb.velocity = moveVelocity;
    }

    private void Attack(string attackAnimation)
    {
        if (!isAttacking)
        {
            isAttacking = true;
            ChangeAnimationState(attackAnimation);
            Invoke(nameof(StopAttacking), attackDelay);
        }
    }

    private void StopAttacking()
    {
        isAttacking = false;
    }

    private void ChangeAnimationState(string newAnimation)
    {
        if (currentAnimation == newAnimation) return;

        animator.Play(newAnimation);
        currentAnimation = newAnimation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            isHit = true;
            StartCoroutine(PlayerHurt());
        }
    }

    private IEnumerator PlayerHurt()
    {
        ChangeAnimationState(PLAYER_HIT);
        yield return new WaitForSeconds(0.3f);
        ChangeAnimationState(PLAYER_IDLE);
        isHit = false;
    }
}
