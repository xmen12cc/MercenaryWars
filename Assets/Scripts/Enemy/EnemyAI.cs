using UnityEngine;
using MercenaryWars.Data;
using MercenaryWars.Player;
using MercenaryWars.Managers;
using MercenaryWars.UI;

namespace MercenaryWars.Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyAI : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private EnemyData enemyData;

        [Header("Patrol Setup")]
        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;

        [Header("Detection Setup")]
        [SerializeField] private float detectRange = 10f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float shootRange = 6f;
        [SerializeField] private LayerMask playerLayer;

        [Header("Combat Settings")]
        [SerializeField] private float attackCooldown = 1.5f;

        [Header("Ranged Settings")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float bulletSpeed = 8f;

        [Header("Knockback")]
        [SerializeField] private float knockbackForce = 4f;

        [Header("Weapon Visuals")]
        [SerializeField] private GameObject weaponHolder;

        private Rigidbody2D rb;
        private Transform currentTarget;
        private Transform playerTransform;

        private int currentHealth;
        private float lastAttackTime;
        private bool isChasing = false;
        private bool facingLeft = false;

        [Header("Health Bar")]
        [SerializeField] private GameObject healthBarPrefab;
        private HealthBar healthBar;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            if (enemyData != null)
                currentHealth = enemyData.health;

            currentTarget = pointA;

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
            CheckForPlayer();

            if (isChasing && playerTransform != null)
                ChasePlayer();
            else
                Patrol();
        }

        private void CheckForPlayer()
        {
            Collider2D playerCol = Physics2D.OverlapCircle(transform.position, detectRange, playerLayer);

            if (playerCol != null)
            {
                if (!isChasing)
                    AudioManager.Instance?.PlayEnemySpot();

                playerTransform = playerCol.transform;
                isChasing = true;
            }
            else
            {
                playerTransform = null;
                isChasing = false;
            }
        }

        private void Patrol()
        {
            if (pointA == null || pointB == null || enemyData == null) return;

            Vector2 targetPos = new Vector2(currentTarget.position.x, rb.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPos, enemyData.speed * Time.deltaTime);

            FlipSprite(currentTarget.position.x < transform.position.x);

            if (Mathf.Abs(transform.position.x - currentTarget.position.x) < 0.1f)
                currentTarget = currentTarget == pointA ? pointB : pointA;
        }

        private void ChasePlayer()
        {
            if (playerTransform == null || enemyData == null) return;

            float distToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            FlipSprite(playerTransform.position.x < transform.position.x);

            if (distToPlayer <= attackRange)
            {
                MeleeAttack();
            }
            else if (distToPlayer <= shootRange)
            {
                ShootAtPlayer();
            }
            else
            {
                Vector2 targetPos = new Vector2(playerTransform.position.x, rb.position.y);
                transform.position = Vector2.MoveTowards(transform.position, targetPos, enemyData.speed * Time.deltaTime);
            }
        }

        private void MeleeAttack()
        {
            if (Time.time - lastAttackTime < attackCooldown) return;
            lastAttackTime = Time.time;

            PlayerController player = playerTransform.GetComponent<PlayerController>();
            if (player != null && enemyData != null)
                player.TakeDamage(enemyData.damage);

            AudioManager.Instance?.PlayEnemyAttack();
        }

        private void ShootAtPlayer()
        {
            if (Time.time - lastAttackTime < attackCooldown) return;
            lastAttackTime = Time.time;

            AudioManager.Instance?.PlayEnemyAttack();

            if (weaponHolder != null)
            {
                weaponHolder.SetActive(true);
                CancelInvoke(nameof(HideWeapon));
                Invoke(nameof(HideWeapon), 0.2f);
            }

            if (bulletPrefab == null || firePoint == null) return;

            Vector2 dir = ((Vector2)playerTransform.position - (Vector2)firePoint.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));

            EnemyBullet eb = bullet.GetComponent<EnemyBullet>();
            if (eb != null)
                eb.SetDirection(dir);
        }

        private void HideWeapon()
        {
            if (weaponHolder != null)
                weaponHolder.SetActive(false);
        }

        private void FlipSprite(bool faceLeft)
        {
            if (facingLeft == faceLeft) return;
            facingLeft = faceLeft;
            transform.Rotate(0f, 180f, 0f);
        }

        public void TakeDamage(int amount)
        {
            currentHealth -= amount;
            currentHealth = Mathf.Max(currentHealth, 0);

            if (healthBar != null)
                healthBar.UpdateFill((float)currentHealth / enemyData.health);

            if (currentHealth <= 0)
                Die();
        }

        public void ApplyKnockback(Vector2 sourcePosition)
        {
            Vector2 knockDir = (rb.position - sourcePosition).normalized;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);
        }

        private void Die()
        {
            AudioManager.Instance?.PlayEnemyDeath();
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, shootRange);
        }
    }
}