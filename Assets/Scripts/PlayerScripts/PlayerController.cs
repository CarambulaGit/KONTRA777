using Photon.Pun;
using Resources.Classes;
using ServerScripts;
using Unity.Collections;
using UnityEngine;

namespace PlayerScripts {
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable {
        private const float EPSILON = 0.00001f;
        public float moveSpeed;
        public Animator animator;
        private PhotonView photonView;
        private float moveAnimSpeed;
        [ReadOnly] private float health;

        void Start() {
            photonView = GetComponent<PhotonView>();
            health = PlayerSoldier.defaultHealth;
        }

        void FixedUpdate() {
            if (!photonView.IsMine) return;
            InGameManager.localPlayer.health = health;
            InGameManager.localPlayer.TakeDamage(InGameManager.localPlayer.takenDamageThisTick);
            Move(out moveAnimSpeed);
            Animate();
            health = InGameManager.localPlayer.health;
            // PhotonView.Find()
        }

        private void Move(out float animSpeed) {
            var posChange = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
            if (posChange.magnitude > Vector3.right.magnitude) {
                posChange = posChange.normalized;
            }

            animSpeed = posChange.magnitude;
            transform.position += posChange * (moveSpeed * Time.deltaTime);
        }

        private void Animate() {
            AnimateMove();
        }

        private void AnimateMove() {
            animator.SetFloat("MoveSpeed", moveAnimSpeed);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
            if (stream.IsWriting) {
                stream.SendNext(health);
            }
            else {
                this.health = (float) stream.ReceiveNext();
            }
        }
    }
}