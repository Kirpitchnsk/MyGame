using SibGameJam2026.Cameras;
using SibGameJam2026.Characters;
using UnityEngine;
using Zenject;

namespace SibGameJam2026.Services {
	public class GameService : IGameService, ITickable {
		private IInventoryService _inventoryService;
		private readonly ACharacter.Factory _characterFactory;
		private readonly CameraController.Factory _cameraFactory;
		private float _startupTimer;
		
		public GameService(
			IInventoryService inventoryService,
			ACharacter.Factory characterFactory,
			CameraController.Factory cameraFactory) {
			_inventoryService = inventoryService;
			_characterFactory = characterFactory;
			_cameraFactory = cameraFactory;
			Debug.Log("GameService is Initialized" + inventoryService);
		}
		
		public bool IsGameActive { get; private set; } = false;

		public void Tick() {
			if (IsGameActive)
				return;

			_startupTimer += Time.deltaTime;
			if (_startupTimer < 3f)
				return;

			StartGame();
		}
		
		public void StartGame() {
			if (IsGameActive)
				return;
			
			var character = _characterFactory.Create(ECharacterType.Player);
			var cameraParent = character.Data.CameraPoint != null ? character.Data.CameraPoint : character.transform;
			_cameraFactory.Create(ECameraType.FirstPerson, cameraParent);

			_inventoryService.Add(1);
			IsGameActive = true;
		}

		public void CompleteGame()
		{
			IsGameActive = false;
		}
	}
}