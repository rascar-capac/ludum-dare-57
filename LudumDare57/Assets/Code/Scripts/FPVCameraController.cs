using UnityEngine;
using UnityEngine.InputSystem;

public class FPVCameraController : MonoBehaviour
{
    [SerializeField] private Transform playerCamera = null;
    [SerializeField][Range(0f, 100f)] private float _yawAnglePerSecond = 0f;
    [SerializeField][Range(0f, 100f)] private float _pitchAnglePerSecond = 0f;
    [SerializeField] private InputActionReference _lookInput;
    [SerializeField] private float _pitchAngleDownLimit;
    [SerializeField] private float _pitchAngleUpLimit;

    private Transform player;

    private void Start()
    {
        player = transform;
    }

    private void Update()
    {
        float yawRotation = _yawAnglePerSecond * Time.deltaTime * _lookInput.action.ReadValue<Vector2>().x;
        player.Rotate(Vector3.up, yawRotation);

        if (playerCamera != null)
        {
            float pitchRotation = _pitchAnglePerSecond * Time.deltaTime * -_lookInput.action.ReadValue<Vector2>().y;
            float newRotation = playerCamera.transform.localRotation.eulerAngles.x + pitchRotation;

            while (newRotation < 0.0f)
            {
                newRotation += 360.0f;
            }

            while (newRotation > 360.0f)
            {
                newRotation -= 360.0f;
            }

            if (newRotation < 180f)
            {
                newRotation = Mathf.Clamp(newRotation, 0f, _pitchAngleDownLimit);
            }
            else
            {
                newRotation = Mathf.Clamp(newRotation, 360f - _pitchAngleUpLimit, 360f);
            }

            playerCamera.transform.localRotation = Quaternion.Euler(newRotation, 0.0f, 0.0f);
        }
    }
}
