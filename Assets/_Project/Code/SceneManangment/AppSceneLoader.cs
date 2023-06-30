using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class AppSceneLoader : MonoBehaviour
{
    [Inject]
    private LoadCanvas _loadCanvas;
    [Inject]
    private ImageViewSceneData _imageViewSceneData;

    [SerializeField]
    private string _mainMenuSceneName = "MainMenu";
    [SerializeField]
    private string _galllerySceneName = "Galllery";
    [SerializeField]
    private string _imageViewSceneName = "ImageView";

    private Sprite _imageViewImage;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private Tweener _showFakeLoadingTweener;
    public void ShowFakeLoading(float seconds)
    {
        _loadCanvas.Open(true);
        _loadCanvas.SetPercent(0f, false);

        _showFakeLoadingTweener?.Kill(true);
        _showFakeLoadingTweener = DOTween.To(t =>
        {
            _loadCanvas.SetPercent(t, true);
        }, 0f, 1f, seconds).OnComplete(() => _loadCanvas.Close(true));
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(_mainMenuSceneName);
        Screen.orientation = ScreenOrientation.Portrait;
    }

    public void LoadGalllery()
    {
        SceneManager.LoadScene(_galllerySceneName);
        Screen.orientation = ScreenOrientation.Portrait;
    }

    public void LoadImageView(Sprite image)
    {
        _imageViewImage = image;
        SceneManager.LoadScene(_imageViewSceneName);
        Screen.orientation = ScreenOrientation.AutoRotation;
    }

    public Sprite GetImageViewImage()
    {
        return _imageViewImage;
    }
}
