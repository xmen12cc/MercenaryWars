using UnityEngine;
using MercenaryWars.Player;

namespace MercenaryWars.Enemy
{
    public class EnemyBullet : MonoBehaviour
    {
        [SerializeField] private int damage = 10;
        [SerializeField] private float speed = 8f;

        private Vector2 direction;
        private bool directionSet = false;

        public void SetDirection(Vector2 dir)
        {
            direction = dir.normalized;
            directionSet = true;
        }

        private void Start()
        {
            Destroy(gameObject, 3f);
        }

        private void Update()
        {
            if (directionSet)
                transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Ignore enemies and other bullets
            if (other.CompareTag("Enemy")) return;
            if (other.isTrigger) return;

            // Damage player on contact
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
                player.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}