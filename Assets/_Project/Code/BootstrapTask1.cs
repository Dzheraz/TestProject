using UnityEngine;
using Zenject;

public class BootstrapTask1 : MonoBehaviour
{
    [Inject]
    private AppSceneLoader _loader;
    private void Start()
    {
        _loader.LoadMainMenu();
    }
}
