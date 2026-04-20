using UnityEngine;
using MercenaryWars.Player;

namespace MercenaryWars.Environment
{
    // Attach to a GameObject with a Collider2D set to Is Trigger.
    // Deals damage to the player every second they remain inside.
    // Resize the collider in the Inspector to match the hazard tile(s).
    public class HazardTile : MonoBehaviour
    {
        [Header("Damage Settings")]
        [SerializeField] private int damagePerSecond = 10;

        private PlayerController playerInside;
        private float damageTimer;

        private void Update()
        {
            if (playerInside == null) return;

            damageTimer += Time.deltaTime;
            if (damageTimer >= 1f)
            {
                damageTimer = 0f;
                Debug.Log($"[HazardTile] Damaging player for {damagePerSecond} over time");
                playerInside.TakeDamage(damagePerSecond);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController pc = other.GetComponentInParent<PlayerController>();
                if (pc != null)
                {
                    Debug.Log($"[HazardTile] Player triggered enter");
                    playerInside = pc;
                    damageTimer  = 0f;
                    playerInside.TakeDamage(damagePerSecond);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.GetComponentInParent<PlayerController>() != null)
                {
                    Debug.Log($"[HazardTile] Player triggered exit");
                    playerInside = null;
                    damageTimer  = 0f;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerController pc = collision.gameObject.GetComponentInParent<PlayerController>();
                if (pc != null)
                {
                    Debug.Log($"[HazardTile] Player collided enter");
                    playerInside = pc;
                    damageTimer  = 0f;
                    playerInside.TakeDamage(damagePerSecond);
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (collision.gameObject.GetComponentInParent<PlayerController>() != null)
                {
                    Debug.Log($"[HazardTile] Player collided exit");
                    playerInside = null;
                    damageTimer  = 0f;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col == null) return;
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.35f);
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
        }
    }
}
