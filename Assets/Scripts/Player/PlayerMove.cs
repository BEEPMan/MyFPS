using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class PlayerMove
{
    public PlayerController Player { get; private set; }

    private Rigidbody rb;
    private bool _isGrounded;
    private bool _isSlope;
    private float _slopeAngle;

    private LayerMask _groundLayer;
    private float jumpHeight = 5f;
    private float maxSlope = 45f;

    private Vector3 _slopeNormal;

    private float jumpTimer;
    private float dashTimer;

    private bool isDashing = false;

    public void Init(PlayerController player)
    {
        Player = player;
        rb = Player.GetComponent<Rigidbody>();
        _groundLayer = 1 << LayerMask.NameToLayer("Ground") | 1 <<  LayerMask.NameToLayer("Interactable");
        jumpTimer = Global.JumpCoolTime;
        dashTimer = Global.DashCoolTime;
    }

    public void OnUpdate()
    {
        _isGrounded = CheckGround();
        _isSlope = CheckSlope();
        rb.useGravity = !(_isGrounded && _slopeAngle <= maxSlope);
        if(!isDashing) rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        jumpTimer += Time.deltaTime;
        dashTimer += Time.deltaTime;
    }

    public void ProcessMove(Vector2 input)
    {
        if (isDashing) return;
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        moveDirection = Player.transform.TransformDirection(moveDirection);
        if (_isGrounded && _isSlope)
        {
            moveDirection = ProjectToSlope(moveDirection);
            moveDirection *= moveDirection.y >= 0 ? Mathf.Cos(Mathf.Deg2Rad * _slopeAngle) : 1 / Mathf.Cos(Mathf.Deg2Rad * _slopeAngle);
        }
        rb.MovePosition(rb.position + moveDirection * Player.Speed * (100 + Player.SpeedFactor) / 100 * Time.deltaTime);
    }

    public void Jump()
    {
        if (jumpTimer < Global.JumpCoolTime || !_isGrounded) return;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        jumpTimer = 0f;
    }

    public void Dash(Vector2 input)
    {
        if (dashTimer < Global.DashCoolTime) return;
        isDashing = true;
        Vector3 dashDirection = input == Vector2.zero ? Player.transform.forward : Player.transform.TransformDirection(input.x, 0f, input.y);
        dashDirection.y = 0f;
        dashDirection = dashDirection.normalized * Player.Speed * 2f;
        dashDirection.y = rb.linearVelocity.y;
        rb.linearVelocity = dashDirection;
        dashTimer = 0f;
        if (UIManager.Instance.InGame != null)
        {
            UIManager.Instance.InGame.UpdateDashCoolDown(dashTimer, Global.DashCoolTime);
        }
        AfterDash().Forget();
    }

    protected async UniTaskVoid AfterDash()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
        rb.linearVelocity = new Vector3(0f, (!_isSlope ? rb.linearVelocity.y : 0f), 0f);
        isDashing = false;
    }

    private bool CheckGround()
    {
        return Physics.BoxCast(Player.GroundCheck.position, new Vector3(Player.transform.lossyScale.x * 0.25f, 0.05f, Player.transform.lossyScale.x * 0.25f), -Player.transform.up, Player.transform.rotation, 1.1f, _groundLayer);
    }

    private bool CheckSlope()
    {
        Ray ray = new Ray(Player.transform.position, Vector3.down);
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
