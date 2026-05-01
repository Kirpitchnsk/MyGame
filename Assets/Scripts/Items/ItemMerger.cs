using SibGameJam2026.MergeService;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Zenject;

namespace SibGameJam2026 {
	public abstract class ItemMerger : AInteractItemVisual {
		[SerializeField] private List<EItemType> _supportedInputTypes;
		[SerializeField] private int _maxBufferedItems = 3;
		[SerializeField] private float _processingTimeSeconds = 5f;

		private readonly List<Item> _bufferedItems = new();

		private IMergeSystem _mergeSystem;
		private ItemsFactory _itemsFactory;
		private Item _processingOutputItem;
		private string _processingInputInfo;
		private bool _isProcessing;
		private float _remainingTime;

		[Inject]
		private void Construct(IMergeSystem mergeSystem, ItemsFactory itemsFactory) {
			_mergeSystem = mergeSystem;
			_itemsFactory = itemsFactory;
		}

		public override void OnInteract(InteractContext context) {
			if (_isProcessing) {
				D.Log($"{nameof(ItemMerger)} is busy.");
				return;
			}

			if (_mergeSystem == null || _itemsFactory == null) {
				D.Error($"{nameof(ItemMerger)} dependencies are not injected.");
				return;
			}

			var item = context.UsedItem;
			if (_supportedInputTypes != null && !_supportedInputTypes.Contains(item.ItemType)) {
				D.Log($"{nameof(ItemMerger)} cannot accept item type {item.ItemType}.");
				return;
			}

			if (_bufferedItems.Count >= _maxBufferedItems) {
				D.Log($"{nameof(ItemMerger)} buffer is full.");
				return;
			}

			_bufferedItems.Add(item);
			D.Log($"{nameof(ItemMerger)} added {item.Name}({item.Id}). Buffered: {_bufferedItems.Count}");
		}

		public bool TryStartMergeProcess() {
			if (_isProcessing) {
				D.Log($"{nameof(ItemMerger)} is already processing.");
				return false;
			}

			if (_bufferedItems.Count == 0) {
				D.Log($"{nameof(ItemMerger)} buffer is empty.");
				return false;
			}

			var inputIds = _bufferedItems.Select(x => x.Id).ToArray();

			if (!_mergeSystem.TryGetMergedProductId(inputIds, out var outputItemId)) {
				D.Log($"{nameof(ItemMerger)} cannot merge current buffer. Recipe was not found.");
				return false;
			}

			_processingInputInfo = BuildProductsInfo(_bufferedItems);
			_processingOutputItem = _itemsFactory.Create(outputItemId);
			_bufferedItems.Clear();

			_remainingTime = _processingTimeSeconds;
			StartProcessing();
			return true;
		}

		private void Update() {
			if (!_isProcessing) {
				return;
			}

			_remainingTime -= Time.deltaTime;
			if (_remainingTime > 0f) {
				return;
			}

			CompleteProcessing();
		}

		private static string BuildProductsInfo(IReadOnlyList<Item> items) {
			var sb = new StringBuilder();
			for (var i = 0; i < items.Count; i++) {
				if (i > 0) {
					sb.Append(", ");
				}

				sb.Append(items[i].Name);
				sb.Append("(");
				sb.Append(items[i].Id);
				sb.Append(")");
			}

			return sb.ToString();
		}
		
		protected virtual void StartProcessing() {
			_isProcessing = true;
		}

		protected virtual void CompleteProcessing() {
			_isProcessing = false;
		}
	}
}
