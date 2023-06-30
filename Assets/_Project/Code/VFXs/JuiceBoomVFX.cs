using DCFApixels;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class JuiceBoomVFX : MonoBehaviour
{
    [SerializeField]
    private float _time;
    [SerializeField]
    private ObjectPoolUnit _poolUnit;
    [SerializeField]
    private VisualEffect _effect;
    private Coroutine _playCoroutine;
    private static readonly int _colorPropId = Shader.PropertyToID("Color");
    public void Play(Vector3 position, Color color)
    {
        transform.position = position;
        gameObject.SetActive(true);
        _effect.SetVector4(_colorPropId, new Vector4(color.r, color.g, color.b, 1f));
        _effect.Play();
        _playCoroutine = StartCoroutine(PlayCoroutine());
    }

    public void Stop()
    {
        _poolUnit.ReturnToPool();
        if (_playCoroutine != null)
        {
            StopCoroutine(_playCoroutine);
            _playCoroutine = null;
        }
    }

    private IEnumerator PlayCoroutine()
    {
        yield return new WaitForSeconds(_time);
        _playCoroutine = null;
        Stop();
    }
}
