using SibGameJam2026.MergeService;
using SibGameJam2026.Characters.Components;
using UnityEngine;

namespace SibGameJam2026 {
	public class ItemVisual : MonoBehaviour, IInteractable {
		[field: SerializeField] public Item ItemData { get; private set; }
		public int ItemId => ItemData.Id;

		public void Initialize(Item item) {
			ItemData = item;
			gameObject.name = $"ItemVisual_{item.Name}_{item.Id}";
		}

		public void OnInteract(InteractContext context)		{
			if (!context.UserCharacter.TryGetComponent<IInventoryComponent>(out var inventoryComponent))
				return;

			if (inventoryComponent.HasItem)
				return;

			if (!inventoryComponent.TryStoreItem(this))
				return;
		}
	}
}
