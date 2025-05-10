using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip damageSFX;
    [SerializeField] private AudioClip deathSFX;
    [SerializeField] private AudioClip respawnSFX;
    private AudioSource audioSource;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public float groundCheckRadius = 0.2f;
    public Transform groundCheck;
    public LayerMask platformLayer;

    [Header("Health Settings")]
    public int maxHealth = 3;
    public Vector3 respawnPosition;

    private Rigidbody2D rb;
    private bool isGrounded;
    private int currentHealth;
    private bool isDead;

    private Platform currentMovingPlatform;
    private Vector3 platformPreviousPosition;
    private bool isOnMovingPlatform;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        respawnPosition = transform.position;
        currentHealth = maxHealth;

        if (groundCheck == null)
        {
            Debug.LogError("GroundCheck not assigned in PlayerController!");
        }

    }

    private void Update()
    {
        if (isDead) return;

        HandleAnimation();

        if (currentMovingPlatform != null && IsGrounded())
        {
            Vector3 delta = currentMovingPlatform.transform.position - platformPreviousPosition;
            transform.position += delta;
            platformPreviousPosition = currentMovingPlatform.transform.position;
        }

        HandleMovement();
        HandleJump();
        CheckOutOfBounds();
    }

    private void LateUpdate()
    {
        if (isOnMovingPlatform && currentMovingPlatform != null && IsGrounded())
        {
            // Calculate platform movement delta
            Vector3 delta = currentMovingPlatform.transform.position - platformPreviousPosition;
            transform.position += delta;

            // Update previous position for next frame
            platformPreviousPosition = currentMovingPlatform.transform.position;
        }
    }

    void HandleAnimation()
    {
        // Flip sprite based on movement direction
        float moveInput = Input.GetAxis("Horizontal");
        if (moveInput > 0.1f)
        {
            _spriteRenderer.flipX = false;
        }
        else if (moveInput < -0.1f)
        {
            _spriteRenderer.flipX = true;
        }

        // Set animation parameters
        _animator.SetFloat("Speed", Mathf.Abs(moveInput));
        _animator.SetBool("isGrounded", isGrounded);
        // _animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
    }

    void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            audioSource.PlayOneShot(jumpSFX);
        }
    }

    void CheckOutOfBounds()
    {
        if (transform.position.y < -5.5f)
        {
            TakeDamage(1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Platform platform = collision.gameObject.GetComponent<Platform>();
        if (!platform) return;

        if (IsGrounded() && collision.contacts[0].normal.y > 0.5f)
        {
            platform.PlayLandSFX();
            if (platform._movingplatform && IsGrounded() && collision.contacts[0].normal.y > 0.5f)
            {
                currentMovingPlatform = platform;
                isOnMovingPlatform = true;
                transform.SetParent(platform.transform); // 👈 Parent to platform
            }

        }

        HandlePlatformEffects(platform, collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Platform>() == currentMovingPlatform)
        {
            currentMovingPlatform = null;
            isOnMovingPlatform = false;
            transform.SetParent(null); // 👈 Unparent when leaving
        }

    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformLayer);
    }

    void HandlePlatformEffects(Platform platform, Collision2D collision)
    {
        if (platform._isspikes)
        {
            TakeDamage(1);
        }
        else if (platform._isbreakable)
        {
            StartCoroutine(BreakPlatform(collision.gameObject));
        }
    }

    private System.Collections.IEnumerator BreakPlatform(GameObject platform)
    {
        yield return new WaitForSeconds(0.5f);
        if (platform.activeInHierarchy)
            platform.SetActive(false);
        transform.SetParent(null);
    }


    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Health: {currentHealth}/{maxHealth}");

        if (audioSource && damageSFX) // Add null checks
            audioSource.PlayOneShot(damageSFX);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        audioSource.PlayOneShot(deathSFX);
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Player Died! Respawning...");
        Invoke("Respawn", 1f);
    }

    void Respawn()
    {
        audioSource.PlayOneShot(respawnSFX);
        currentHealth = maxHealth;
        transform.position = respawnPosition;
        isDead = false;
    }
}

