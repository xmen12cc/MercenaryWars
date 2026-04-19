using UnityEngine;

namespace MercenaryWars.Data
{
    /// <summary>
    /// Hold enemy stats.
    /// </summary>
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "Mercenary Wars/Data/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Name of enemy.")]
        public string enemyName;

        [Header("Stats")]
        [Tooltip("Max health.")]
        public int health;

        [Tooltip("Move speed.")]
        public float speed;

        [Tooltip("Attack damage.")]
        public int damage;

        [Header("Loot")]
        [Tooltip("Chance to drop item (0.0 to 1.0).")]
        [Range(0f, 1f)]
        public float dropChance;
    }
}
