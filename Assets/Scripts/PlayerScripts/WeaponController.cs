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
        public AudioClip shootSound;
        private InGameCanvasController canvasController;

        private AudioSource audio;

        void Start() {
            canvasController = GameObject.FindGameObjectWithTag("InGameCanvas").GetComponent<InGameCanvasController>();
            audio = GetComponent<AudioSource>();
        }

        void Update() {
            // if (!canvasController.isReady) return;
            if (!photonView.IsMine) return;
            if (PlayerController.isDead) return;
            if (Input.GetButtonDown("Fire1")) Shoot();
        }

        bool Shoot() {
            var hitInfo = Physics2D.Raycast(firePoint.position, firePoint.up);
            photonView.RPC(nameof(ShootRPC), RpcTarget.All, PlayerSoldier.localPlayer.photonView.ViewID,
                firePoint.position, hitInfo ? hitInfo.transform.position : firePoint.position + firePoint.up * 100);
            if (hitInfo.transform.TryGetComponent<PhotonView>(out var hittedPlayerPV)) {
                photonView.RPC(nameof(GiveDamageRPC), RpcTarget.All, PlayerSoldier.localPlayer.weapon.damage,
                    hittedPlayerPV.ViewID, PlayerSoldier.localPlayer.photonView.ViewID);
            }

            return hitInfo;
        }

        [PunRPC]
        private void GiveDamageRPC(float damage, int viewIdBeenDamaged, int viewIdWhoShooted) {
            if (PlayerSoldier.localPlayer.photonView.ViewID == viewIdBeenDamaged) {
                Debug.Log($"Auch! Taken {damage} damage");
                PlayerSoldier.localPlayer.TakeDamage(damage);
            }
        }

        [PunRPC]
        private void ShootRPC(int viewIdWhoShooted, Vector3 startPos, Vector3 finishPos) {
            if (photonView.ViewID == viewIdWhoShooted) {
                StartCoroutine(AnimateShoot(startPos, finishPos));
                audio.PlayOneShot(shootSound);
                Debug.Log($"Pif paf oyoyoy");
            }
        }

        // IEnumerator AnimateShoot(RaycastHit2D hitInfo) {
        IEnumerator AnimateShoot(Vector3 startPos, Vector3 finishPos) {
            // lineRenderer.SetPosition(0, firePoint.position);
            // lineRenderer.SetPosition(1, hitInfo.transform.position);
            // // Debug.Log($"Hit {hitInfo.transform.name}");
            // lineRenderer.SetPosition(0, firePoint.position);
            // lineRenderer.SetPosition(1, firePoint.position + firePoint.up * 100);
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, finishPos);
            lineRenderer.enabled = true;
            yield return new WaitForSeconds(0.02f);
            lineRenderer.enabled = false;
        }
    }
}