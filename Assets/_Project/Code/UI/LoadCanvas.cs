using DCFApixels;
using DG.Tweening;
using UnityEngine;

public class LoadCanvas : MonoBehaviour
{
    [SerializeField]
    private FloatProgressBarUI _progressBar;
    [SerializeField]
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private float _fadeDuration = 0.1f;
    public void Open(bool isAnimated)
    {
        _canvasGroup.DOKill();
        _canvasGroup.gameObject.SetActive(true);
        if (!isAnimated)
        {
            _canvasGroup.alpha = 1f;
            return;
        }

        _canvasGroup.alpha = 0f;
        //_canvasGroup.interactable = true;
        _canvasGroup.DOFade(1f, _fadeDuration);
    }
    public void Close(bool isAnimated)
    {
        _canvasGroup.DOKill();
        if (!isAnimated)
        {
            _canvasGroup.gameObject.SetActive(false);
            return;
        }
        // _canvasGroup.DOFade(0f, _fadeDuration).OnComplete(() =>_canvasGroup.interactable = false);
        _canvasGroup.DOFade(0f, _fadeDuration).OnComplete(() => _canvasGroup.gameObject.SetActive(false));
    }

    public void SetPercent(float percent, bool isAnimated)
    {
        _progressBar.SetValue(percent, isAnimated);
    }
    public void ResetPercent()
    {
        _progressBar.SetValue(0, false);
    }
}
