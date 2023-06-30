using UnityEngine;
using UnityEngine.UI;
using Zenject;

internal class MainMenuScreen : MonoBehaviour
{
    [Inject]
    private AppSceneLoader _sceneLoader;
    [SerializeField]
    private Button _galleyButton;

    private void OnEnable()
    {
        _galleyButton.onClick.AddListener(OnGalleryButtonClicked);
    }
    private void OnDisable()
    {
        _galleyButton.onClick.RemoveListener(OnGalleryButtonClicked);
    }

    private void OnGalleryButtonClicked()
    {
        _sceneLoader.LoadGalllery();
    }
}
