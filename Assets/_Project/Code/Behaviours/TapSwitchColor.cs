using DCFApixels;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TapSwitchColor : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Gradient _gradient;
    [SerializeField]
    private ObjectPoolRef _juseVfxPool;
    [SerializeField]
    private Renderer _renderer;

    [SerializeField]
    private float _animDuration = 0.1f;

    private static readonly int _colorPropId = Shader.PropertyToID("_BaseColor");


    private MaterialPropertyBlock _materialPropertyBlock;

    private void Start()
    {
        _materialPropertyBlock = new MaterialPropertyBlock();
        _renderer.GetPropertyBlock(_materialPropertyBlock);
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        _renderer.GetPropertyBlock(_materialPropertyBlock);

        Color oldColor = _materialPropertyBlock.GetColor(_colorPropId);
        Color newCOlor = _gradient.Evaluate(Random.value);

        _materialPropertyBlock.SetColor(_colorPropId, newCOlor);
        _renderer.SetPropertyBlock(_materialPropertyBlock);

        transform.DOKill();
        transform.DOPunchScale(Vector3.one * 0.4f, _animDuration);

        if (_juseVfxPool.TryTake(out JuiceBoomVFX vfx))
        {
            vfx.Play(transform.position, newCOlor);
        }
    }
}
