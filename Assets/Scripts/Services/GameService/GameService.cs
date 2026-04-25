using UnityEngine;
using Zenject;

namespace SibGameJam2026.Services {
	public class GameService : IGameService {
		private IInventoryService _inventoryService;
		
		public GameService(IInventoryService inventoryService) {
			_inventoryService = inventoryService;
			Debug.Log("GameService is Initialized" + inventoryService);
		}
		
		public bool IsGameActive { get; private set; } = false;
		
		public void StartGame() {
			if (IsGameActive)
				return;
			
			_inventoryService.Add(1);
			IsGameActive = true;
		}

		public void CompleteGame()
		{
			IsGameActive = false;
		}
	}
}