using UnityEngine;
using MercenaryWars.Data;
using MercenaryWars.Player;

namespace MercenaryWars.Enemy
{
    /// <summary>
    /// Simple Enemy AI. Patrols, chases player, attacks.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyAI : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private EnemyData enemyData;

        [Header("Patrol Setup")]
        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;
        
        [Header("Detection Setup")]
        [SerializeField] private float detectRange = 5f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private LayerMask playerLayer;

        [Header("Combat Settings")]
        [SerializeField] private float attackCooldown = 1.5f;

        private Rigidbody2D rb;
        private Transform currentTarget;
        private Transform playerTransform;
        
        private int currentHealth;
        private float lastAttackTime;
        private bool isChasing = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            
            if (enemyData != null)
            {
                currentHealth = enemyData.health;
            }
            
            currentTarget = pointA;
        }

        private void Update()
        {
            CheckForPlayer();

            if (isChasing && playerTransform != null)
            {
                ChasePlayer();
            }
            else
            {
                Patrol();
            }
        }

        private void CheckForPlayer()
        {
            Collider2D playerCol = Physics2D.OverlapCircle(transform.position, detectRange, playerLayer);
            
            if (playerCol != null)
            {
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

            // Move toward current target point
            Vector2 targetPos = new Vector2(currentTarget.position.x, rb.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPos, enemyData.speed * Time.deltaTime);

            // Check if reached point
            if (Mathf.Abs(transform.position.x - currentTarget.position.x) < 0.1f)
            {
                currentTarget = currentTarget == pointA ? pointB : pointA;
            }
        }

        private void ChasePlayer()
        {
            if (playerTransform == null || enemyData == null) return;

            float distToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distToPlayer <= attackRange)
            {
                Attack();
            }
            else
            {
                // Move toward player
                Vector2 targetPos = new Vector2(playerTransform.position.x, rb.position.y);
                transform.position = Vector2.MoveTowards(transform.position, targetPos, enemyData.speed * Time.deltaTime);
            }
        }

        private void Attack()
        {
            if (Time.time - lastAttackTime < attackCooldown) return;

            lastAttackTime = Time.time;
            
            PlayerController player = playerTransform.GetComponent<PlayerController>();
            if (player != null && enemyData != null)
            {
                player.TakeDamage(enemyData.damage);
                Debug.Log($"{gameObject.name} attacked Player for {enemyData.damage} damage.");
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
            // Handle loot drop here using enemyData.dropChance if needed
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            // Draw detect range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectRange);

            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
