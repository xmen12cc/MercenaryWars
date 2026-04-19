using UnityEngine;
using MercenaryWars.Managers; // Assuming GameManager is here
using MercenaryWars.Enemy;

namespace MercenaryWars.Player
{
    /// <summary>
    /// Handles player movement, combat, and health.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 10f;
        
        [Header("Ground Check")]
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundLayer;

        [Header("Combat Setup")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private Transform meleeHitbox;
        [SerializeField] private float meleeRadius = 0.5f;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private int meleeDamage = 10;

        [Header("Health Setup")]
        [SerializeField] private int maxHealth = 100;
        
        private int currentHealth;
        private Rigidbody2D rb;
        private bool isGrounded;
        private Vector2 movement;
        private bool facingRight = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            currentHealth = maxHealth;
        }

        private void Update()
        {
            // Input
            movement.x = Input.GetAxisRaw("Horizontal");

            // Jump
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                Jump();
            }

            // Shoot
            if (Input.GetButtonDown("Fire1")) // Left click / Ctrl
            {
                Shoot();
            }

            // Melee
            if (Input.GetButtonDown("Fire2")) // Right click / Alt
            {
                MeleeAttack();
            }

            // Flip sprite based on direction
            if (movement.x > 0 && !facingRight) Flip();
            else if (movement.x < 0 && facingRight) Flip();
        }

        private void FixedUpdate()
        {
            // Move
            rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);

            // Ground Check
            isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        }

        private void Jump()
        {
            // Reset y velocity before jump for consistent height
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        private void Shoot()
        {
            if (bulletPrefab == null || firePoint == null) return;
            
            // Basic shoot in facing direction
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            // Assume bullet script handles its own velocity
        }

        private void MeleeAttack()
        {
            if (meleeHitbox == null) return;

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(meleeHitbox.position, meleeRadius, enemyLayer);
            
            foreach (Collider2D enemy in hitEnemies)
            {
                var enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.TakeDamage(meleeDamage);
                    Debug.Log("Hit " + enemy.name);
                }
            }
        }

        public void TakeDamage(int amount)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            // Tell GameManager player dead
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadGameOver(); // Assuming this method exists
            }
            
            Destroy(gameObject);
        }

        private void Flip()
        {
            facingRight = !facingRight;
            transform.Rotate(0f, 180f, 0f);
        }

        private void OnDrawGizmosSelected()
        {
            // Draw ground check
            if (groundCheckPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
            }

            // Draw melee range
            if (meleeHitbox != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(meleeHitbox.position, meleeRadius);
            }
        }
    }
}
