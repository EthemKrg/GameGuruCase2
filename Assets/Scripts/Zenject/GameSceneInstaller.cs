using UnityEngine;
using Zenject;

namespace Injection
{
    public class GameSceneInstaller : MonoInstaller<GameSceneInstaller>
    {
        [SerializeField] private InputController inputController;
        [SerializeField] private CharacterMovementController characterMovementController;

        public override void InstallBindings()
        {
            //Signals
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<TapEvent>();

            //Bindings
            Container.Bind<InputController>().FromInstance(inputController).AsSingle();
            Container.Bind<CharacterMovementController>().FromInstance(characterMovementController).AsSingle();
        }
    }
}