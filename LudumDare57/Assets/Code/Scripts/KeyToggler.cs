using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyToggler : MonoBehaviour
{
    [SerializeField] private InputActionReference _toggleInput;
    [SerializeField] private Transform _keyOutAnchor;
    [SerializeField] private float _transitionDuration;
    [SerializeField] private Ease _transitionEase;

    private Tween _keyTween;
    private Vector3 _keyStoredPosition;
    private bool _keyIsOut;

    private void PutKeyOut()
    {
        if (_keyTween.IsActive())
        {
            _keyTween.Kill();
        }

        _keyTween = transform.DOLocalMove(_keyOutAnchor.localPosition, _transitionDuration).SetEase(_transitionEase);
        _keyIsOut = true;
    }

    private void StoreKey()
    {
        if (_keyTween.IsActive())
        {
            _keyTween.Kill();
        }

        _keyTween = transform.DOLocalMove(_keyStoredPosition, _transitionDuration).SetEase(_transitionEase);
        _keyIsOut = false;
    }

    private void OnToggleInputPerformed(InputAction.CallbackContext context)
    {
        if (_keyIsOut)
        {
            StoreKey();
        }
        else
        {
            PutKeyOut();
        }
    }

    private void Awake()
    {
        _keyStoredPosition = transform.localPosition;
        _keyIsOut = false;
        _toggleInput.action.performed += OnToggleInputPerformed;
    }
}
