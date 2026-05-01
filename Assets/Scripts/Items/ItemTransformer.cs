using SibGameJam2026.MergeService;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SibGameJam2026 {
	public abstract class ItemTransformer : AInteractItemVisual {
		[SerializeField] private List<EItemType> _supportedInputTypes = new();
		[SerializeField] private float _processingTimeSeconds = 5f;

		private bool _isProcessing;
		private float _remainingTime;
		private Item _processingInputItem;
		private Item _processingOutputItem;
		private IMergeSystem _mergeSystem;
		private ItemsFactory _itemsFactory;

		[Inject]
		private void Construct(IMergeSystem mergeSystem, ItemsFactory itemsFactory) {
			_mergeSystem = mergeSystem;
			_itemsFactory = itemsFactory;
		}

		public override void OnInteract(InteractContext context) {
			if (_mergeSystem == null || _itemsFactory == null) {
				D.Error($"{nameof(ItemTransformer)} dependencies are not injected.");
				return;
			}
			
			var item = context.UsedItem;

			if (_isProcessing) {
				D.Log($"{nameof(ItemTransformer)} is busy.");
				return;
			}

			if (_supportedInputTypes != null && !_supportedInputTypes.Contains(item.ItemType)) {
				D.Log($"{nameof(ItemTransformer)} cannot process item type {item.ItemType}.");
				return;
			}
			
			if (!_mergeSystem.TryGetMergedProductId(new[] { item.Id }, out var outputItemId)) {
				D.Log($"{nameof(ItemTransformer)} cannot transform item {item.Name}({item.Id}). Recipe was not found.");
				return;
			}

			_processingOutputItem = _itemsFactory.Create(outputItemId);
			_processingInputItem = item;
			_remainingTime = _processingTimeSeconds;
			
			StartProcessing();
		}

		private void Update() {
			if (!_isProcessing)
				return;

			_remainingTime -= Time.deltaTime;
			if (_remainingTime > 0f)
				return;

			CompleteProcessing();
		}

		protected virtual void StartProcessing() {
			_isProcessing = true;
		}

		protected virtual void CompleteProcessing() {
			D.Log($"{nameof(ItemTransformer)} processed {_processingInputItem.Name}({_processingInputItem.Id}) to {_processingOutputItem.Name}({_processingOutputItem.Id})");
			_isProcessing = false;
		}
	}
}
