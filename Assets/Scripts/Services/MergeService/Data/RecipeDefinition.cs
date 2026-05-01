using System.Collections.Generic;
using UnityEngine;

namespace SibGameJam2026.MergeService {
	[CreateAssetMenu(fileName = "RecipeDefinition", menuName = "Data/RecipeDefinition", order = 52)]
	public class RecipeDefinition : ScriptableObject {
		[SerializeField] private string _recipeId;
		[SerializeField] private List<int> _inputItemIds = new();
		[SerializeField] private int _outputItemId;

		public string RecipeId => _recipeId;
		public IReadOnlyList<int> InputItemIds => _inputItemIds;
		public int OutputItemId => _outputItemId;
	}
}
