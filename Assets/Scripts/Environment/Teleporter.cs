using System.Collections;
using UnityEngine;
using MercenaryWars.Player;

namespace MercenaryWars.Environment
{
    // Teleporter — place two of these in the scene and link them to each other.
    //
    // Setup:
    //   1. Create an empty GameObject, add a BoxCollider2D (Is Trigger = true).
    //   2. Attach this script.
    //   3. Assign the other teleporter's Transform to the Destination slot.
    //   4. Repeat for the second teleporter, pointing back at the first.
    //   5. Add a portal sprite as a child so the player can see it.
    //
    // The cooldown prevents the player from immediately triggering the
    // destination teleporter and bouncing back.
    public class Teleporter : MonoBehaviour
    {
        [Header("Link")]
        [SerializeField] private Transform destination;

        [Header("Settings")]
        [SerializeField] private float cooldown = 1.2f;

        private bool ready = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!ready) return;
            if (destination == null) return;

            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc == null) return;

            // Move the player
            other.transform.position = destination.position;

            // Suppress velocity so the player lands cleanly
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            // Lock this teleporter briefly, then also lock the destination
            // so the player isn't bounced straight back.
            StartCoroutine(Cooldown());

            Teleporter dest = destination.GetComponent<Teleporter>();
            if (dest != null)
                dest.StartCoroutine(dest.Cooldown());
        }

        public IEnumerator Cooldown()
        {
            ready = false;
            yield return new WaitForSeconds(cooldown);
            ready = true;
        }

        private void OnDrawGizmos()
        {
            if (destination == null) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.4f);
            Gizmos.DrawLine(transform.position, destination.position);
            Gizmos.DrawWireSphere(destination.position, 0.4f);
        }
    }
}
