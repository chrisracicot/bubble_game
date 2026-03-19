using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 7f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private bool isGrounded;
    private float moveInput;

    private bool isInBubble = false;
    private Transform currentBubble;
    private float bubbleCooldown = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent the manual cube from rolling over!
        
        // Auto-assign groundCheck if it's not set in the inspector
        if (groundCheck == null)
        {
            groundCheck = transform.Find("GroundCheck");
            if (groundCheck == null)
            {
                // Create one if it doesn't exist
                GameObject gc = new GameObject("GroundCheck");
                gc.transform.SetParent(this.transform);
                gc.transform.localPosition = new Vector3(0f, -0.6f, 0f);
                groundCheck = gc.transform;
            }
        }

        // Auto-assign layer if not set
        if (groundLayer == 0)
        {
            groundLayer = LayerMask.GetMask("Default");
            Debug.Log("[PlayerController] Auto-assigned groundLayer to 'Default'");
        }
    }

    private void Update()
    {
        if (bubbleCooldown > 0f) bubbleCooldown -= Time.deltaTime;
        
        // Use keyboard horizontal if it's active, otherwise use mobile input
        float keyboardHorizontal = Input.GetAxisRaw("Horizontal");
        moveInput = Mathf.Abs(keyboardHorizontal) > 0.1f ? keyboardHorizontal : MobileInput.Horizontal;

        // Check for jump from either mobile button or Space bar
        bool jumpRequested = MobileInput.JumpPressed || Input.GetKeyDown(KeyCode.Space);

        if (jumpRequested && (isGrounded || isInBubble))
        {
            if (isInBubble)
            {
                ExitBubble();
                bubbleCooldown = 0.5f; // Prevent immediately re-entering the same bubble
            }
            Jump();
            MobileInput.ConsumeJump();
        }
    }

    private void FixedUpdate()
    {
        CheckGround();

        if (isInBubble)
        {
            if (currentBubble != null)
                transform.position = currentBubble.position;
            
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveInput * moveSpeed;
        rb.linearVelocity = velocity;
    }

    private void Jump()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public void EnterBubble(Transform bubbleCenter)
    {
        if (bubbleCooldown > 0f || isInBubble) return;

        isInBubble = true;
        currentBubble = bubbleCenter;
        
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        transform.position = bubbleCenter.position;
    }

    public void ExitBubble()
    {
        isInBubble = false;
        currentBubble = null;
        rb.useGravity = true;
    }
}