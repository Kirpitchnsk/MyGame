using SibGameJam2026.MergeService;
using UnityEngine;

namespace SibGameJam2026 {
	public class ItemVisual : MonoBehaviour {
		public Item ItemData { get; private set; }
		public int ItemId => ItemData.Id;

		public void Initialize(Item item) {
			ItemData = item;
			gameObject.name = $"ItemVisual_{item.Name}_{item.Id}";
		}
	}
}
