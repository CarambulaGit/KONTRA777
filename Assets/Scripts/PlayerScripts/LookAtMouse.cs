using Photon.Pun;
using UnityEngine;
using Resources.Classes;

namespace PlayerScripts {
    public class LookAtMouse : MonoBehaviour {
        public PlayerController PlayerController;
        public PhotonView photonView;
        private Camera playerCamera;
        public Transform rotationPoint;
        private InGameCanvasController canvasController;

        void Start() {
            playerCamera = GameObject.FindGameObjectWithTag(CameraController.playerCameraTag).GetComponent<Camera>();
            canvasController = GameObject.FindGameObjectWithTag("InGameCanvas").GetComponent<InGameCanvasController>();
        }

        void Update() {
            // if (!canvasController.isReady) return;
            if (!photonView.IsMine) return;
            if (PlayerController.isDead) return;
            Vector3 aimDir = (playerCamera.WorldToScreenPoint(rotationPoint.position) - Input.mousePosition).normalized;
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, 0, angle + 90);
        }
    }
}