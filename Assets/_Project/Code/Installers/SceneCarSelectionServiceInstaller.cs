using UnityEngine;
using Zenject;

internal class SceneCarSelectionServiceInstaller : MonoInstaller
{
    [SerializeField]
    private CarSelectionService _service;

    public override void InstallBindings()
    {
        Container.Bind<CarSelectionService>().FromInstance(_service).AsSingle().NonLazy();
    }
}
