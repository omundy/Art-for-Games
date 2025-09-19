using System;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.InputSystem; // NEW INPUT SYSTEM

/// Basic 2D platformer character controller
/// 2022 Owen Mundy
/// Based on Brackeys "2D Movement in Unity (Tutorial)"
/// https://www.youtube.com/watch?v=dwcT-Dch0bA&list=PLPV2KyIb3jR6TFcFuzI2bB7TMNIIBpKMQ&index=2&t=1008s
/// 2025 Improved to make it easier to reuse for non-platformer characters

// Ensure required components are also added to Game Object
public class PlayerController : MonoBehaviour
{
    [Header("References")]

    public Animator animator;
    public Rigidbody2D rb2d;
    [Tooltip("Position marking where to check if the player is isGrounded")]
    public Transform groundCheck;


    [Header("Input")]

    [Tooltip("Movement speed and direction")]
    public Vector2 playerInput;
    // NEW INPUT SYSTEM > receive input 
    InputAction moveAction;
    InputAction jumpAction;

    [Tooltip("Store jump just pressed")]
    public bool jumpPress = false;
    [Tooltip("Store jump was held")]
    public bool jumpHold = false;

    [Tooltip("Which direction is the player facing")]
    public bool facingRight = true;
    [Tooltip("Is the player on the ground?")]
    public bool isGrounded;
    [Tooltip("Player's current velocity'")]
    public Vector3 velocity = Vector3.zero;


    [Header("Settings")]

    [Tooltip("A mask determining what is ground to the character")]
    public LayerMask groundLayer;

    [Tooltip("How fast should they run")]
    public float runSpeed = 30f;
    [Tooltip("Force added when the player jumps")]
    public float jumpForce = 6.2f;
    [Tooltip("Force added to bring them back to earth")]
    public float fallMultiplier = 2.5f;
    [Tooltip("Force added (on low jump) to bring them back to earth")]
    public float lowJumpMultiplier = 3f;
    [Tooltip("How much to smooth out the movement")]
    [Range(0, .3f)] public float movementSmoothing = .05f;
    [Tooltip("Whether or not a player can steer while jumping")]
    public bool airControl = true;
    [Tooltip("Radius of the overlap circle to determine if isGrounded")]
    const float isGroundedRadius = .2f;

    public float rememberGroundedFor = 0.1f;
    public float lastTimeGrounded;



    private void Awake()
    {
        // get components
        if (rb2d == null) GetComponent<Rigidbody2D>();
        if (animator == null) GetComponent<Animator>();
        if (groundCheck == null) groundCheck = transform.Find("GroundCheck").gameObject.transform;

        if (!groundCheck)
        {
            Debug.LogError("GroundCheck required");
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    void Start()
    {
        // NEW INPUT SYSTEM > Find references to InputSystem_Actions
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    private void Update()
    {
        // get horizontal input (between -1 and 1) from player
        // playerInput.x = Input.GetAxisRaw("Horizontal"); // Legacy Input
        playerInput = moveAction.ReadValue<Vector2>(); // NEW INPUT SYSTEM
        Move(playerInput.x * runSpeed * Time.fixedDeltaTime);

        // check if jump keys / buttons are pressed on this loop
        // jumpPress = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.UpArrow);
        jumpPress = jumpAction.WasPressedThisFrame(); // NEW INPUT SYSTEM
        if (jumpPress) Debug.Log("Jump");
        // jumpPress = jumpAction.IsPressed();
        Jump();

        // check if jump keys / buttons are held down
        // jumpHold = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow);
        jumpHold = Keyboard.current.spaceKey.isPressed || Keyboard.current.upArrowKey.isPressed; // NEW INPUT SYSTEM
        BetterJump();

        // whether currently on ground (collision detection)
        CheckIfGrounded();
    }

    public void Move(float move)
    {
        if (animator != null)
            // set the speed variable
            animator.SetFloat("Speed", Mathf.Abs(move));


        // only control the player if isGrounded or airControl is turned on
        if (isGrounded || airControl)
        {
            if (rb2d != null)
            {
                // Move the character by finding the target velocity
                Vector3 targetVelocity = new Vector2(move * 10f, rb2d.linearVelocity.y);
                // And then smoothing it out and applying it to the character
                rb2d.linearVelocity = Vector3.SmoothDamp(rb2d.linearVelocity, targetVelocity, ref velocity, movementSmoothing);
            }
            // If the input is moving the player right and the player is facing left...
            if ((move > 0 && !facingRight) || (move < 0 && facingRight))
            {
                Flip();
            }
        }
    }

    public void Jump()
    {
        if (rb2d != null)
        {
            // if jump button pressed + either grounded or late jump off edge 
            if (jumpPress && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor))
            {
                // Add a vertical force to the player 
                // => Brackeys version: e.g. jumpForce = 550, rb2d.gravityScale = 3
                //rb2d.AddForce(new Vector2(0f, jumpForce));

                // Add a vertical force to the player 
                // => craftgames version: e.g. jumpForce = 6, rb2d.gravityScale = 1
                rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, jumpForce);

                // reset for late jump
                lastTimeGrounded = Time.time;

                if (animator != null)
                    animator.SetBool("IsJumping", true);
            }
        }
    }

    // adjust falling and jumping speed based on whether player still has jump pressed
    // for more see: https://www.youtube.com/watch?v=7KiK0Aqtmzc
    void BetterJump()
    {
        if (rb2d != null)
        {
            // slow down velocity for all
            if (rb2d.linearVelocity.y < 0)
            {
                rb2d.linearVelocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
            }
            // slow up velocity if they release early
            else if (rb2d.linearVelocity.y > 0 && !jumpHold)
            {
                rb2d.linearVelocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;
        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void CheckIfGrounded()
    {
        // is currently grounded?
        bool wasGrounded = isGrounded;
        isGrounded = false;

        // Check if a circle cast to the groundcheck position hits anything designated as Ground
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, isGroundedRadius, groundLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                // if not parent collider then set true 
                isGrounded = true;
                // if wasn't already on ground then it just landed
                if (!wasGrounded)
                {
                    //Debug.Log("collided with: " + colliders[i].gameObject.name);
                    OnLanding();
                }
            }
        }
    }

    void OnLanding()
    {
        if (animator != null)
            animator.SetBool("IsJumping", false);
    }
}