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
        public AudioSource audio;
        public ParticleSystem shootParticle;
        private InGameCanvasController canvasController;
        private float reloadTimer;


        void Start() {
            canvasController = GameObject.FindGameObjectWithTag("InGameCanvas").GetComponent<InGameCanvasController>();
            audio.clip = PlayerController.weapon.shootSound;
        }

        void Update() {
            // if (!canvasController.isReady) return;
            if (!photonView.IsMine) return;
            if (PlayerController.isDead) return;
            if (!PlayerController.init) return;
            if (PlayerSoldier.localPlayer.weapon.currentAmmo == 0 || (Input.GetKeyDown(KeyCode.R)) || PlayerSoldier.localPlayer.weapon.isReloading)
            {
                ReloadTimer();
            }
            if (Input.GetButtonDown("Fire1")) Shoot();
        }

        bool Shoot() {
            if (PlayerSoldier.localPlayer.weapon.numOfBullets == 0 && PlayerSoldier.localPlayer.weapon.currentAmmo == 0)
            {
                // TODO sound not amoo
            }
            if (PlayerSoldier.localPlayer.weapon.isReloading) return false;
            PlayerSoldier.localPlayer.weapon.currentAmmo--;
            var hitInfo = Physics2D.Raycast(firePoint.position, firePoint.up);
            photonView.RPC(nameof(ShootRPC), RpcTarget.All, PlayerSoldier.localPlayer.photonView.ViewID,
                firePoint.position, hitInfo ? (Vector3) hitInfo.point : firePoint.position + firePoint.up * 100);
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
                audio.Play();
                shootParticle.Emit(1);
            }
        }

        IEnumerator AnimateShoot(Vector3 startPos, Vector3 finishPos) {
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, finishPos);
            lineRenderer.enabled = true;
            yield return new WaitForSeconds(0.02f);
            lineRenderer.enabled = false;
        }

        public void ReloadTimer() {

            if (PlayerSoldier.localPlayer.weapon.numOfBullets == 0 && PlayerSoldier.localPlayer.weapon.currentAmmo == 0)
            {
                PlayerSoldier.localPlayer.weapon.isReloading = true;
                Debug.Log("No ammo!");
                return;
            }
            if (!PlayerSoldier.localPlayer.weapon.isReloading)
            {
                reloadTimer = 0;
                PlayerSoldier.localPlayer.weapon.isReloading = true;
            }
            else
            {
                reloadTimer += Time.deltaTime;
                if (reloadTimer >= PlayerSoldier.localPlayer.weapon.reloadTime)
                {
                    Reload();
                    PlayerSoldier.localPlayer.weapon.isReloading = false;
                }
            }
        }

        public void Reload()
        {
            if (PlayerSoldier.localPlayer.weapon.numOfBullets < PlayerSoldier.localPlayer.weapon.bulletsInMagazine)
            {
                PlayerSoldier.localPlayer.weapon.currentAmmo = PlayerSoldier.localPlayer.weapon.numOfBullets;
                PlayerSoldier.localPlayer.weapon.numOfBullets = 0;
                return;
            }
            PlayerSoldier.localPlayer.weapon.numOfBullets -= PlayerSoldier.localPlayer.weapon.bulletsInMagazine - PlayerSoldier.localPlayer.weapon.currentAmmo;
            PlayerSoldier.localPlayer.weapon.currentAmmo = PlayerSoldier.localPlayer.weapon.bulletsInMagazine;
            Debug.Log("Reloading");
        }
    }
}