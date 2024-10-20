using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody _rb;
    private bool _isGrounded;
    private bool _isSlope;
    private float _slopeAngle;

    public Transform groundCheck;
    private LayerMask _groundLayer;
    public float speed = 5f;
    public float jumpHeight = 10f;
    [Range(0f, 60f)]
    public float maxSlope = 45f;

    private Vector3 _slopeNormal;

    private bool _isDashing;

    private float dashingTimer;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _groundLayer = 1 << LayerMask.NameToLayer("Ground") | 1 <<  LayerMask.NameToLayer("Interactable");
    }

    void Update()
    {
        _isGrounded = CheckGround();
        _isSlope = CheckSlope();
        if (!_isDashing) _rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
        dashingTimer += Time.deltaTime;
        if(dashingTimer > 0.3f)
        {
            if (_isDashing) _rb.velocity = new Vector3(0f, (!_isSlope ? _rb.velocity.y : 0f), 0f);
            _isDashing = false;
        }
        _rb.useGravity = !(_isGrounded && _slopeAngle <= maxSlope);
    }

    public void ProcessMove(Vector2 input)
    {
        if (_isDashing) return;
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        moveDirection = transform.TransformDirection(moveDirection);
        if (_isGrounded && _isSlope)
        {
            moveDirection = ProjectToSlope(moveDirection);
            moveDirection *= moveDirection.y >= 0 ? Mathf.Cos(Mathf.Deg2Rad * _slopeAngle) : 1 / Mathf.Cos(Mathf.Deg2Rad * _slopeAngle);
        }
        _rb.MovePosition(_rb.position + moveDirection * speed * Time.deltaTime);
    }

    public bool TryJump()
    {
        if (_isGrounded)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            return true;
        }
        return false;
    }

    public void Dash(Vector2 input)
    {
        dashingTimer = 0f;
        _isDashing = true;
        Vector3 dashDirection = input == Vector2.zero ? transform.forward : transform.TransformDirection(input.x, 0f, input.y);
        dashDirection.y = 0f;
        dashDirection = dashDirection.normalized * 10.0f;
        dashDirection.y = _rb.velocity.y;
        _rb.velocity = dashDirection;
    }

    private bool CheckGround()
    {
        return Physics.BoxCast(groundCheck.position, new Vector3(transform.lossyScale.x * 0.25f, 0.05f, transform.lossyScale.x * 0.25f), -transform.up, transform.rotation, 1.1f, _groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position - transform.up * 0.0f, new Vector3(transform.lossyScale.x * 0.5f, 0.1f, transform.lossyScale.x * 0.5f));
    }

    private bool CheckSlope()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 2f, _groundLayer))
        {
            _slopeNormal = hitInfo.normal;
            _slopeAngle = Vector3.Angle(Vector3.up, _slopeNormal);
            return _slopeAngle > 0.1f && _slopeAngle <= maxSlope;
        }
        return false;
    }

    private Vector3 ProjectToSlope(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, _slopeNormal).normalized;
    }

    
}
