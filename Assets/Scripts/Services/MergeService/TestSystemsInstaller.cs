using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace SibGameJam2026.MergeService {
	public class TestSystemsInstaller : MonoInstaller {
		[SerializeField] private List<int> _repiceTestInputIds = new();

		public override void InstallBindings() {
			#if UNITY_EDITOR
			TestRecipe();
			#endif
		}

		private void TestRecipe()
		{
			Container.Bind<int[]>()
				.WithId("RecipeTestInputIds")
				.FromInstance(_repiceTestInputIds.ToArray())
				.AsSingle();

			Container.BindInterfacesAndSelfTo<RecipeTest>().AsSingle().NonLazy();
		}
	}
}
