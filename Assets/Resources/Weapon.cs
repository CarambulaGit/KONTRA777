using UnityEngine;
using UnityEngine.Serialization;

namespace Resources {
    [CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObject/Weapon", order = 1)]
    public class Weapon : ScriptableObject {
        public string name;
        // private Sprite sprite; // todo 
        public float damage;
        public int bulletsInMagazine;
        public int numOfBullets;
        public float reloadTime;
        public int currentAmmo;
        public bool isReloading;
        public AudioClip shootSound;
        public AudioClip reloadSound;
    }
}