using System;
using Unity.Mathematics;
using UnityEngine;

public class KeyMatcher : MonoBehaviour
{
    [SerializeField] private int _crystalIndex;
    [SerializeField] private Transform _shapeCenter;
    [SerializeField] private Vector2 _distanceMinMax;
    [SerializeField] private float _aimTolerance;
    [SerializeField] private float _angleTolerance;
    [SerializeField] private AlmostRatiosInfo _almostRatios;

    private bool _aimIsValid;
    private bool _distanceIsValid;
    private bool _angleIsValid;
    private bool _isValidated;

    public void CheckMatch()
    {
        if (!Game.KeyToggler.KeyIsOut)
        {
            Game.KeyMatchFeedback.SetFeedbackIntensity(0f, this);

            return;
        }

        if (_isValidated)
        {
            return;
        }

        float keyNearMatchScore = 0f;

        Vector3 cameraPosition = Game.Camera.transform.position;
        Vector3 cameraDirection = Game.Camera.transform.forward;
        Vector3 cameraToShapeDirection = (_shapeCenter.position - Game.Camera.transform.position).normalized;

        float aimAngleOffset = Vector3.Angle(cameraDirection, cameraToShapeDirection);
        _aimIsValid = aimAngleOffset < _aimTolerance;
        keyNearMatchScore += Mathf.Clamp01(math.remap(_aimTolerance * (1f + _almostRatios.Aim), _aimTolerance, 0f, 1f, aimAngleOffset));

        Vector3 flatCameraPosition = new(cameraPosition.x, 0f, cameraPosition.z);
        Vector3 flatShapePosition = new(_shapeCenter.position.x, 0f, _shapeCenter.position.z);

        float distance = (flatShapePosition - flatCameraPosition).magnitude;
        _distanceIsValid = distance > _distanceMinMax.x && distance < _distanceMinMax.y;
        float distanceGap = _distanceMinMax.y - _distanceMinMax.x;
        keyNearMatchScore += Mathf.Clamp01(
            distance < _distanceMinMax.x ?
            math.remap(_distanceMinMax.x - distanceGap * (1f + _almostRatios.Distance), _distanceMinMax.x, 0f, 1f, distance) :
            math.remap(_distanceMinMax.y + distanceGap * (1f + _almostRatios.Distance), _distanceMinMax.y, 0f, 1f, distance)
            );

        Vector3 flatShapeToCameraDirection = (flatCameraPosition - flatShapePosition).normalized;
        Vector3 flatShapeForward = new(_shapeCenter.forward.x, 0f, _shapeCenter.forward.z);
        float angleOffsetFromShapeForward = Vector3.Angle(flatShapeToCameraDirection, flatShapeForward);
        _angleIsValid = angleOffsetFromShapeForward < _angleTolerance;
        keyNearMatchScore += Mathf.Clamp01(math.remap(_angleTolerance * (1f + _almostRatios.Angle), _angleTolerance, 0f, 1f, angleOffsetFromShapeForward));

        keyNearMatchScore /= 3;

        Game.KeyMatchFeedback.SetFeedbackIntensity(keyNearMatchScore, this);

        if (_aimIsValid && _distanceIsValid && _angleIsValid)
        {
            Game.KeyMatchManager.ValidateMatch(_crystalIndex, this);
            _isValidated = true;
        }
    }

    private void Update()
    {
        CheckMatch();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _aimIsValid ? Color.green : Color.red;
        Gizmos.DrawSphere(_shapeCenter.position, radius: 0.05f);

        Gizmos.color = _angleIsValid && _distanceIsValid ? Color.green : Color.red;
        Vector3 flatShapePosition = new(_shapeCenter.position.x, transform.position.y, _shapeCenter.position.z);
        Vector3 leftEdgeEnd = flatShapePosition + Quaternion.AngleAxis(-_angleTolerance, Vector3.up) * new Vector3(_shapeCenter.forward.x, 0f, _shapeCenter.forward.z).normalized * _distanceMinMax.y;
        Vector3 rightEdgeEnd = flatShapePosition + Quaternion.AngleAxis(_angleTolerance, Vector3.up) * new Vector3(_shapeCenter.forward.x, 0f, _shapeCenter.forward.z).normalized * _distanceMinMax.y;
        Gizmos.DrawLine(flatShapePosition, leftEdgeEnd);
        Gizmos.DrawLine(flatShapePosition, rightEdgeEnd);
    }

    [Serializable]
    private struct AlmostRatiosInfo
    {
        public float Distance;
        public float Aim;
        public float Angle;
    }
}
