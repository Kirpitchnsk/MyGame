using SibGameJam2026.MergeService;
using SibGameJam2026.Characters.Components;
using UnityEngine;

namespace SibGameJam2026 {
	public class ItemVisual : MonoBehaviour, IInteractable {
		[field: SerializeField] public Collider InteractionCollider { get; private set; }
		[field: SerializeField] public Item ItemData { get; private set; }
		public int ItemId => ItemData.Id;

		private void Awake() {
			if (InteractionCollider == null)
				InteractionCollider = GetComponent<Collider>();
		}

		public void Initialize(Item item) {
			ItemData = item;
			gameObject.name = $"ItemVisual_{item.Name}_{item.Id}";
		}

		public void SetInteractionColliderEnabled(bool isEnabled) {
			if (InteractionCollider != null)
				InteractionCollider.enabled = isEnabled;
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
