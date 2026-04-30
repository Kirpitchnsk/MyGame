using UnityEngine;
using SibGameJam2026.Services;
using Zenject;

namespace SibGameJam2026.Cameras {
	public class CameraController : MonoBehaviour {
		[SerializeField] private Vector3 _localOffset = Vector3.zero;
		[SerializeField] private float _lookSensitivity = 120f;
		[SerializeField] private float _lookDeadZone = 0.1f;
		[SerializeField] private float _minPitch = -80f;
		[SerializeField] private float _maxPitch = 80f;
		[SerializeField] private string _lookVectorAction = "CameraRotation";

		private IInputService _inputService;
		private float _pitch;

		public Transform Parent { get; private set; }
		public Transform YawTarget { get; private set; }

		public void Initialize(Transform parent, IInputService inputService) {
			Parent = parent;
			YawTarget = parent.root;
			_inputService = inputService;
		}

		private void LateUpdate() {
			if (Parent == null || _inputService == null) {
				return;
			}

			var lookInput = _inputService.GetVector(_lookVectorAction);

			if (Mathf.Abs(lookInput.x) > _lookDeadZone && YawTarget != null) {
				YawTarget.Rotate(Vector3.up, lookInput.x * _lookSensitivity * Time.deltaTime, Space.World);
			}

			if (Mathf.Abs(lookInput.y) > _lookDeadZone) {
				_pitch = Mathf.Clamp(_pitch - lookInput.y * _lookSensitivity * Time.deltaTime, _minPitch, _maxPitch);
			}

			transform.position = Parent.position + Parent.TransformDirection(_localOffset);
			transform.rotation = Quaternion.Euler(_pitch, Parent.eulerAngles.y, 0f);
		}


		public class Factory : PlaceholderFactory<ECameraType, Transform, CameraController> { }
	}
}
