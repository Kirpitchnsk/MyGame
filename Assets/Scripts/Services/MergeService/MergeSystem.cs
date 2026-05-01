using System.Collections.Generic;
using System.Linq;

namespace SibGameJam2026.MergeService {
	public class MergeSystem : IMergeSystem {
		private static readonly IReadOnlyList<int> EmptyProductList = new List<int>();
		private readonly RecipesDatabase _recipesDatabase;

		public MergeSystem(RecipesDatabase recipesDatabase) {
			_recipesDatabase = recipesDatabase;
		}

		public bool TryGetMergedProductId(IReadOnlyList<int> inputProductIds, out int outputProductId) {
			outputProductId = -1;

			if (inputProductIds == null || inputProductIds.Count == 0)
				return false;
			
			foreach (var recipe in _recipesDatabase.Recipes) {
				if (recipe == null)
					continue;

				if (!AreSameProductSet(recipe.InputItemIds, inputProductIds))
					continue;

				outputProductId = recipe.OutputItemId;
				return true;
			}

			return false;
		}

		public bool TryGetSourceProductIds(int outputProductId, out IReadOnlyList<int> sourceProductIds) {
			sourceProductIds = EmptyProductList;

			if (!_recipesDatabase.TryGetRecipeByOutputItemId(outputProductId, out var recipe)
			    || recipe == null)
				return false;
			
			sourceProductIds = recipe.InputItemIds.ToList();
			return true;
		}

		private static bool AreSameProductSet(IReadOnlyList<int> left, IReadOnlyList<int> right) {
			if (left == null || right == null)
				return false;

			if (left.Count != right.Count)
				return false;

			var counts = new Dictionary<int, int>();
			foreach (var id in left) {
				if (!counts.TryAdd(id, 1))
					counts[id]++;
			}

			foreach (var id in right) {
				if (!counts.TryGetValue(id, out var count))
					return false;

				if (count == 1)
					counts.Remove(id);
				else
					counts[id] = count - 1;
			}

			return counts.Count == 0;
		}
	}
}
