using System.Collections;
using Photon.Pun;
using Resources;
using Resources.Classes;
using UnityEngine;
using System.Collections;

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
            PlayerController.weapon.numOfBullets = 120;
            PlayerController.weapon.currentAmmo = PlayerController.weapon.bulletsInMagazine;
            canvasController = GameObject.FindGameObjectWithTag("InGameCanvas").GetComponent<InGameCanvasController>();
            audio = GetComponent<AudioSource>();
        }

        void Update() {
            // if (!canvasController.isReady) return;
            if (!photonView.IsMine) return;
            if (PlayerController.isDead) return;
            if (PlayerController.weapon.numOfBullets <= 0)
            {
                Debug.Log("Not ammo)");
                //Sound empty magazine
                return;
            }
            if (PlayerController.weapon.isReloading) return;
            if (PlayerController.weapon.currentAmmo <= 0)
            {
                StartCoroutine(Reload());
                return;
            }
            if (Input.GetButtonDown("Fire1")) Shoot();
        }

        bool Shoot() {
            PlayerController.weapon.currentAmmo--;
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

        private IEnumerator Reload() {
            PlayerController.weapon.isReloading = true;
            Debug.Log("Reloading...");
            yield return new WaitForSeconds(PlayerController.weapon.reloadTime);
            PlayerController.weapon.currentAmmo = PlayerController.weapon.bulletsInMagazine;
            PlayerController.weapon.numOfBullets -= PlayerController.weapon.bulletsInMagazine;
            PlayerController.weapon.isReloading = false;
        }
    }
}