using DCFApixels.UI.Internal;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DCFApixels
{
    [ExecuteAlways]
    public class FloatProgressBarUI : MonoBehaviour
    {
        [SerializeField]
        private bool _executeAlwaysRuntime = false;

        [SerializeReference, SerializeReferenceButton]
        private IProgressBarBehaviour _behaviour;
        public bool TryGetBehaivour<T>(out T behaviour) where T : class, IProgressBarBehaviour
        {
            if (_behaviour is T t)
            {
                behaviour = t;
                return true;
            }
            behaviour = null;
            return false;
        }
        [SerializeField]
        private RangeFloat _range = RangeFloat.one;
        public RangeFloat Range => _range;
        [SerializeField]
        private float _value;
        public float Value => _value;
        [SerializeField, NullableProperty]
        private SerializableNullable<Color> _color;
        public Color? Color => _color;
        [SerializeField]
        private bool _isInverce;
        public bool IsInverce => _isInverce;

        [ContextMenu("SetRefs")]
        private void SetRefs()
        {
            if (_behaviour is IProgressBarBehaviourSetRefs behaiour)
                behaiour.OnSetRefs(this);
        }
        private bool IsValide()
        {
            return _behaviour != null && _behaviour.IsValide;
        }

        public void SetRange(float max, bool isAnimated)
        {
            _range = new RangeFloat(0, max);
            SetValue(_value, isAnimated);
        }
        public void SetRange(RangeFloat range, bool isAnimated)
        {
            _range = range;
            SetValue(_value, isAnimated);
        }
        public void SetValue(float value, bool isAnimated)
        {
            _value = value;
            if (IsValide() == false)
                return;
            _behaviour.SetFillAmount(this, _range.PercentClamp(_value), isAnimated);
        }
        public void SetColor(Color? color, bool isAnimated)
        {
            _color = color;
            if (IsValide() == false)
                return;
            if (_color != null)
                _behaviour?.SetColor(this, _color.Value, false);
        }

        public float FiilAmount => _behaviour == null ? 0f : _behaviour.FillAmount;

        #region UnityEvents
        private void Awake()
        {
            if (IsValide() == false)
                return;
            if (_behaviour is IProgressBarBehaviourInit behaior)
                behaior.Init(this);
        }

        private void OnEnable()
        {
            if (IsValide() == false)
                return;
            if (_behaviour is IProgressBarBehaviourEnable behaior)
                behaior.OnEnable(this);
            _behaviour?.SetFillAmount(this, _range.PercentClamp(_value), false);
            if (_color != null)
                _behaviour?.SetColor(this, _color.Value, false);
        }

        private void OnDisable()
        {
            if (IsValide() == false)
                return;
            if (_behaviour is IProgressBarBehaviourDisable behaior)
                behaior.OnDisable(this);
        }

        private void Update()
        {
            if (IsValide() == false)
                return;
            if (Application.isPlaying == false || _executeAlwaysRuntime)
            {
                _behaviour?.SetFillAmount(this, _range.PercentClamp(_value), false);
                if (_color != null)
                    _behaviour?.SetColor(this, _color.Value, false);
            }

            if (_behaviour is IProgressBarBehaviourUpdate behaior)
                behaior.Update(this, Time.deltaTime);
        }
        #endregion
    }

    public abstract class SubProgressBar : MonoBehaviour
    {
        public abstract void SetFillAmount(int index, SubProgressBarBehavior source, float value, bool isAnimated);
        public abstract void SetRefs(SubProgressBarBehavior source);
    }
}

namespace DCFApixels.UI.Internal
{
    public interface IProgressBarBehaviour
    {
        public bool IsValide { get; }
        public float FillAmount { get; }
        public void SetFillAmount(FloatProgressBarUI source, float value, bool isAnimated);
        public void SetColor(FloatProgressBarUI source, Color color, bool isAnimated);
    }

    public interface IProgressBarBehaviourInit : IProgressBarBehaviour
    {
        public void Init(FloatProgressBarUI source);
    }
    public interface IProgressBarBehaviourUpdate : IProgressBarBehaviour
    {
        public void Update(FloatProgressBarUI source, float deltaTime);
    }
    public interface IProgressBarBehaviourEnable : IProgressBarBehaviour
    {
        public void OnEnable(FloatProgressBarUI source);
    }
    public interface IProgressBarBehaviourDisable : IProgressBarBehaviour
    {
        public void OnDisable(FloatProgressBarUI source);
    }

    public interface IProgressBarBehaviourSetRefs : IProgressBarBehaviour
    {
        public void OnSetRefs(FloatProgressBarUI source);
    }

    [System.Serializable]
    public class FillImageProgressBarBehavior : IProgressBarBehaviour
    {
        [SerializeField]
        private Image _slider;

        [SerializeField]
        private float _animationDuration = 0.1f;
        [SerializeField]
        private Ease _ease = Ease.Linear;

        private float _fiilAmount = 0f;
        float IProgressBarBehaviour.FillAmount => _fiilAmount;

        bool IProgressBarBehaviour.IsValide => _slider != null;

        private Tween m_Tween;

        void IProgressBarBehaviour.SetFillAmount(FloatProgressBarUI source, float value, bool isAnimated)
        {
            _fiilAmount = source.IsInverce ? 1f - value : value;

            if (isAnimated == false)
            {
                _slider.fillAmount = _fiilAmount;
                return;
            }

            m_Tween?.Kill();
            m_Tween = DOTween.To(t =>
            {
                Apply(t);
            }, _slider.fillAmount, _fiilAmount, _animationDuration).SetEase(_ease);
        }

        private void Apply(float i_Value)
        {
            _slider.fillAmount = i_Value;
        }

        void IProgressBarBehaviour.SetColor(FloatProgressBarUI source, Color color, bool isAnimated)
        {
        }
    }

    [System.Serializable]
    public class TextProgressBarBehavior : IProgressBarBehaviour, IProgressBarBehaviourSetRefs
    {
        [SerializeField]
        private TMP_Text _text;
        [SerializeField]
        private RangeFloat _valueRange = RangeFloat.MinMax(0f, 100f);
        [SerializeField]
        private string _stringFormat = "{0:F2}";

        private float _fillAmount = 0f;

        bool IProgressBarBehaviour.IsValide => _text != null;
        float IProgressBarBehaviour.FillAmount => _fillAmount;

        void IProgressBarBehaviourSetRefs.OnSetRefs(FloatProgressBarUI source) => _text = source.GetComponentInChildren<TMP_Text>(true);
        void IProgressBarBehaviour.SetColor(FloatProgressBarUI source, Color color, bool isAnimated) { }

        void IProgressBarBehaviour.SetFillAmount(FloatProgressBarUI source, float value, bool isAnimated)
        {
            _fillAmount = value;
            Apply();
        }

        private void Apply()
        {
            _text.text = string.Format(_stringFormat, _valueRange.Lerp(_fillAmount));
        }

    }

    [System.Serializable]
    public class SliderProgressBarBehavior : IProgressBarBehaviour
    {
        [SerializeField]
        private Slider _slider;
        [SerializeField]
        private float _animationDuration = 0.1f;
        [SerializeField]
        private Ease _ease = Ease.Linear;
        [SerializeField]
        private bool _isTimeScaled = true;

        private float _fiilAmount = 0f;

        float IProgressBarBehaviour.FillAmount => _fiilAmount;
        bool IProgressBarBehaviour.IsValide => _slider != null;

        private Tween _tween;

        void IProgressBarBehaviour.SetFillAmount(FloatProgressBarUI source, float value, bool isAnimated)
        {
            _fiilAmount = source.IsInverce ? 1f - value : value;

            if (isAnimated == false)
            {
                _slider.value = _fiilAmount;
                return;
            }

            _tween?.Kill();
            _tween = DOTween.To(t =>
            {
                Apply(t);
            }, _slider.value, _fiilAmount, _animationDuration).SetEase(_ease).SetUpdate(!_isTimeScaled);
        }

        private void Apply(float value)
        {
            _slider.value = value;
        }

        void IProgressBarBehaviour.SetColor(FloatProgressBarUI source, Color color, bool isAnimated)
        {
        }
    }

    [System.Serializable]
    public class CircleProgressBarBehavior : IProgressBarBehaviour
    {
        [SerializeField]
        private Image _iamge;
        [SerializeField]
        private Transform _headImageTransform;
        [SerializeField]
        private Transform _tailImageTransform;
        [SerializeField]
        private float _radius = 1f;

        [SerializeField]
        private float _animationDuration = 0.1f;
        [SerializeField]
        private Ease _ease = Ease.Linear;

        private float _fiilAmount = 0f;
        float IProgressBarBehaviour.FillAmount => _fiilAmount;

        bool IProgressBarBehaviour.IsValide => _iamge != null;

        [SerializeField, ClampedRange]
        private RangeFloat _offset = RangeFloat.one;

        private Tween _tween;

        void IProgressBarBehaviour.SetFillAmount(FloatProgressBarUI source, float value, bool isAnimated)
        {
            _fiilAmount = source.IsInverce ? 1f - value : value;

            if (isAnimated == false)
            {
                Apply(_fiilAmount);
                return;
            }

            _tween?.Kill();
            _tween = DOTween.To(t =>
            {
                Apply(t);
            }, _iamge.fillAmount, _fiilAmount, _animationDuration).SetEase(_ease);
        }

        private void Apply(float value)
        {
            float v = _offset.Lerp(value);
            _iamge.fillAmount = v;

            Quaternion rot;
            Vector3 pos;

            if (_headImageTransform == null || _tailImageTransform == null)
                return;

            pos = (-Vector3.up * _radius);
            _headImageTransform.localPosition = pos;

            rot = Quaternion.Euler(0f, 0f, (1f - v) * 360f);
            pos = rot * (-Vector3.up * _radius);
            _tailImageTransform.localPosition = pos;
        }

        void IProgressBarBehaviour.SetColor(FloatProgressBarUI source, Color color, bool isAnimated)
        {
            _iamge.color = color;
        }
    }

    [System.Serializable]
    public class CellsProgressBarBehavior : IProgressBarBehaviour
    {
        private float _fiilAmount = 0f;

        [SerializeField]
        private Sprite _spriteOn;
        [SerializeField]
        private Sprite _spriteOff;
        [SerializeField]
        private Color _colorOn = Color.white;
        [SerializeField]
        private Color _colorOff = Color.white;
        [SerializeField]
        private ParticleSystem _particleVFX;

        [SerializeField]
        private Image[] _images;

        bool IProgressBarBehaviour.IsValide => true;
        float IProgressBarBehaviour.FillAmount => _fiilAmount;

        void IProgressBarBehaviour.SetColor(FloatProgressBarUI source, Color color, bool isAnimated)
        {
        }

        void IProgressBarBehaviour.SetFillAmount(FloatProgressBarUI source, float value, bool isAnimated)
        {
            for (int i = 0; i < _images.Length; i++)
            {
                _images[i].gameObject.SetActive(source.Range.length > i);
            }


            for (int i = 0; i < _images.Length; i++)
            {
                bool result = ((value > 0 && ((float)i / (source.Range.length - 0.01f)) < value) ^ source.IsInverce);

                var oldSprite = _images[i].sprite;
                _images[i].sprite = result ? _spriteOn : _spriteOff;

                if (isAnimated && _particleVFX != null && i < source.Range.Max && oldSprite != _images[i].sprite && _images[i].sprite == _spriteOff)
                {
                    _particleVFX.transform.position = _images[i].transform.position;
                    _particleVFX.Stop();
                    _particleVFX.Play();
                }

                _images[i].color = result ? _colorOn : _colorOff;
            }
        }
    }

    [System.Serializable]
    public class SubProgressBarBehavior : IProgressBarBehaviour, IProgressBarBehaviourSetRefs
    {
        private float _fiilAmount = 0f;

        [SerializeField]
        private SubProgressBar[] _subBars;

        bool IProgressBarBehaviour.IsValide => true;
        float IProgressBarBehaviour.FillAmount => _fiilAmount;

        private FloatProgressBarUI _source;
        public FloatProgressBarUI Source => _source;

        void IProgressBarBehaviour.SetColor(FloatProgressBarUI source, Color color, bool isAnimated) { }
        void IProgressBarBehaviour.SetFillAmount(FloatProgressBarUI source, float value, bool isAnimated)
        {
            if (_source == null)
            {
                _source = source;
            }
            for (int i = 0; i < _subBars.Length; i++)
            {
                _subBars[i].gameObject.SetActive(source.Range.length > i);
            }

            for (int i = 0; i < _subBars.Length; i++)
            {
                _subBars[i].SetFillAmount(i, this, value, isAnimated);
            }
        }

        void IProgressBarBehaviourSetRefs.OnSetRefs(FloatProgressBarUI source)
        {
            _subBars = _source.GetComponentsInChildren<SubProgressBar>(true);
        }
    }

    [System.Serializable]
    public class MultiProgressBarBehavior : IProgressBarBehaviour,
        IProgressBarBehaviourSetRefs,
        IProgressBarBehaviourDisable,
        IProgressBarBehaviourEnable,
        IProgressBarBehaviourInit,
        IProgressBarBehaviourUpdate
    {
        [SerializeReference, SerializeReferenceButton]
        private IProgressBarBehaviour[] _behaviours;

        bool IProgressBarBehaviour.IsValide
        {
            get
            {
                foreach (var item in _behaviours)
                {
                    if (item.IsValide == false)
                        return false;
                }
                return true;
            }
        }
        float IProgressBarBehaviour.FillAmount => _behaviours.Length > 0 ? _behaviours[0].FillAmount : 0f;

        void IProgressBarBehaviourInit.Init(FloatProgressBarUI source)
        {
            foreach (var item in _behaviours)
                if (item is IProgressBarBehaviourInit target)
                    target.Init(source);
        }
        void IProgressBarBehaviourDisable.OnDisable(FloatProgressBarUI source)
        {
            foreach (var item in _behaviours)
                if (item is IProgressBarBehaviourDisable target)
                    target.OnDisable(source);
        }
        void IProgressBarBehaviourEnable.OnEnable(FloatProgressBarUI source)
        {
            foreach (var item in _behaviours)
                if (item is IProgressBarBehaviourEnable target)
                    target.OnEnable(source);
        }
        void IProgressBarBehaviourSetRefs.OnSetRefs(FloatProgressBarUI source)
        {
            foreach (var item in _behaviours)
                if (item is IProgressBarBehaviourSetRefs target)
                    target.OnSetRefs(source);
        }
        void IProgressBarBehaviour.SetColor(FloatProgressBarUI source, Color color, bool isAnimated)
        {
            foreach (var item in _behaviours)
                item.SetColor(source, color, isAnimated);
        }
        void IProgressBarBehaviour.SetFillAmount(FloatProgressBarUI source, float value, bool isAnimated)
        {
            foreach (var item in _behaviours)
                item.SetFillAmount(source, value, isAnimated);
        }
        void IProgressBarBehaviourUpdate.Update(FloatProgressBarUI source, float deltaTime)
        {
            foreach (var item in _behaviours)
                if (item is IProgressBarBehaviourUpdate target)
                    target.Update(source, deltaTime);
        }
    }

    [System.Serializable]
    public class FadeImageProgressBarBehavior : IProgressBarBehaviour
    {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private float _animationDuration = 0.1f;
        [SerializeField]
        private Ease _ease = Ease.Linear;
        [SerializeField]
        private AnimationCurve _curve;

        private float _fillAmount = 0f;
        float IProgressBarBehaviour.FillAmount => _fillAmount;
        bool IProgressBarBehaviour.IsValide => _image != null;

        private Tween m_Tween;



        private void ApplyAlpha(float a)
        {
            Color c = _image.color;
            c.a = _curve.Evaluate(a);
            _image.color = c;
        }

        void IProgressBarBehaviour.SetFillAmount(FloatProgressBarUI source, float value, bool isAnimated)
        {
            _fillAmount = source.IsInverce ? 1f - value : value;

            if (isAnimated == false)
            {
                ApplyAlpha(_fillAmount);
                return;
            }

            m_Tween?.Kill();
            m_Tween = DOTween.To(t =>
            {
                ApplyAlpha(t);
            }, _image.fillAmount, _fillAmount, _animationDuration).SetEase(_ease);
        }

        void IProgressBarBehaviour.SetColor(FloatProgressBarUI source, Color color, bool isAnimated)
        {
        }
    }
}