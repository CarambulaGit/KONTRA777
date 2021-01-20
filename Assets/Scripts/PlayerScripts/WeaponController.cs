using System.Collections;
using Photon.Pun;
using Resources;
using Resources.Classes;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace PlayerScripts {
    public class WeaponController : MonoBehaviour {
        public PlayerController PlayerController;
        public Transform firePoint;
        public LineRenderer lineRenderer;
        public PhotonView photonView;
        public AudioSource audioShoot;
        public AudioSource audioReload;
        public ParticleSystem shootParticle;
        private InGameCanvasController canvasController;
        private float reloadTimer;
       

        public static UnityAction IAmReloading;

        void Start() {
            canvasController = GameObject.FindGameObjectWithTag("InGameCanvas").GetComponent<InGameCanvasController>();
            audioShoot.clip = PlayerController.weapon.shootSound;
        }

        void Update() {
            // if (!canvasController.isReady) return;
            if (!photonView.IsMine) return;
            if (PlayerController.isDead) return;
            if (!PlayerController.init) return;
            if (PlayerSoldier.localPlayer.weapon.currentAmmo == 0 || Input.GetKeyDown(KeyCode.R) ||
                PlayerSoldier.localPlayer.weapon.isReloading) {
                ReloadTimer();
            }

            if (Input.GetButtonDown("Fire1")) Shoot();
        }

        bool Shoot() {
            if (PlayerSoldier.localPlayer.weapon.numOfBullets == 0 &&
                PlayerSoldier.localPlayer.weapon.currentAmmo == 0) {
                // TODO sound not amoo
            }

            if (PlayerSoldier.localPlayer.weapon.isReloading) return false;
            PlayerSoldier.localPlayer.weapon.currentAmmo--;
            var rand = Spreading(PlayerSoldier.localPlayer.weapon.spread);
            var angle = Vector2.SignedAngle(Vector2.up, firePoint.up);
            var hitInfo = Physics2D.Raycast(firePoint.position, firePoint.up + new Vector3(Mathf.Cos(angle) * rand.x, Mathf.Sin(angle) * rand.x));
            photonView.RPC(nameof(ShootRPC), RpcTarget.All, PlayerSoldier.localPlayer.photonView.ViewID,
                firePoint.position, hitInfo ? (Vector3) hitInfo.point : firePoint.position + firePoint.up * 100);
            if (hitInfo.transform.TryGetComponent<PhotonView>(out var hittedPlayerPV)) {
                photonView.RPC(nameof(GiveDamageRPC), RpcTarget.All, PlayerSoldier.localPlayer.weapon.damage,
                    hittedPlayerPV.ViewID, PlayerSoldier.localPlayer.photonView.ViewID);
            }

            return hitInfo;
        }

        //Разброс
        private float FindG(float random, float a){
            return Mathf.Pow(random / 0.5f, 0.5f) * a - a;
        }

        private float FindRandNumberUsingSimpson(float random, float a) {
            return random <= 0.5 ? FindG(random, a) : -FindG(1 - random, a);
        }

        private Vector2 Spreading(float a) {
            return new Vector2(FindRandNumberUsingSimpson(Random.value, a), 100).normalized;
        }
        //Конец разброса

        [PunRPC]
        private void GiveDamageRPC(float damage, int viewIdBeenDamaged, int viewIdWhoShooted) {
            if (PlayerSoldier.localPlayer.photonView.ViewID == viewIdBeenDamaged) {
                Debug.Log($"Auch! Taken {damage} damage");
                PlayerSoldier.localPlayer.TakeDamage(damage);
                PlayerSoldier.localPlayer.playerController.isDamaged();
            }else if (photonView.ViewID == viewIdBeenDamaged) {
                PlayerSoldier.localPlayer.playerController.blood.Emit(4);

            }
            
        }

        [PunRPC]
        private void ShootRPC(int viewIdWhoShooted, Vector3 startPos, Vector3 finishPos) {
            if (photonView.ViewID == viewIdWhoShooted) {
                StartCoroutine(AnimateShoot(startPos, finishPos));
                audioShoot.clip = PlayerController.weapon.shootSound;
                audioShoot.Play();
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

        private void ReloadTimer() {
            if (PlayerSoldier.localPlayer.weapon.numOfBullets == 0 &&
                PlayerSoldier.localPlayer.weapon.currentAmmo == 0) {
                PlayerSoldier.localPlayer.weapon.isReloading = true;
                Debug.Log("No ammo!");
                return;
            }
            if (PlayerSoldier.localPlayer.weapon.numOfBullets == 0 && PlayerSoldier.localPlayer.weapon.currentAmmo != 0) return;
            if (!PlayerSoldier.localPlayer.weapon.isReloading) {
                audioReload.clip = PlayerController.weapon.reloadSound;
                audioReload.Play();
                reloadTimer = 0;
                PlayerSoldier.localPlayer.weapon.isReloading = true;
            }
            
            reloadTimer += Time.deltaTime;
            IAmReloading?.Invoke();
            if (reloadTimer >= PlayerSoldier.localPlayer.weapon.reloadTime) {
                Reload();
                PlayerSoldier.localPlayer.weapon.isReloading = false;
            }
            
        }
        
        private void Reload() {
            var allAmmo = PlayerSoldier.localPlayer.weapon.currentAmmo + PlayerSoldier.localPlayer.weapon.numOfBullets;
            PlayerSoldier.localPlayer.weapon.currentAmmo = allAmmo >= PlayerSoldier.localPlayer.weapon.bulletsInMagazine 
                                                                    ? PlayerSoldier.localPlayer.weapon.bulletsInMagazine 
                                                                    : allAmmo % PlayerSoldier.localPlayer.weapon.bulletsInMagazine;
            PlayerSoldier.localPlayer.weapon.numOfBullets = allAmmo - PlayerSoldier.localPlayer.weapon.currentAmmo;
        }

    }
}