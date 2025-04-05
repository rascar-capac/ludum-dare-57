using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPVMoveController : MonoBehaviour
{
    [SerializeField][Range(0f, 100f)] private float _metersPerSecond = 0f;
    [SerializeField][Range(0f, 10f)] private float _jumpHeight = 0f;
    [SerializeField] private InputActionReference _moveInput;
    [SerializeField] private InputActionReference _jumpInput;
    [SerializeField] private InputActionReference _crouchInput;
    [SerializeField] private Transform _groundCheck = null;
    private Transform _transform;
    private CharacterController _characterController;
    private float _jumpVelocity;
    private bool _isCrouched;
    private readonly float _gravityValue = Physics.gravity.y;

    private void Awake()
    {
        _transform = transform;
        _characterController = GetComponent<CharacterController>();
        if (!_characterController) Debug.LogWarning("Missing CharacterController component", this);
        if (!_groundCheck) Debug.LogWarning("Missing Ground Check reference in FPVMoveController", this);
    }
    private void Update()
    {
        Vector3 xMovement = _moveInput.action.ReadValue<Vector2>().x * _transform.right;
        Vector3 zMovement = _moveInput.action.ReadValue<Vector2>().y * _transform.forward;
        Vector3 movement = (xMovement + zMovement) * Time.deltaTime * _metersPerSecond;
        if (_characterController)
        {
            _characterController.Move(movement);
        }

        HandleJump();
        HandleCrouch();
    }

    private void HandleJump()
    {
        if (_jumpInput == null) return;
        if (!_groundCheck) return;

        bool isGrounded = Physics.CheckSphere(_groundCheck.position, 0.1f);
        if (isGrounded && _jumpVelocity < 0)
        {
            _jumpVelocity = 0f;
        }

        if (_jumpInput.action.inProgress && isGrounded)
        {
            _jumpVelocity += Mathf.Sqrt(_jumpHeight * -3.0f * _gravityValue);
        }

        _jumpVelocity += _gravityValue * Time.deltaTime;
        _characterController.Move(new Vector3(0f, _jumpVelocity, 0f) * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        if (_crouchInput == null) return;

        if (!_isCrouched && _crouchInput.action.inProgress)
        {
            _characterController.height = 1;
            transform.position -= new Vector3(0, 0.5f, 0);
            _groundCheck.transform.position += new Vector3(0, 0.5f, 0);
            _isCrouched = true;
        }
        else if (_isCrouched && !_crouchInput.action.inProgress)
        {
            _characterController.height = 2;
            transform.position += new Vector3(0, 0.5f, 0);
            _groundCheck.transform.position -= new Vector3(0, 0.5f, 0);
            _isCrouched = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_groundCheck.position, 0.1f);
    }
}
