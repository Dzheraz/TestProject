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

    private bool _isLoading = false;
    public bool IsLoading => _isLoading;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private Tweener _showFakeLoadingTweener;
    public void ShowFakeLoading(float seconds)
    {
        _loadCanvas.Open(true);
        _loadCanvas.SetPercent(0f, false);
        _isLoading = true;
        _showFakeLoadingTweener?.Kill(true);
        _showFakeLoadingTweener = DOTween.To(t =>
        {
            _loadCanvas.SetPercent(t, false);
        }, 0f, 1f, seconds).OnComplete(() =>
        {
            _isLoading = false;
            _loadCanvas.Close(true);
        });
    }

    private const string ISLOADING_WARNING_TEXT = "Can't start a new scene while loading.";
    public void LoadMainMenu()
    {
        _imageViewSceneData.SetImage(null);
        if (IsLoading)
        {
            Debug.LogWarning(ISLOADING_WARNING_TEXT);
            return;
        }
        Screen.orientation = ScreenOrientation.Portrait;
        SceneManager.LoadScene(_mainMenuSceneName);
    }

    public void LoadGalllery()
    {
        _imageViewSceneData.SetImage(null);
        if (IsLoading)
        {
            Debug.LogWarning(ISLOADING_WARNING_TEXT);
            return;
        }
        StartCoroutine(LoadGallleryCoroutine());
    }
    private IEnumerator LoadGallleryCoroutine()
    {
        ShowFakeLoading(3f);
        yield return new WaitForSeconds(1f);
        Screen.orientation = ScreenOrientation.Portrait;
        SceneManager.LoadScene(_galllerySceneName);
    }

    public void LoadImageView(Sprite image)
    {
        if (IsLoading)
        {
            Debug.LogWarning(ISLOADING_WARNING_TEXT);
            return;
        }
        StartCoroutine(LoadImageViewCoroutine(image));
    }
    private IEnumerator LoadImageViewCoroutine(Sprite image)
    {
        ShowFakeLoading(3f);
        _imageViewSceneData.SetImage(image);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(_imageViewSceneName);
        Screen.orientation = ScreenOrientation.AutoRotation;
    }
}
