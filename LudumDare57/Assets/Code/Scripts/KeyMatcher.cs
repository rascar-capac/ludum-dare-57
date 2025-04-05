using UnityEngine;

public class KeyMatcher : MonoBehaviour
{
    [SerializeField] private Transform _shapeCenter;
    [SerializeField] private Vector2 _distanceMinMax;
    [SerializeField] private float _aimTolerance;
    [SerializeField] private float _angleTolerance;

    private bool _aimIsValid;
    private bool _distanceIsValid;
    private bool _angleIsValid;

    public void CheckMatch()
    {
        //check KeyActivator

        Vector3 cameraPosition = Game.Camera.transform.position;
        Vector3 cameraDirection = Game.Camera.transform.forward;
        Vector3 cameraToShapeDirection = (_shapeCenter.position - Game.Camera.transform.position).normalized;

        float aimAngleOffset = Vector3.Angle(cameraDirection, cameraToShapeDirection);
        _aimIsValid = aimAngleOffset < _aimTolerance;

        Vector3 flatCameraPosition = new(cameraPosition.x, 0f, cameraPosition.z);
        Vector3 flatShapePosition = new(_shapeCenter.position.x, 0f, _shapeCenter.position.z);

        float sqrDistance = (flatShapePosition - flatCameraPosition).sqrMagnitude;
        _distanceIsValid = sqrDistance > _distanceMinMax.x * _distanceMinMax.x && sqrDistance < _distanceMinMax.y * _distanceMinMax.y;

        Vector3 flatShapeToCameraDirection = (flatCameraPosition - flatShapePosition).normalized;
        Vector3 flatShapeForward = new(0f, _shapeCenter.forward.y, _shapeCenter.forward.z);
        float angleOffsetFromShapeForward = Vector3.Angle(flatShapeToCameraDirection, flatShapeForward);
        _angleIsValid = angleOffsetFromShapeForward < _angleTolerance;
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
        Vector3 flatShapePosition = new(_shapeCenter.position.x, 0f, _shapeCenter.position.z);
        Vector3 leftEdgeEnd = flatShapePosition + Quaternion.AngleAxis(-_angleTolerance, Vector3.up) * new Vector3(_shapeCenter.forward.x, 0f, _shapeCenter.forward.z).normalized * _distanceMinMax.y;
        Vector3 rightEdgeEnd = flatShapePosition + Quaternion.AngleAxis(_angleTolerance, Vector3.up) * new Vector3(_shapeCenter.forward.x, 0f, _shapeCenter.forward.z).normalized * _distanceMinMax.y;
        Gizmos.DrawLine(flatShapePosition, leftEdgeEnd);
        Gizmos.DrawLine(flatShapePosition, rightEdgeEnd);
    }
}
