using UnityEngine;
using Zenject;

internal class AppSceneLoaderInstaller : MonoInstaller
{
    [SerializeField]
    private AppSceneLoader _appSceneLoader;
    public override void InstallBindings()
    {
        AppSceneLoader instance = Container.InstantiatePrefabForComponent<AppSceneLoader>(_appSceneLoader);
        DontDestroyOnLoad(instance.gameObject);
        Container.Bind<AppSceneLoader>().FromInstance(instance).AsSingle().NonLazy();
    }
}
