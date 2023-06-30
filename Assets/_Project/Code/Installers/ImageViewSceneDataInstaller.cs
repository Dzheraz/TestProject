using UnityEngine;
using Zenject;

internal class ImageViewSceneDataInstaller : MonoInstaller
{
    [SerializeField]
    private ImageViewSceneData _instance;
    public override void InstallBindings()
    {
        Container.Bind<ImageViewSceneData>().FromInstance(_instance).NonLazy();
    }
}
