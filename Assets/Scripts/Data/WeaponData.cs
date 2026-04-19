using UnityEngine;

namespace MercenaryWars.Data
{
    /// <summary>
    /// Hold weapon stats.
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "Mercenary Wars/Data/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Basic Info")]
        [Tooltip("Name of weapon.")]
        public string weaponName;

        [Header("Combat Stats")]
        [Tooltip("Damage per hit.")]
        public int damage;

        [Tooltip("Time between shots.")]
        public float fireRate;

        [Header("Ammunition")]
        [Tooltip("Bullet to shoot.")]
        public GameObject bulletPrefab;

        [Tooltip("Type of ammo used.")]
        public AmmoType ammoType;
    }

    /// <summary>
    /// Types of ammo.
    /// </summary>
    public enum AmmoType
    {
        Light,
        Heavy,
        Energy,
        Explosive
    }
}
