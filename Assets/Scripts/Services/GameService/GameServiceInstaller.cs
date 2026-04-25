using Zenject;

namespace SibGameJam2026.Services {
	public class GameServiceInstaller : MonoInstaller {
		public override void InstallBindings() {
			Container.Bind<IGameService>().To<GameService>().AsSingle();
		}
	}
}