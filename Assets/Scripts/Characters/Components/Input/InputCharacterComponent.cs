using SibGameJam2026.Services;
using UnityEngine;

namespace SibGameJam2026.Characters.Components {
	public class InputCharacterComponent : IInputCharacterComponent {
		private readonly ACharacter _character;
		public ACharacter Character => _character;
		private readonly IInputService _inputService;

		private const string MovementAction = "Movement";
		private const float MoveDeadZone = 0.05f;

		public InputCharacterComponent(ACharacter character, IInputService inputService) {
			_character = character;
			_inputService = inputService;
		}

		public void OnUpdate() {
			if (!_character.TryGetComponent<IMovementCharacterComponent>(out var movementComponent)) {
				return;
			}

			var moveInput = _inputService.GetVector(MovementAction);
			var activeCamera = Camera.main;

			Vector3 forward;
			Vector3 right;

			if (activeCamera != null) {
				forward = activeCamera.transform.forward;
				right = activeCamera.transform.right;
			} else {
				forward = _character.transform.forward;
				right = _character.transform.right;
			}

			forward.y = 0f;
			right.y = 0f;
			forward.Normalize();
			right.Normalize();

			var moveDirection = forward * moveInput.y + right * moveInput.x;

			if (moveDirection.sqrMagnitude > MoveDeadZone * MoveDeadZone) {
				movementComponent.SetMoveInput(moveDirection);
			} else {
				movementComponent.Stop();
			}
		}
	}
}
