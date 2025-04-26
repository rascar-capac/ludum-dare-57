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

    private float _playerTimer;
    private bool _playerIsInElevator;
    private bool _wasLookingUp;
    private bool _wasLookingDown;
    private bool _isMoving;
    private readonly List<Material> _wallMaterials = new();

    private void MoveUp()
    {
        _isMoving = true;
        SetWallCollidersEnabled(true);

        foreach (Material wall in _wallMaterials)
        {
            wall.SetFloat("_DirectionFeedback_OpacityFactor", 0f);
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

        foreach (Material wall in _wallMaterials)
        {
            wall.SetFloat("_DirectionFeedback_OpacityFactor", 0f);
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

        foreach (Material wall in _wallMaterials)
        {
            wall.SetFloat("_DirectionFeedback_OpacityFactor", 1f);
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

            if (isLookingUp && !_wasLookingUp || isLookingDown && !_wasLookingDown)
            {
                _playerTimer = 0f;

                foreach (Material material in _wallMaterials)
                {
                    material.SetFloat("_DirectionFeedback_FlashStartTime", Time.time);
                }
            }

            if (isLookingDown || isLookingUp)
            {
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

            _wasLookingUp = isLookingUp;
            _wasLookingDown = isLookingDown;
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
        Sequence sequence = DOTween.Sequence();

        for (int wallIndex = 0; wallIndex < _walls.Count; wallIndex++)
        {
            Tween tween = _wallMaterials[wallIndex].DOFloat(active ? 1f : 0f, "_DirectionFeedback_OpacityFactor", _wallFadeDuration).SetEase(Ease.Linear);

            GameObject wall = _walls[wallIndex].gameObject;

            if (active)
            {
                wall.SetActive(true);
            }
            else
            {
                tween.OnComplete(() => wall.SetActive(false));
            }

            sequence.Join(tween);
        }
    }

    private void SetWallCollidersEnabled(bool enabled)
    {
        Sequence sequence = DOTween.Sequence();

        for (int wallIndex = 0; wallIndex < _walls.Count; wallIndex++)
        {
            Tween tween = _wallMaterials[wallIndex].DOFloat(enabled ? 1f : 0f, "_OpacityFactor", _wallFadeDuration);

            Collider wall = _walls[wallIndex];

            if (enabled)
            {
                wall.enabled = true;
            }
            else
            {
                tween.OnComplete(() => wall.enabled = false);
            }

            sequence.Join(tween);
        }
    }

    private void Awake()
    {
        for (int wallIndex = 0; wallIndex < _walls.Count; wallIndex++)
        {
            Material wallMaterial = _walls[wallIndex].GetComponent<Renderer>().material;
            _wallMaterials.Add(wallMaterial);
            wallMaterial.SetFloat("_TriggeringCameraAngle", _minimumCameraAngleToTrigger);
            wallMaterial.SetFloat("_OpacityFactor", 0f);
            wallMaterial.SetFloat("_DirectionFeedback_OpacityFactor", 0f);
        }

        SetWallsVisible(false);
        SetWallCollidersEnabled(false);
    }

    private void Update()
    {
        CheckPlayerPosition();
    }
}
