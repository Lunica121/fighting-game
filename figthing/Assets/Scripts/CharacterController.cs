using UnityEngine;
using System.Collections;

public class CharacterController: MonoBehaviour
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
    private const string PLAYER_LOW_KICK = "L KICK";
    private const string PLAYER_LOW_PUNCH = "L PUNCH";
    private const string PLAYER_HIGH_KICK = "H KICK";
    private const string PLAYER_HIGH_PUNCH = "H PUNCH";
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

        isJumpPressed = Input.GetKey(KeyCode.W);

        if (Input.GetKeyDown(KeyCode.C))
            isBlocking = true;

        if (Input.GetKeyUp(KeyCode.C))
            isBlocking = false;

        if (Input.GetKeyDown(KeyCode.U))
            Attack(PLAYER_HIGH_PUNCH);          // U for high punch

        if (Input.GetKeyDown(KeyCode.I))
            Attack(PLAYER_LOW_PUNCH);           // I for low punch

        if (Input.GetKeyDown(KeyCode.J))
            Attack(PLAYER_HIGH_KICK);           // J for high kick

        if (Input.GetKeyDown(KeyCode.K))
            Attack(PLAYER_LOW_KICK);            // K for low kick
    }


    private void FixedUpdate()
    {
        // Ground check
        isHit = false;
        RaycastHit hit;
        isHit = Physics.CheckSphere(transform.position, 0.5f, groundLayerMask);

        // Movement
        float zAxis = Input.GetAxisRaw("Horizontal"); // Change to Horizontal for A and D keys
        Vector3 moveVelocity = new Vector3(0f, rb.velocity.y, -zAxis * walkSpeed); // Adjust to z-axis movement

        if (zAxis < 0)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        else if (zAxis > 0)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        // Jumping
        if (isJumpPressed && isHit)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            //isJumpPressed = false;
        }

        // Animation handling
        if (isHit)
        {
            ChangeAnimationState(PLAYER_IDLE);
        }
        else
        {
            if (zAxis != 0)
                ChangeAnimationState(PLAYER_WALK);
            else
                ChangeAnimationState(PLAYER_IDLE);
        }

        rb.velocity = moveVelocity;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
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
