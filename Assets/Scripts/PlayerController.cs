using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f; // Kept for keyboard override steering
    
    [Header("Trajectory Jump Settings")]
    public float baseJumpForce = 7f;
    public float baseForwardForce = 5f;
    public float maxSideForce = 5f;

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
        rb.constraints = RigidbodyConstraints.FreezeRotation; 
        
        if (groundCheck == null)
        {
            groundCheck = transform.Find("GroundCheck");
            if (groundCheck == null)
            {
                GameObject gc = new GameObject("GroundCheck");
                gc.transform.SetParent(this.transform);
                gc.transform.localPosition = new Vector3(0f, -0.6f, 0f);
                groundCheck = gc.transform;
            }
        }

        if (groundLayer == 0)
        {
            groundLayer = LayerMask.GetMask("Default");
            Debug.Log("[PlayerController] Auto-assigned groundLayer to 'Default'");
        }
    }

    private void Update()
    {
        if (bubbleCooldown > 0f) bubbleCooldown -= Time.deltaTime;
        
        bool wantsJump = false;
        Vector2 jumpInput = Vector2.zero;

        // Check Control Panel Input
        if (MobileInput.TrajectoryJumpRequested)
        {
            wantsJump = true;
            jumpInput = MobileInput.TrajectoryInput;
            MobileInput.ConsumeTrajectoryJump();
        }

        // Keyboard Fallback for testing on PC
        try 
        {
            moveInput = Input.GetAxisRaw("Horizontal"); 
            
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                wantsJump = true;
                jumpInput = Vector2.zero; // default center jump
                
                if (Input.GetKey(KeyCode.LeftArrow)) jumpInput.x = -1f;
                if (Input.GetKey(KeyCode.RightArrow)) jumpInput.x = 1f;
                
                // UpArrow = center/forward jump
                // Space = higher jump
                // DownArrow = longer/lower jump
                if (Input.GetKey(KeyCode.Space)) jumpInput.y = 1f; 
                else if (Input.GetKey(KeyCode.DownArrow)) jumpInput.y = -1f;
                else jumpInput.y = 0f;
            }
        } 
        catch { }

        if (wantsJump && (isGrounded || isInBubble))
        {
            if (isInBubble)
            {
                ExitBubble();
                bubbleCooldown = 0.5f; 
            }
            
            ExecuteTrajectoryJump(jumpInput);
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
        
        // Handle horizontal movement/friction
        if (isGrounded && rb.linearVelocity.y <= 0.1f) // Only apply friction if not actively jumping upward
        {
            if (Mathf.Abs(moveInput) > 0.01f)
            {
                // Keyboard steering overrides friction
                velocity.x = moveInput * moveSpeed;
            }
            else
            {
                // Ground friction to prevent sliding forever after landing a side-jump
                velocity.x = Mathf.MoveTowards(velocity.x, 0f, 50f * Time.fixedDeltaTime);
            }
        }
        else
        {
            // Air steering via keyboard
            if (Mathf.Abs(moveInput) > 0.01f)
            {
                velocity.x = moveInput * moveSpeed;
            }
            // If no keyboard input, retain momentum (don't force velocity.x to 0)
        }

        rb.linearVelocity = velocity;
    }

    private void ExecuteTrajectoryJump(Vector2 input)
    {
        // Reset velocities for a consistent jump arc
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f; 
        velocity.x = 0f; 
        rb.linearVelocity = velocity;

        // X determines side force (-1 to 1)
        float lateralForce = input.x * maxSideForce;
        
        // Y determines balance between Up and Forward (-1 to 1)
        float normalizedY = (input.y + 1f) / 2f; // Converts -1..1 to 0..1
        
        // normalizedY = 1 (Top): Max Up, Min Forward
        // normalizedY = 0.5 (Center): Base Up, Base Forward
        // normalizedY = 0 (Bottom): Min Up, Max Forward
        float upForce = Mathf.Lerp(baseJumpForce * 0.5f, baseJumpForce * 1.5f, normalizedY);
        float fwdForce = Mathf.Lerp(baseForwardForce * 1.5f, baseForwardForce * 0.5f, normalizedY);
        
        Vector3 force = new Vector3(lateralForce, upForce, fwdForce);
        rb.AddForce(force, ForceMode.Impulse);
        
        Debug.Log($"[PlayerController] Jumped with Forces: Lateral={lateralForce:F1}, Up={upForce:F1}, Fwd={fwdForce:F1}");
    }

    private void CheckGround()
    {
        if (groundCheck == null) return;
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