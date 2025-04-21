using DG.Tweening;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private float _playerDetectionDelay;
    [SerializeField] private float _height;
    [SerializeField] private float _travelTime;
    [SerializeField] private Ease _easeType;
    [SerializeField] private Collider _collider;
    [SerializeField] private float _minimumCameraAngleToTrigger;

    float _playerTimer;
    bool _isMoving;
    bool _lastCameraAngleIsUp;

    private void MoveUp()
    {
        _isMoving = true;
        DOTween.Sequence()
            .Join(transform.DOLocalMoveY(transform.position.y + _height, _travelTime).SetEase(_easeType))
            .Join(Game.Player.DOLocalMoveY(Game.Player.position.y + _height, _travelTime).SetEase(_easeType))
            .OnComplete(Stop);
    }

    private void MoveDown()
    {
        _isMoving = true;
        DOTween.Sequence()
            .Join(transform.DOLocalMoveY(transform.position.y - _height, _travelTime).SetEase(_easeType))
            .Join(Game.Player.DOLocalMoveY(Game.Player.position.y - _height, _travelTime).SetEase(_easeType))
            .OnComplete(Stop);
    }

    private void Stop()
    {
        _isMoving = false;
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

            _lastCameraAngleIsUp = isLookingUp;
        }
        else
        {
            _playerTimer = 0f;
        }
    }

    private void Update()
    {
        CheckPlayerPosition();
    }
}
