using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private float _playerDetectionDelay;
    [SerializeField] private float _height;
    [SerializeField] private float _metersPerSecond;
    [SerializeField] private Ease _easeType;
    [SerializeField] private Collider _collider;
    [SerializeField] private float _minimumCameraAngleToTrigger;
    [SerializeField] private List<Collider> _walls;
    [SerializeField] private float _wallFadeDuration;

    float _playerTimer;
    bool _playerIsInElevator;
    bool _lastCameraAngleIsUp;
    bool _isMoving;

    private void MoveUp()
    {
        _isMoving = true;
        SetWallCollidersEnabled(true);

        foreach (Collider wall in _walls)
        {
            wall.GetComponent<Renderer>().material.SetFloat("_DirectionFeedback_OpacityFactor", 0f);
        }

        DOTween.Sequence()
            .Join(transform.DOLocalMoveY(transform.position.y + _height, _height / _metersPerSecond).SetEase(_easeType))
            .Join(Game.Player.DOLocalMoveY(Game.Player.position.y + _height, _height / _metersPerSecond).SetEase(_easeType))
            .OnComplete(Stop);
    }

    private void MoveDown()
    {
        _isMoving = true;
        SetWallCollidersEnabled(true);

        foreach (Collider wall in _walls)
        {
            wall.GetComponent<Renderer>().material.SetFloat("_DirectionFeedback_OpacityFactor", 0f);
        }

        DOTween.Sequence()
            .Join(transform.DOLocalMoveY(transform.position.y - _height, _height / _metersPerSecond).SetEase(_easeType))
            .Join(Game.Player.DOLocalMoveY(Game.Player.position.y - _height, _height / _metersPerSecond).SetEase(_easeType))
            .OnComplete(Stop);
    }

    private void Stop()
    {
        _isMoving = false;
        SetWallCollidersEnabled(false);

        foreach (Collider wall in _walls)
        {
            wall.GetComponent<Renderer>().material.SetFloat("_DirectionFeedback_OpacityFactor", 1f);
        }
    }

    private void CheckPlayerPosition()
    {
        if (_isMoving)
        {
            return;
        }

        if (Game.Player.position.x > _collider.bounds.min.x && Game.Player.position.x < _collider.bounds.max.x
                && Game.Player.position.z > _collider.bounds.min.z && Game.Player.position.z < _collider.bounds.max.z)
        {
            if (!_playerIsInElevator)
            {
                _playerIsInElevator = true;
                SetWallsVisible(true);
            }

            bool isLookingUp = Game.Camera.localEulerAngles.x > 180f && Game.Camera.localEulerAngles.x < 360f - _minimumCameraAngleToTrigger;
            bool isLookingDown = Game.Camera.localEulerAngles.x < 180f && Game.Camera.localEulerAngles.x > _minimumCameraAngleToTrigger;

            if (isLookingDown || isLookingUp)
            {
                if (isLookingDown && _lastCameraAngleIsUp)
                {
                    _playerTimer = 0;
                }

                _playerTimer += Time.deltaTime;

                if (_playerTimer > _playerDetectionDelay)
                {
                    if (isLookingDown)
                    {
                        MoveDown();
                    }
                    else if (isLookingUp)
                    {
                        MoveUp();
                    }

                    _playerTimer = 0f;
                }
            }
            else
            {
                _playerTimer = 0f;
            }

            _lastCameraAngleIsUp = isLookingUp;
        }
        else if (_playerIsInElevator)
        {
            _playerIsInElevator = false;
            _playerTimer = 0f;
            SetWallsVisible(false);
        }
    }

    private void SetWallsVisible(bool active)
    {
        foreach (Collider wall in _walls)
        {
            Tween tween = wall.GetComponent<Renderer>().material.DOFloat(active ? 1f : 0f, "_DirectionFeedback_OpacityFactor", _wallFadeDuration).SetEase(Ease.Linear);

            if (active)
            {
                wall.gameObject.SetActive(true);
            }
            else
            {
                tween.OnComplete(() => wall.gameObject.SetActive(false));
            }
        }
    }

    private void SetWallCollidersEnabled(bool enabled)
    {
        foreach (Collider wall in _walls)
        {
            Tween tween = wall.GetComponent<Renderer>().material.DOFloat(enabled ? 1f : 0f, "_OpacityFactor", _wallFadeDuration);

            if (enabled)
            {
                wall.enabled = true;
            }
            else
            {
                tween.OnComplete(() => wall.enabled = false);
            }
        }
    }

    private void Awake()
    {
        SetWallsVisible(false);
        SetWallCollidersEnabled(false);

        foreach (Collider wall in _walls)
        {
            Material material = wall.GetComponent<Renderer>().material;
            material.SetFloat("_TriggeringCameraAngle", _minimumCameraAngleToTrigger);
            material.SetFloat("_OpacityFactor", 0f);
            material.SetFloat("_DirectionFeedback_OpacityFactor", 0f);
        }
    }

    private void Update()
    {
        CheckPlayerPosition();
    }
}
