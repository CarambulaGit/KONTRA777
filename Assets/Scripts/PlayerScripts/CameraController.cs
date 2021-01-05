using Photon.Pun;
using UnityEngine;

namespace PlayerScripts
{
	public class CameraController : MonoBehaviour {
		public const string playerCameraTag = "PlayerCam";
		private Vector3 playerCameraOffset;
		private PhotonView photonView;
		private Transform playerCamera;

		void Start() {
			photonView = GetComponent<PhotonView>();
			playerCamera = GameObject.FindGameObjectWithTag(playerCameraTag).transform;
			playerCameraOffset = new Vector3(0, 0, playerCamera.position.z);
		}

		void Update() {
			if (!photonView.IsMine) return;
			playerCamera.position = transform.position + playerCameraOffset;
		}
	}
}