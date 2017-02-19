using Zenject;

namespace World
{
    public class WorldInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IInitializable>().To<WorldInitializer>().AsSingle();
        }
    }
}