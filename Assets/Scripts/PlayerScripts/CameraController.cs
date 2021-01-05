using Photon.Pun;
using UnityEngine;

namespace PlayerScripts
{
	public class CameraController : MonoBehaviour {
		public const string playerCameraTag = "PlayerCam";
		public readonly Vector3 playerCameraOffset = new Vector3(0, 0, -10);
		private PhotonView photonView;
		private Transform playerCamera;

		void Start() {
			photonView = GetComponent<PhotonView>();
			playerCamera = GameObject.FindGameObjectWithTag(playerCameraTag).transform;
		}

		void Update() {
			if (!photonView.IsMine) return;
			playerCamera.position = transform.position + playerCameraOffset;
		}
	}
}