using UnityEngine;
using Zenject;

internal class WebGalleryServiceInstaller : MonoInstaller
{
    [SerializeField]
    private WebGalleryService _service;

    public override void InstallBindings()
    {
        Container.Bind<WebGalleryService>().FromInstance(_service).AsSingle().NonLazy();
    }
}
