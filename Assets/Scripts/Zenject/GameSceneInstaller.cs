using UnityEngine;
using Zenject;

namespace Injection
{
    public class GameSceneInstaller : MonoInstaller<GameSceneInstaller>
    {
        //[SerializeField] private WallsController wallsController;

        public override void InstallBindings()
        {
            //Signals
            SignalBusInstaller.Install(Container);
            //Container.DeclareSignal<DragEvent>();

            //Bindings
            //Container.Bind<ColorData>().FromInstance(colorData).AsSingle();
        }
    }
}