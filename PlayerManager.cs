using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(TrailRenderer))]

public class PlayerManager : MonoBehaviour
{
    //* Attach this script to the Player game object.
    //* In Unity Editor, layer 3 should be "Static Normal Layer".
    //* In Unity Editor, layer 6 should be "Movable Normal Layer".
    //* In the project settings, make the default Physics2D material a frictionless and not bouncy material.
    //* Make sure that movable objects have a Rigidbody.

    [Header("Horizontal and Vertical")]
    const float minimum = 0.1f;
    int moveSpeed = 14, startMoveMultiplier = 14, stopMoveOnGroundMultiplier = 12, stopMoveOnAirMultiplier = 4, bumpStopMultiplier = 16;
    float horizontal, horizontalVelocityOfTheMovableGroundThatYouAreStandingOn, verticalVelocityOfTheMovableGroundThatYouAreStandingOn;
    bool facingRight = true, rightInput, leftInput, bumping;
    Vector2 theLocalScale;
    Transform playerTransform;
    Rigidbody2D playerRigidbody;

    [Header("Jump")]
    public static bool canDoubleJump;
    int jumpPower = 28, normalGravity = 9, fastFallingGravity = 14;
    bool groundedForAll, playerTouchingToAnyGround, playerStandingOnMovableGround, doubleJumped;

    [Header("Coyote Time")]
    const float coyoteTime = 0.1f;
    float coyoteTimeCounter;

    [Header("Jump Buffer")]
    const float jumpBufferTime = 0.2f;
    float jumpBufferCounter;

    [Header("Dash")]
    public static bool canDash;
    int dashSpeed = 28, dashSoundRandomizer;
    float dashTime = 0.2f, dashCooldown = 0.5f;
    bool isDashing, dashInput;
    TrailRenderer playerTrailRenderer;

    [Header("Keybinds")]
    KeyCode rightKey = KeyCode.D, leftKey = KeyCode.A, jumpKey = KeyCode.Space, dashKey = KeyCode.LeftShift;

    [Header("Inputs")]
    [SerializeField] Transform groundCheckTransform;
    [SerializeField] Transform bumpCheckTransform;
    [SerializeField] LayerMask staticNormalLayer, movableNormalLayer;
    [SerializeField] AudioSource jumpSound, doubleJumpSound, dashSound1, dashSound2, dashSound3, dashSound4, dashSound5;

    void Start()
    {
        playerTransform = transform;
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        playerRigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
        playerRigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        playerRigidbody.freezeRotation = true;
        playerRigidbody.gravityScale = normalGravity;
        playerTrailRenderer = GetComponent<TrailRenderer>();
        playerTrailRenderer.time = 0.5f;
        playerTrailRenderer.emitting = false;
    }

    void Update()
    {
        if (PauseMenuManager.gamePaused)
        {
            return;
        }

        MovementInputs();
        Jump();
    }

    void FixedUpdate()
    {// I didn't added the if not game paused condition because if game pauses, FixedUpdate pauses too.
        BumpCheck();
        GroundedCheckAndCoyoteTime();
        FastFallingAndFlip();
        Movement();
        Dash();
    }

    void MovementInputs()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else if (jumpBufferCounter <= 0)
        {
            jumpBufferCounter = 0;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (isDashing)
        {
            return;
        }

        dashInput = Input.GetKey(dashKey);
        rightInput = Input.GetKey(rightKey);
        leftInput = Input.GetKey(leftKey);

        if (rightInput && !leftInput)
        {
            horizontal += startMoveMultiplier * Time.deltaTime;
            horizontal = Mathf.Clamp(horizontal, -1, 1);
        }
        else if (!rightInput && leftInput)
        {
            horizontal -= startMoveMultiplier * Time.deltaTime;
            horizontal = Mathf.Clamp(horizontal, -1, 1);
        }
        else if (!rightInput && !leftInput)
        {
            if (!bumping)
            {
                if (groundedForAll)
                {
                    if (horizontal > minimum)
                    {
                        horizontal -= stopMoveOnGroundMultiplier * Time.deltaTime;
                        horizontal = Mathf.Clamp(horizontal, 0, 1);
                    }
                    else if (horizontal < -minimum)
                    {
                        horizontal += stopMoveOnGroundMultiplier * Time.deltaTime;
                        horizontal = Mathf.Clamp(horizontal, -1, 0);
                    }
                    else
                    {
                        horizontal = 0;
                    }
                }
                else
                {
                    if (horizontal > minimum)
                    {
                        horizontal -= stopMoveOnAirMultiplier * Time.deltaTime;
                        horizontal = Mathf.Clamp(horizontal, 0, 1);
                    }
                    else if (horizontal < -minimum)
                    {
                        horizontal += stopMoveOnAirMultiplier * Time.deltaTime;
                        horizontal = Mathf.Clamp(horizontal, -1, 0);
                    }
                    else
                    {
                        horizontal = 0;
                    }
                }
            }
            else
            {
                if (horizontal > minimum)
                {
                    horizontal -= bumpStopMultiplier * Time.deltaTime;
                    horizontal = Mathf.Clamp(horizontal, 0, 1);
                }
                else if (horizontal < -minimum)
                {
                    horizontal += bumpStopMultiplier * Time.deltaTime;
                    horizontal = Mathf.Clamp(horizontal, -1, 0);
                }
                else
                {
                    horizontal = 0;
                }
            }
        }
    }

    // * jumpBufferCounter > 0 ~= Input.GetKeyDown(jumpKey)
    // * coyoteTimeCounter > 0 ~= groundedForAll

    void Jump()
    {
        if (isDashing)
        {
            return;
        }

        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            if (!jumpSound.isPlaying)
            {
                jumpSound.Play();
            }
            else if (jumpSound.isPlaying)
            {
                jumpSound.Stop();
                jumpSound.Play();
            }

            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpPower);
            coyoteTimeCounter = 0;
            jumpBufferCounter = 0;
        }
        else if (canDoubleJump && !doubleJumped && jumpBufferCounter > 0 && !(coyoteTimeCounter > 0)) // Not grounded jump == Double jump
        {
            if (!doubleJumpSound.isPlaying)
            {
                doubleJumpSound.Play();
            }
            else if (doubleJumpSound.isPlaying)
            {
                doubleJumpSound.Stop();
                doubleJumpSound.Play();
            }

            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpPower);
            doubleJumped = true;
            coyoteTimeCounter = 0;
            jumpBufferCounter = 0;
        }

        if (Input.GetKeyUp(jumpKey) && playerRigidbody.velocity.y > minimum)
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.y / 2);
        }
    }

    void BumpCheck()
    {
        bumping = Physics2D.OverlapBox(bumpCheckTransform.position, new Vector2(0.1f, 0.95f), 0, staticNormalLayer | movableNormalLayer);
    }

    void GroundedCheckAndCoyoteTime()
    {
        groundedForAll = Physics2D.OverlapBox(groundCheckTransform.position, new Vector2(0.95f, 0.1f), 0, staticNormalLayer | movableNormalLayer);
        playerStandingOnMovableGround = Physics2D.OverlapBox(groundCheckTransform.position, new Vector2(0.95f, 0.1f), 0, movableNormalLayer);

        if (groundedForAll)
        {
            coyoteTimeCounter = coyoteTime;
            doubleJumped = false; // doubleJumped falsing
        }
        else if (coyoteTimeCounter <= 0)
        {
            coyoteTimeCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    void FastFallingAndFlip()
    {
        playerRigidbody.gravityScale = playerRigidbody.velocity.y > minimum ? normalGravity : fastFallingGravity;

        if (!isDashing && ((facingRight && leftInput && !rightInput) || (!facingRight && rightInput && !leftInput)))
        {
            facingRight = !facingRight;
            theLocalScale = playerTransform.localScale;
            theLocalScale.x *= -1;
            playerTransform.localScale = theLocalScale;
        }
    }

    void Movement()
    {
        if (isDashing)
        {
            return;
        }

        if (!(playerStandingOnMovableGround && playerTouchingToAnyGround))
        {
            playerRigidbody.velocity = new Vector2(horizontal * moveSpeed, playerRigidbody.velocity.y);
        }
        else
        {
            playerRigidbody.velocity = new Vector2(horizontal * moveSpeed + horizontalVelocityOfTheMovableGroundThatYouAreStandingOn, playerRigidbody.velocity.y + verticalVelocityOfTheMovableGroundThatYouAreStandingOn);
        }
    }

    void Dash()
    {
        if (canDash && !isDashing && dashInput)
        {
            StartCoroutine(Dashing());
        }
    }

    IEnumerator Dashing()
    {
        dashSoundRandomizer = Random.Range(1, 6);

        switch (dashSoundRandomizer)
        {
            case 1:
                dashSound1.Play();
                break;
            case 2:
                dashSound2.Play();
                break;
            case 3:
                dashSound3.Play();
                break;
            case 4:
                dashSound4.Play();
                break;
            case 5:
                dashSound5.Play();
                break;
        }

        isDashing = true;
        canDash = false;
        playerRigidbody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        playerRigidbody.gravityScale = 0;

        if (!playerStandingOnMovableGround)
        {
            playerRigidbody.velocity = new Vector2(playerTransform.localScale.x * dashSpeed, 0);
        }
        else
        {
            playerRigidbody.velocity = new Vector2(playerTransform.localScale.x * dashSpeed + horizontalVelocityOfTheMovableGroundThatYouAreStandingOn, 0);
        }

        yield return new WaitForSeconds(0.05f);
        playerTrailRenderer.emitting = true;
        yield return new WaitForSeconds(dashTime);
        playerTrailRenderer.emitting = false;
        playerRigidbody.gravityScale = normalGravity;
        playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3 || collision.gameObject.layer == 6)
        {
            playerTouchingToAnyGround = true;
        }

        if (playerStandingOnMovableGround && collision.gameObject.layer == 6)
        {
            horizontalVelocityOfTheMovableGroundThatYouAreStandingOn = collision.gameObject.GetComponent<Rigidbody2D>().velocity.x;
            verticalVelocityOfTheMovableGroundThatYouAreStandingOn = collision.gameObject.GetComponent<Rigidbody2D>().velocity.y;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3 || collision.gameObject.layer == 6)
        {
            playerTouchingToAnyGround = false;
        }
    }
}
