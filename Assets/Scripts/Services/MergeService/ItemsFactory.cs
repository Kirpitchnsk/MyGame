using System;
using Zenject;

namespace SibGameJam2026.MergeService {
	public class ItemsFactory : IFactory<int, Item> {
		private readonly ItemsDatabase _itemsDatabase;

		public ItemsFactory(ItemsDatabase itemsDatabase) {
			_itemsDatabase = itemsDatabase;
		}

		public Item Create(int itemId) {
			if (_itemsDatabase.TryGetItemById(itemId, out var item)) {
				return item;
			}

			throw new ArgumentOutOfRangeException(nameof(itemId), itemId, "Item was not found in ItemsDatabase.");
		}
	}
}
