using UnityEngine;

namespace Resources {
    [CreateAssetMenu(fileName = "New Soldier", menuName = "ScriptableObject/Soldier", order = 1)]
    public class Soldier : ScriptableObject {
        public float health;
    }
}