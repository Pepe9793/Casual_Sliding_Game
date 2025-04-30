using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        respawnPosition = transform.position;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDead) return;

        HandleMovement();
        HandleJump();
        CheckOutOfBounds();
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
        }
    }

    void CheckOutOfBounds()
    {
        if (transform.position.y < -10f)
        {
            TakeDamage(1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Platform platform = collision.gameObject.GetComponent<Platform>();
        if (!platform) return;

        HandlePlatformEffects(platform, collision);
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
        else if (platform._movingplatform)
        {
            transform.SetParent(collision.transform);
        }
    }

    private System.Collections.IEnumerator BreakPlatform(GameObject platform)
    {
        yield return new WaitForSeconds(0.5f);
        platform.SetActive(false);
        transform.SetParent(null);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Platform>()?._movingplatform ?? false)
        {
            transform.SetParent(null);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Player Died! Respawning...");
        Invoke("Respawn", 1f);
    }

    void Respawn()
    {
        currentHealth = maxHealth;
        transform.position = respawnPosition;
        isDead = false;
    }
}