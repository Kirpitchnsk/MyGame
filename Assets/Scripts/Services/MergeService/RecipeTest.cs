using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Zenject;

namespace SibGameJam2026.MergeService {
	public class RecipeTest : IInitializable {
		private readonly IMergeSystem _mergeSystem;
		private readonly ItemsDatabase _itemsDatabase;
		private readonly IReadOnlyList<int> _inputIds = new  List<int>()
		{
			1, 3
		};

		public RecipeTest(
			IMergeSystem mergeSystem,
			ItemsDatabase itemsDatabase) {
			_mergeSystem = mergeSystem;
			_itemsDatabase = itemsDatabase;
		}

		public void Initialize() {
			if (_inputIds == null || _inputIds.Count == 0) {
				D.Log("RepiceTest: input ids are empty.");
				return;
			}

			var inputInfo = BuildProductsInfo(_inputIds);

			if (_mergeSystem.TryGetMergedProductId(_inputIds, out var outputProductId)) {
				var outputInfo = BuildProductInfo(outputProductId);
				D.Log($"RepiceTest success. Used: {inputInfo}. Result: {outputInfo}");
				return;
			}

			D.Log($"RepiceTest failed. Used: {inputInfo}. Result: -1 (no recipe found)");
		}

		private string BuildProductsInfo(IReadOnlyList<int> ids) {
			var sb = new StringBuilder();
			for (var i = 0; i < ids.Count; i++) {
				if (i > 0) {
					sb.Append(", ");
				}

				sb.Append(BuildProductInfo(ids[i]));
			}

			return sb.ToString();
		}

		private string BuildProductInfo(int id) {
			return _itemsDatabase.TryGetItemById(id, out var item)
				? $"{item.Name}({id})"
				: $"Unknown({id})";
		}
	}
}
