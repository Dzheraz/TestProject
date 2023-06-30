using UnityEngine;
using Zenject;

internal class SceneCarsListInstaller : MonoInstaller
{
    [SerializeField]
    private CarsList _carsList;
    public override void InstallBindings()
    {
        Container.Bind<CarsList>().FromInstance(_carsList).AsSingle().NonLazy();
    }
}
