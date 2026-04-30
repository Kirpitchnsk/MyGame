using UnityEngine;

namespace SibGameJam2026.Characters.Components {
	public class MovementCharacterComponent : IMovementCharacterComponent {
		private readonly ACharacter _character;
		public ACharacter Character => _character;
		private readonly Transform _transform;
		private Vector3 _moveInput;
		private Vector3 _lookDirection;

		public float MoveSpeed { get; }
		public float RotationSpeed { get; }

		public MovementCharacterComponent(ACharacter character, float moveSpeed = 5f, float rotationSpeed = 360f) {
			_character = character;
			_transform = character.transform;
			MoveSpeed = moveSpeed;
			RotationSpeed = rotationSpeed;
		}

		public void SetMoveInput(Vector3 moveInput) {
			_moveInput = Vector3.ClampMagnitude(moveInput, 1f);
		}

		public void SetLookDirection(Vector3 lookDirection) {
			lookDirection.y = 0f;
			_lookDirection = lookDirection.normalized;
		}

		public void Stop() {
			_moveInput = Vector3.zero;
		}

		public void OnUpdate() {
			if (_moveInput.sqrMagnitude > 0f) {
				_transform.position += _moveInput * (MoveSpeed * Time.deltaTime);
			}

			if (_lookDirection.sqrMagnitude > 0f) {
				var targetRotation = Quaternion.LookRotation(_lookDirection, Vector3.up);
				_transform.rotation = Quaternion.RotateTowards(
					_transform.rotation,
					targetRotation,
					RotationSpeed * Time.deltaTime);
			}
		}
	}
}
