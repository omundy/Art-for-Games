using UnityEngine;

/// <summary>
/// An enemy that wanders left and right, sometimes pausing or turning around.
/// Stay On Platform: it turns around at the edge of whatever it's standing on.
/// Jump Between Platforms: at an edge (and sometimes along the way) it works out where a jump would land,
/// and only jumps if that's on another platform. Otherwise it turns around, so it never jumps into a gap.
/// Needs a Rigidbody2D and a Collider2D. Platforms need to be on the Ground Layer
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Enemy_Wander : MonoBehaviour
{
    public enum WanderMode
    {
        StayOnPlatform,
        JumpBetweenPlatforms
    }

    [Header("Wandering")]

    [Tooltip("Stay On Platform: turn around at edges. Jump Between Platforms: jump to other platforms when it can")]
    public WanderMode mode = WanderMode.StayOnPlatform;

    [Tooltip("The layers it can stand on (platforms, ground and walls)")]
    public LayerMask groundLayer;

    [Tooltip("Walking speed, in units per second")]
    public float moveSpeed = 2;

    [Tooltip("Start by walking right (or left)")]
    public bool startFacingRight = true;

    [Tooltip("The chance each second of turning around for no reason (0 = never, 1 = about once a second)")]
    [Range(0, 1)]
    public float turnChance = 0.1f;

    [Tooltip("The chance each second of stopping for a moment")]
    [Range(0, 1)]
    public float pauseChance = 0.15f;

    [Tooltip("How long a pause lasts, in seconds: somewhere between x and y")]
    public Vector2 pauseTime = new Vector2(0.5f, 2);

    [Header("Jumping (Jump Between Platforms only)")]

    [Tooltip("How fast it goes up when it jumps")]
    public float jumpForce = 7;

    [Tooltip("How fast it goes sideways while in the air")]
    public float jumpSpeed = 3;

    [Tooltip("The chance each second of trying a jump in the middle of a platform, like up onto a higher one " +
        "(it only jumps if it would land on a platform)")]
    [Range(0, 1)]
    public float jumpChance = 0.3f;

    [Tooltip("The furthest it will jump down, in units")]
    public float maxDropDistance = 6;

    [Tooltip("Seconds to wait after landing before jumping again")]
    public float jumpCooldown = 0.5f;

    // which way it's walking: 1 is right, -1 is left
    int direction = 1;

    // is it standing on something, and what?
    bool isGrounded = false;
    Collider2D currentGround;

    // when the current pause ends, and when it's allowed to jump again
    float pauseUntil = 0;
    float canJumpAt = 0;

    Rigidbody2D rb2d;
    Collider2D body;

    // how far below its feet to look for the ground
    const float groundCheckDistance = 0.1f;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        body = GetComponent<Collider2D>();

        // stop it tipping over when it bumps into things
        rb2d.freezeRotation = true;

        if (groundLayer.value == 0)
        {
            Debug.LogWarning(name + ": Enemy_Wander's Ground Layer isn't set, so it can't find anything to stand on");
        }
    }

    void Start()
    {
        // face the starting direction
        direction = startFacingRight ? 1 : -1;
        Face(direction);
    }

    // physics code goes in FixedUpdate, which runs in step with the physics engine
    void FixedUpdate()
    {
        CheckIfGrounded();

        // in the air, keep going wherever the jump (or fall) is taking it
        if (!isGrounded) return;

        // pausing: stand still
        if (Time.time < pauseUntil)
        {
            SetSpeed(0);
            return;
        }

        // sometimes turn around or pause, just to look less like a robot.
        // multiplying by the time step turns "chance per second" into "chance this step"
        if (Random.value < turnChance * Time.fixedDeltaTime)
        {
            Turn();
        }
        if (Random.value < pauseChance * Time.fixedDeltaTime)
        {
            pauseUntil = Time.time + Random.Range(pauseTime.x, pauseTime.y);
            return;
        }

        if (mode == WanderMode.JumpBetweenPlatforms && Time.time >= canJumpAt)
        {
            // at an edge, try to jump. Along the way, sometimes try a jump up onto a higher platform
            bool atEdge = !GroundAhead();
            bool feelLikeJumping = Random.value < jumpChance * Time.fixedDeltaTime;
            if ((atEdge || feelLikeJumping) && JumpWouldLand())
            {
                Jump();
                return;
            }
        }

        // nowhere to stand ahead, so turn around
        if (!GroundAhead())
        {
            Turn();
        }

        SetSpeed(moveSpeed * direction);
    }

    // walk at this speed, keeping the up and down speed from gravity
    void SetSpeed(float speed)
    {
        rb2d.linearVelocity = new Vector2(speed, rb2d.linearVelocity.y);
    }

    void Turn()
    {
        direction = -direction;
        Face(direction);
    }

    // flip the enemy to face left (-1) or right (1). Children, like eyes, flip with it
    void Face(int newDirection)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * newDirection;
        transform.localScale = scale;
    }

    void Jump()
    {
        rb2d.linearVelocity = new Vector2(jumpSpeed * direction, jumpForce);
        isGrounded = false;
        canJumpAt = Time.time + jumpCooldown;
    }

    // the middle of the bottom of its collider
    Vector2 Feet()
    {
        Bounds bounds = body.bounds;
        return new Vector2(bounds.center.x, bounds.min.y);
    }

    // look straight down from its feet for something to stand on
    void CheckIfGrounded()
    {
        bool wasGrounded = isGrounded;

        // start a little inside the collider, so the ray doesn't begin inside the ground
        Vector2 start = Feet() + Vector2.up * 0.05f;
        RaycastHit2D hit = Physics2D.Raycast(start, Vector2.down, groundCheckDistance + 0.05f, groundLayer);

        // only count it as standing once it has stopped going up (so a jump doesn't "land" straight away)
        isGrounded = hit.collider != null && rb2d.linearVelocity.y <= 0.01f;
        currentGround = isGrounded ? hit.collider : null;

        if (isGrounded && !wasGrounded)
        {
            // just landed: wait a moment before jumping again
            canJumpAt = Mathf.Max(canJumpAt, Time.time + jumpCooldown);
        }
    }

    // is there ground just in front of its front foot?
    bool GroundAhead()
    {
        Bounds bounds = body.bounds;
        Vector2 frontFoot = new Vector2(bounds.center.x + (bounds.extents.x + 0.05f) * direction, bounds.min.y + 0.05f);
        return Physics2D.Raycast(frontFoot, Vector2.down, 0.5f, groundLayer).collider != null;
    }

    // follow the jump's arc in small steps, the way gravity would move it, and see where it lands.
    // true if it lands on top of a platform other than the one it's on now
    bool JumpWouldLand()
    {
        Vector2 point = Feet() + Vector2.up * 0.1f;
        Vector2 velocity = new Vector2(jumpSpeed * direction, jumpForce);
        Vector2 gravity = Physics2D.gravity * rb2d.gravityScale;
        float lowest = point.y - maxDropDistance;
        float step = Time.fixedDeltaTime;

        // 5 seconds is far longer than any jump, it just stops the loop running forever
        for (float time = 0; time < 5; time += step)
        {
            Vector2 next = point + velocity * step;
            velocity += gravity * step;

            RaycastHit2D hit = Physics2D.Linecast(point, next, groundLayer);
            if (hit.collider != null)
            {
                // landing means coming down onto the top of something.
                // hitting a wall, a ceiling, or the platform it's leaving means the jump won't work
                bool onTop = velocity.y < 0 && hit.normal.y > 0.5f;
                return onTop && hit.collider != currentGround;
            }

            // fell further than it's willing to drop
            if (next.y < lowest) return false;

            point = next;
        }
        return false;
    }

    // turn around when it walks into something, like a wall, another enemy or the player
    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // a contact pointing back against the direction it's walking is something in its way
            if (contact.normal.x * direction < -0.5f)
            {
                Turn();
                return;
            }
        }
    }

    // when the enemy is selected, draw the edge check and the jump it would make
    void OnDrawGizmosSelected()
    {
        Collider2D bodyCollider = GetComponent<Collider2D>();
        Rigidbody2D bodyRigidbody = GetComponent<Rigidbody2D>();
        if (bodyCollider == null || bodyRigidbody == null) return;

        int facing = transform.localScale.x < 0 ? -1 : 1;
        Bounds bounds = bodyCollider.bounds;

        // the edge check, under the front foot
        Gizmos.color = Color.yellow;
        Vector2 frontFoot = new Vector2(bounds.center.x + (bounds.extents.x + 0.05f) * facing, bounds.min.y + 0.05f);
        Gizmos.DrawLine(frontFoot, frontFoot + Vector2.down * 0.5f);

        if (mode != WanderMode.JumpBetweenPlatforms) return;

        // the jump's arc, the same way JumpWouldLand() works it out
        Gizmos.color = Color.cyan;
        Vector2 point = new Vector2(bounds.center.x, bounds.min.y + 0.1f);
        Vector2 velocity = new Vector2(jumpSpeed * facing, jumpForce);
        Vector2 gravity = Physics2D.gravity * bodyRigidbody.gravityScale;
        float step = 0.02f;
        for (float time = 0; time < 3 && point.y > bounds.min.y - maxDropDistance; time += step)
        {
            Vector2 next = point + velocity * step;
            velocity += gravity * step;
            Gizmos.DrawLine(point, next);
            point = next;
        }
    }
}
