using UnityEngine;
using UnityEngine.UI;
using Zenject;

internal class ImageViewScreen : MonoBehaviour
{
    [Inject]
    private ImageViewSceneData _sceneData;
    [Inject]
    private AppSceneLoader _sceneLoader;

    [SerializeField]
    private Button _closeButton;
    [SerializeField]
    private Image _image;

    private void OnEnable()
    {
        _closeButton.onClick.AddListener(OnCloseButtonClicked);
        _image.sprite = _sceneData.Image;
    }
    private void OnDisable()
    {
        _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
    }

    private void OnCloseButtonClicked()
    {
        _sceneLoader.LoadGalllery();
    }
}
