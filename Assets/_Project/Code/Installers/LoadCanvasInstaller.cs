using UnityEngine;
using Zenject;

namespace Assets
{
    internal class LoadCanvasInstaller : MonoInstaller
    {
        [SerializeField]
        private LoadCanvas _loadCanvasPrefab;

        public override void InstallBindings()
        {
            LoadCanvas instance = Container.InstantiatePrefabForComponent<LoadCanvas>(_loadCanvasPrefab);
            instance.Close(false);
            DontDestroyOnLoad(instance.gameObject);
            Container.Bind<LoadCanvas>().FromInstance(instance).AsSingle().NonLazy();

            // Container.QueueForInject();
        }
    }
}
