using System.Collections;
using Photon.Pun;
using Resources.Classes;
using UnityEngine;

namespace PlayerScripts {
    public class WeaponController : MonoBehaviour {
        public PlayerController PlayerController;
        public Transform firePoint;
        public LineRenderer lineRenderer;
        public PhotonView photonView;
        private InGameCanvasController canvasController;

        void Start() {
            canvasController = GameObject.FindGameObjectWithTag("InGameCanvas").GetComponent<InGameCanvasController>();
        }

        void Update() {
            // if (!canvasController.isReady) return;
            if (!photonView.IsMine) return;
            if (PlayerController.isDead) return;
            if (Input.GetButtonDown("Fire1")) Shoot();
        }

        bool Shoot() {
            var hitInfo = Physics2D.Raycast(firePoint.position, firePoint.up);
            StartCoroutine(AnimateShoot(hitInfo));
            PhotonView hittedPlayerPV;
            if (hitInfo.transform.TryGetComponent<PhotonView>(out hittedPlayerPV)) {
                photonView.RPC(nameof(GiveDamageRPC), RpcTarget.All, PlayerSoldier.localPlayer.weapon.damage,
                    hittedPlayerPV.ViewID);
            }

            return hitInfo;
        }

        [PunRPC]
        private void GiveDamageRPC(float damage, int viewId) {
            if (PlayerSoldier.localPlayer.photonView.ViewID != viewId) return;
            Debug.Log($"Auch! Taken {damage} damage");
            PlayerSoldier.localPlayer.TakeDamage(damage);
        }

        IEnumerator AnimateShoot(RaycastHit2D hitInfo) {
            if (hitInfo) {
                lineRenderer.SetPosition(0, firePoint.position);
                lineRenderer.SetPosition(1, hitInfo.transform.position);
                // Debug.Log($"Hit {hitInfo.transform.name}");
            }
            else {
                lineRenderer.SetPosition(0, firePoint.position);
                lineRenderer.SetPosition(1, firePoint.position + firePoint.up * 100);
            }

            lineRenderer.enabled = true;
            yield return new WaitForSeconds(0.02f);
            lineRenderer.enabled = false;
        }
    }
}