using UnityEngine;
using MercenaryWars.Managers;
using MercenaryWars.Enemy;
using MercenaryWars.Data;
using Unity.Cinemachine;
using MercenaryWars.UI;

namespace MercenaryWars.Player
{
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

        [Header("Weapon")]
        [SerializeField] private WeaponData currentWeapon;
        [SerializeField] private Transform firePoint;

        [Header("Melee")]
        [SerializeField] private Transform meleeHitbox;
        [SerializeField] private float meleeRadius = 0.5f;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private int meleeDamage = 25;
        [SerializeField] private GameObject swordHitEffectPrefab; // assign in Inspector
        [SerializeField] private float meleeEffectOffset = 0.8f;  // how far in front

        [Header("Health")]
        [SerializeField] private int maxHealth = 100;

        [Header("Knockback")]
        [SerializeField] private float knockbackForce = 5f;

        [Header("Weapon Visuals")]
        [SerializeField] private GameObject weaponHolder;

        private int currentHealth;
        private Rigidbody2D rb;
        private bool isGrounded;
        private float moveInput;
        private bool facingRight = true;
        private float lastFireTime;
        private float footstepTimer = 0f;
        private SpriteRenderer spriteRenderer;

        [Header("Health Bar")]
        [SerializeField] private GameObject healthBarPrefab;
        private HealthBar healthBar;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            currentHealth = maxHealth;
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (weaponHolder != null)
                weaponHolder.SetActive(false);
        
        if (healthBarPrefab != null)
        {
            GameObject hb = Instantiate(healthBarPrefab);
            healthBar = hb.GetComponent<HealthBar>();
            if (healthBar != null)
                healthBar.Setup(transform, 1f);
        }
        }

        private void Update()
        {
            moveInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Jump") && isGrounded)
                Jump();

            // C to shoot, F to melee
            if (Input.GetKeyDown(KeyCode.C))
                Shoot();

            if (Input.GetKeyDown(KeyCode.F))
                MeleeAttack();

            // Flip sprite
            if (moveInput > 0 && !facingRight) Flip();
            else if (moveInput < 0 && facingRight) Flip();

            // Footsteps
            if (isGrounded && Mathf.Abs(moveInput) > 0.1f)
            {
                footstepTimer -= Time.deltaTime;
                if (footstepTimer <= 0f)
                {
                    AudioManager.Instance?.PlayFootstep();
                    footstepTimer = 0.35f;
                }
            }
        }

        private void FixedUpdate()
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

            if (groundCheckPoint != null)
                isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();

                Vector2 knockDir = (transform.position - collision.transform.position).normalized;
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);

                TakeDamage(10); // contact damage

                if (enemy != null)
                    enemy.ApplyKnockback(transform.position);
            }
        }

        private void Jump()
        {
            AudioManager.Instance?.PlayJump();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        private void Shoot()
        {
            if (currentWeapon == null) return;
            if (currentWeapon.bulletPrefab == null) return;
            if (firePoint == null) return;

            if (Time.time - lastFireTime < currentWeapon.fireRate) return;
            lastFireTime = Time.time;

            AudioManager.Instance?.PlayShoot();
            Instantiate(currentWeapon.bulletPrefab, firePoint.position, firePoint.rotation);

            // Show pistol briefly
            if (weaponHolder != null)
            {
                weaponHolder.SetActive(true);
                CancelInvoke(nameof(HideWeapon));
                Invoke(nameof(HideWeapon), 0.2f);
            }
        }

        private void HideWeapon()
        {
            if (weaponHolder != null)
                weaponHolder.SetActive(false);
        }

        private void MeleeAttack()
        {
            AudioManager.Instance?.PlayMelee();

            // Spawn sword hit effect in front of player
            if (swordHitEffectPrefab != null)
            {
                float dir = facingRight ? 1f : -1f;
                Vector3 spawnPos = transform.position + new Vector3(dir * meleeEffectOffset, 0f, 0f);
                GameObject effect = Instantiate(swordHitEffectPrefab, spawnPos, Quaternion.identity);
                Destroy(effect, 0.3f); // remove after brief flash
            }

            // Damage enemies in front
            if (meleeHitbox == null) return;
            Collider2D[] hits = Physics2D.OverlapCircleAll(meleeHitbox.position, meleeRadius, enemyLayer);
            foreach (Collider2D hit in hits)
            {
                EnemyAI enemy = hit.GetComponent<EnemyAI>();
                if (enemy != null)
                    enemy.TakeDamage(meleeDamage);
            }
        }

        public void TakeDamage(int amount)
        {
            currentHealth -= amount;
            currentHealth = Mathf.Max(currentHealth, 0);

            if (healthBar != null)
                healthBar.UpdateFill((float)currentHealth / maxHealth);

            AudioManager.Instance?.PlayPlayerHurt();
            GetComponent<CinemachineImpulseSource>().GenerateImpulse();

            if (currentHealth <= 0)
                Die();
        }

        private void Die()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.LoadGameOver();

            Destroy(gameObject);
        }

        private void Flip()
        {
            facingRight = !facingRight;
            transform.Rotate(0f, 180f, 0f);
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheckPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
            }
            if (meleeHitbox != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(meleeHitbox.position, meleeRadius);
            }
        }
    }
}