using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 8f;
    public float groundAccel = 80f;
    public float groundDecel = 90f;
    public float airAccel = 45f;
    public float airDecel = 35f;

    [Header("Jump")]
    public float jumpForce = 14f;

    [Tooltip("Seconds after leaving ground you can still jump")]
    public float coyoteTime = 0.10f;

    [Tooltip("Seconds before landing that jump input is buffered")]
    public float jumpBufferTime = 0.10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Heavier Jump Feel")]
    public float fallGravityMultiplier = 2.3f;     // stronger fall
    public float lowJumpGravityMultiplier = 1.7f;  // if you release jump early
    public float maxFallSpeed = 25f;

    private Rigidbody2D rb;

    private float moveInput;
    private float lastGroundedTime;   // for coyote time
    private float lastJumpPressedTime; // for jump buffer

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded) lastGroundedTime = coyoteTime;
        else lastGroundedTime -= Time.deltaTime;

        if (Input.GetButtonDown("Jump")) lastJumpPressedTime = jumpBufferTime;
        else lastJumpPressedTime -= Time.deltaTime;

        // Jump if: jump was pressed recently AND we are grounded (or within coyote time)
        if (lastJumpPressedTime > 0f && lastGroundedTime > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            lastJumpPressedTime = 0f;
            lastGroundedTime = 0f; // prevents double-jump via coyote
        }

        // Heavier fall + variable jump height
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallGravityMultiplier - 1f) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpGravityMultiplier - 1f) * Time.deltaTime;
        }

        // Clamp fall speed
        if (rb.linearVelocity.y < -maxFallSpeed)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
    }

    void FixedUpdate()
    {
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        float targetSpeed = moveInput * maxSpeed;

        // Accel/Decel are now "units of speed per second"
        float accelRate;
        if (Mathf.Abs(targetSpeed) > 0.01f)
            accelRate = isGrounded ? groundAccel : airAccel;
        else
            accelRate = isGrounded ? groundDecel : airDecel;

        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accelRate * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
    }


    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
