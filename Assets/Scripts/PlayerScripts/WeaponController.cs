using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace PlayerScripts {
    public class WeaponController : MonoBehaviour {
        public Transform firePoint;
        public LineRenderer lineRenderer;
        private PhotonView photonView;

        void Start() {
            photonView = GetComponent<PhotonView>();
        }

        void Update() {
            if (!photonView.IsMine) return;
            if (Input.GetButtonDown("Fire1")) Shoot();
        }

        bool Shoot() {
            var hitInfo = Physics2D.Raycast(firePoint.position, firePoint.up);
            StartCoroutine(AnimateShoot(hitInfo));
            PlayerController hittedPlayerController;
            if (hitInfo.transform.TryGetComponent<PlayerController>(out hittedPlayerController)) {
                //TODO take some damage
            }

            return hitInfo;
        }

        IEnumerator AnimateShoot(RaycastHit2D hitInfo) {
            if (hitInfo) {
                lineRenderer.SetPosition(0, firePoint.position);
                lineRenderer.SetPosition(1, hitInfo.transform.position);
                Debug.Log($"Hit {hitInfo.transform.name}");
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