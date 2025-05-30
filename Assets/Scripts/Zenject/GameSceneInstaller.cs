using UnityEngine;
using Zenject;

namespace Injection
{
    public class GameSceneInstaller : MonoInstaller<GameSceneInstaller>
    {
        [SerializeField] private InputController inputController;
        [SerializeField] private CharacterMovementController characterMovementController;
        [SerializeField] private ObjectPool objectPool;
        [SerializeField] private PlatformSpawner platformController;
        [SerializeField] private PlatformPlacer platformPlacer;
        [SerializeField] private GameController gameController;
        [SerializeField] private FinishController finishController;
        [SerializeField] private PlatformColor platformColor;

        public override void InstallBindings()
        {
            //Signals
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<TapEvent>();
            Container.DeclareSignal<GameStartedEvent>();
            Container.DeclareSignal<GameOverEvent>();
            Container.DeclareSignal<GameSuccessEvent>();
            Container.DeclareSignal<PerfectPlacementEvent>();
            Container.DeclareSignal<NormalPlacementEvent>();
            Container.DeclareSignal<StopMovingPlatformEvent>();
            Container.DeclareSignal<LevelInitializedEvent>();



            //Bindings
            Container.Bind<InputController>().FromInstance(inputController).AsSingle();
            Container.Bind<CharacterMovementController>().FromInstance(characterMovementController).AsSingle();
            Container.Bind<ObjectPool>().FromInstance(objectPool).AsSingle();
            Container.Bind<PlatformSpawner>().FromInstance(platformController).AsSingle();
            Container.Bind<GameController>().FromInstance(gameController).AsSingle();
            Container.Bind<PlatformPlacer>().FromInstance(platformPlacer).AsSingle();
            Container.Bind<FinishController>().FromInstance(finishController).AsSingle();
            Container.Bind<PlatformColor>().FromInstance(platformColor).AsSingle();
        }
    }
}