using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class KeyMatchFeedback : MonoBehaviour
{
    [SerializeField] private Renderer _keyRenderer;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _minScoreForFeedback;
    [SerializeField] private FullScreenPassRendererFeature _nearMatchScoreContrastPP;
    [SerializeField] private Volume _volume;
    [SerializeField] private VolumeProfile _defaultVolumeProfile;
    [SerializeField] private VolumeProfile _nearMatchVolumeProfile;
    [SerializeField] private float _matchFadeOutDuration;

    private MaterialPropertyBlock _keyblock;
    private KeyMatcher _currentMatcher;
    private float _currentIntensity;
    private VolumeProfileProperties _activeVolumeProfileProperties;
    private VolumeProfileProperties _defaultVolumeProfileProperties;
    private VolumeProfileProperties _nearMatchVolumeProfileProperties;

    public void SetFeedbackIntensity(float score, KeyMatcher matcher)
    {
        float newIntensity = Mathf.Clamp01(Mathf.InverseLerp(_minScoreForFeedback, 1f, score));

        if (matcher == _currentMatcher)
        {
            _currentIntensity = newIntensity;
        }
        else
        {
            if (newIntensity > _currentIntensity)
            {
                _currentMatcher = matcher;
                _currentIntensity = newIntensity;
            }
        }

        UpdateNearMatchFeedbacks();
    }

    public void PlayMatchFeedback(KeyMatcher matcher)
    {
        _animator.SetTrigger("match");

        DOTween.Sequence()
            .Join(_keyRenderer.material.DOFloat(0f, "_NearMatchIntensity", _matchFadeOutDuration))
            .Join(_nearMatchScoreContrastPP.passMaterial.DOFloat(0f, "_NearMatchIntensity", _matchFadeOutDuration))
            .Join(DOTween.To(() => _activeVolumeProfileProperties.Bloom.intensity.value, x => _activeVolumeProfileProperties.Bloom.intensity.value = x, _defaultVolumeProfileProperties.Bloom.intensity.value, _matchFadeOutDuration))
            .Join(DOTween.To(() => _activeVolumeProfileProperties.Vignette.intensity.value, x => _activeVolumeProfileProperties.Vignette.intensity.value = x, _defaultVolumeProfileProperties.Vignette.intensity.value, _matchFadeOutDuration))
            .Join(DOTween.To(() => _activeVolumeProfileProperties.LensDistortion.intensity.value, x => _activeVolumeProfileProperties.LensDistortion.intensity.value = x, _defaultVolumeProfileProperties.LensDistortion.intensity.value, _matchFadeOutDuration))
            .OnComplete(() => Game.KeyMatchFeedback.SetFeedbackIntensity(0f, matcher));
    }

    private void UpdateNearMatchFeedbacks()
    {
        _keyblock ??= new();
        _keyRenderer.GetPropertyBlock(_keyblock);
        _keyblock.SetFloat("_NearMatchIntensity", _currentIntensity);
        _keyRenderer.SetPropertyBlock(_keyblock);

        _nearMatchScoreContrastPP.passMaterial.SetFloat("_NearMatchIntensity", _currentIntensity);

        _activeVolumeProfileProperties.Bloom.intensity.value = Mathf.Lerp(_defaultVolumeProfileProperties.Bloom.intensity.value, _nearMatchVolumeProfileProperties.Bloom.intensity.value, _currentIntensity);
        _activeVolumeProfileProperties.Vignette.intensity.value = Mathf.Lerp(_defaultVolumeProfileProperties.Vignette.intensity.value, _nearMatchVolumeProfileProperties.Vignette.intensity.value, _currentIntensity);
        _activeVolumeProfileProperties.LensDistortion.intensity.value = Mathf.Lerp(_defaultVolumeProfileProperties.LensDistortion.intensity.value, _nearMatchVolumeProfileProperties.LensDistortion.intensity.value, _currentIntensity);
    }

    private void Awake()
    {
        _activeVolumeProfileProperties.Initialize(_volume.profile);
        _defaultVolumeProfileProperties.Initialize(_defaultVolumeProfile);
        _nearMatchVolumeProfileProperties.Initialize(_nearMatchVolumeProfile);
    }

    private struct VolumeProfileProperties
    {
        public Bloom Bloom;
        public Vignette Vignette;
        public LensDistortion LensDistortion;

        public void Initialize(VolumeProfile volumeProfile)
        {
            if (volumeProfile.TryGet(out Bloom bloom))
            {
                Bloom = bloom;
            }

            if (volumeProfile.TryGet(out Vignette vignette))
            {
                Vignette = vignette;
            }

            if (volumeProfile.TryGet(out LensDistortion lensDistortion))
            {
                LensDistortion = lensDistortion;
            }
        }
    }
}
