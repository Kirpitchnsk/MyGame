using SibGameJam2026.MergeService;
using SibGameJam2026.Characters.Components;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SibGameJam2026 {
	public class ItemTransformer : AInteractItemVisual {
		[SerializeField] private List<EItemType> _supportedInputTypes;
		[SerializeField] private float _processingTimeSeconds = 5f;
		[SerializeField] private Transform _inputItemPosition;
		[SerializeField] private Transform _outputItemPosition;

		private bool _isProcessing;
		private float _remainingTime;
		private Item _processingInputItem;
		private int _processingOutputItemId;
		private IMergeSystem _mergeSystem;
		private ItemsFactory _itemsFactory;
		private ItemsDatabase _itemsDatabase;

		[Inject]
		private void Construct(IMergeSystem mergeSystem, ItemsFactory itemsFactory, ItemsDatabase itemsDatabase) {
			_mergeSystem = mergeSystem;
			_itemsFactory = itemsFactory;
			_itemsDatabase = itemsDatabase;
		}

		public override void OnInteract(InteractContext context) {
			if (_mergeSystem == null || _itemsFactory == null || _itemsDatabase == null) {
				D.Error($"{nameof(ItemTransformer)} dependencies are not injected.");
				return;
			}
			
			if (_isProcessing) {
				D.Log($"{nameof(ItemTransformer)} is busy.");
				return;
			}

			if (!context.UserCharacter.TryGetComponent<IInventoryComponent>(
				out var inventoryComponent) || !inventoryComponent.HasItem)
				return;

			var item = inventoryComponent.CurrentItem;
			if (_supportedInputTypes == null || !_supportedInputTypes.Contains(item.ItemType)) {
				D.Log($"{nameof(ItemTransformer)} cannot process item type {item.ItemType}.");
				return;
			}
			
			if (!_mergeSystem.TryGetMergedProductId(new[] { item.Id }, out var outputItemId)) {
				D.Log($"{nameof(ItemTransformer)} cannot transform item {item.Name}({item.Id}). Recipe was not found.");
				return;
			}

			if (!inventoryComponent.TryTakeItem(out var itemVisual))
				return;

			_processingOutputItemId = outputItemId;
			_processingInputItem = item;
			_remainingTime = _processingTimeSeconds;
			ProcessConsumedItemVisual(itemVisual, _inputItemPosition);
			
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

		private void ProcessConsumedItemVisual(ItemVisual itemVisual, Transform inputPosition) {
			if (itemVisual == null)
				return;

			if (inputPosition != null) {
				itemVisual.transform.SetPositionAndRotation(inputPosition.position, inputPosition.rotation);
				itemVisual.SetInteractionColliderEnabled(false);
			}

			_itemsFactory.ReturnToPool(itemVisual);
		}

		protected virtual void CompleteProcessing() {
			var outputPosition = _outputItemPosition != null ? _outputItemPosition.position : transform.position;
			var outputRotation = _outputItemPosition != null ? _outputItemPosition.rotation : transform.rotation;
			var outputVisual = _itemsFactory.Create(_processingOutputItemId, outputPosition, outputRotation);

			var outputName = _itemsDatabase.TryGetItemById(_processingOutputItemId, out var outputItem)
				? outputItem.Name
				: "Unknown";

			D.Log($"{nameof(ItemTransformer)} processed {_processingInputItem.Name}({_processingInputItem.Id}) to {outputName}({_processingOutputItemId}) visual:{outputVisual.name}");
			_isProcessing = false;
		}
	}
}
