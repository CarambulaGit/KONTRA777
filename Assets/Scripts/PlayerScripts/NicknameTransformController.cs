using System;
using UnityEngine;

namespace PlayerScripts {
    public class NicknameTransformController : MonoBehaviour {
        public Transform player;
        [SerializeField] private Vector3 defaultPosition;

        void Update() {
            transform.position = player.position + defaultPosition;
            transform.eulerAngles = Vector3.zero;
        }
    }
}