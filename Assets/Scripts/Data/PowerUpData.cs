using UnityEngine;

namespace MercenaryWars.Data
{
    /// <summary>
    /// Hold power-up stats.
    /// </summary>
    [CreateAssetMenu(fileName = "NewPowerUpData", menuName = "Mercenary Wars/Data/PowerUp Data")]
    public class PowerUpData : ScriptableObject
    {
        [Header("Settings")]
        [Tooltip("Type of buff.")]
        public PowerUpType powerUpType;

        [Tooltip("Amount to add/boost.")]
        public float value;

        [Tooltip("How long it lasts in seconds. 0 means instant use.")]
        public float duration;
    }

    /// <summary>
    /// Types of power-ups.
    /// </summary>
    public enum PowerUpType
    {
        HealthPack,
        SpeedBoost,
        DamageBoost
    }
}
