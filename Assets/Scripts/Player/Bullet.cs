using UnityEngine;
using MercenaryWars.Enemy;

namespace MercenaryWars.Combat
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 10f;
        public int damage = 10;

        private void Start()
        {
            Destroy(gameObject, 3f);
        }

        private void Update()
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                EnemyAI enemy = other.GetComponent<EnemyAI>();
                if (enemy != null)
                    enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}