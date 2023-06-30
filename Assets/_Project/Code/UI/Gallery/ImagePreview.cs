using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

internal class ImagePreview : MonoBehaviour, IPointerClickHandler
{
    [Inject]
    private WebGalleryService _service;
    [Inject]
    private AppSceneLoader _loader;
    [SerializeField]
    private Image _image;

    private bool _isLoaded = false;
    private int _index;

    public void SetIndex(int index)
    {
        _index = index;

        _service.RequestImage(index, (s) =>
        {
            _isLoaded = true;
            _image.sprite = s;
        });
    }
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (_isLoaded == false)
            return;

        _loader.LoadImageView(_image.sprite);
    }
}
