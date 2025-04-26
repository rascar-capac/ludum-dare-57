using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private float _playerDetectionDelay;
    [SerializeField] private float _metersPerSecond;
    [SerializeField] private Ease _easeType;
    [SerializeField] private Collider _collider;
    [SerializeField] private float _minimumCameraAngleToTrigger;
    [SerializeField] private List<GameObject> _walls;
    [SerializeField] private Renderer _wallRenderer;
    [SerializeField] private float _wallFadeDuration;

    private float _playerTimer;
    private bool _playerIsInElevator;
    private bool _wasLookingUp;
    private bool _wasLookingDown;
    private bool _isMoving;

    private void MoveUp()
    {
        float height = Game.LevelManager.GetLevel(Game.LevelManager.CurrentPlayerMetaLevel).Height;

        Game.LevelManager.SetCurrentToUpperLevel();

        _isMoving = true;
        SetWallCollidersActive(true);
        Game.Player.GetComponent<CharacterController>().enabled = false;

        _wallRenderer.material.SetFloat("_DirectionFeedback_OpacityFactor", 0f);

        DOTween.Sequence()
            .Join(transform.DOLocalMoveY(transform.position.y + height, height / _metersPerSecond).SetEase(_easeType))
            .Join(Game.Player.DOLocalMoveY(Game.Player.position.y + height, height / _metersPerSecond).SetEase(_easeType))
            .OnComplete(Stop);
    }

    private void MoveDown()
    {
        Game.LevelManager.SetCurrentToLowerLevel();
        float height = Game.LevelManager.GetLevel(Game.LevelManager.CurrentPlayerMetaLevel).Height;

        _isMoving = true;
        SetWallCollidersActive(true);
        Game.Player.GetComponent<CharacterController>().enabled = false;

        _wallRenderer.material.SetFloat("_DirectionFeedback_OpacityFactor", 0f);

        DOTween.Sequence()
            .Join(transform.DOLocalMoveY(transform.position.y - height, height / _metersPerSecond).SetEase(_easeType))
            .Join(Game.Player.DOLocalMoveY(Game.Player.position.y - height, height / _metersPerSecond).SetEase(_easeType))
            .OnComplete(Stop);
    }

    private void Stop()
    {
        Game.Player.GetComponent<CharacterController>().enabled = true;
        _isMoving = false;
        SetWallCollidersActive(false);

        _wallRenderer.material.SetFloat("_DirectionFeedback_OpacityFactor", 1f);
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

                _wallRenderer.material.SetFloat("_DirectionFeedback_FlashStartTime", Time.time);
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
        _wallRenderer.material.DOFloat(active ? 1f : 0f, "_DirectionFeedback_OpacityFactor", _wallFadeDuration).SetEase(Ease.Linear);
    }

    private void SetWallCollidersActive(bool active)
    {
        Tween tween = _wallRenderer.material.DOFloat(active ? 1f : 0f, "_OpacityFactor", _wallFadeDuration);

        if (active)
        {
            foreach (GameObject wall in _walls)
            {
                wall.SetActive(true);
            }
        }
        else
        {
            tween.OnComplete(() => _walls.ForEach(wall => wall.SetActive(false)));
        }
    }

    private void Awake()
    {
        for (int wallIndex = 0; wallIndex < _walls.Count; wallIndex++)
        {
            _wallRenderer.material.SetFloat("_TriggeringCameraAngle", _minimumCameraAngleToTrigger);
            _wallRenderer.material.SetFloat("_OpacityFactor", 0f);
            _wallRenderer.material.SetFloat("_DirectionFeedback_OpacityFactor", 0f);
        }

        SetWallsVisible(false);
        SetWallCollidersActive(false);
    }

    private void Update()
    {
        CheckPlayerPosition();
    }
}
