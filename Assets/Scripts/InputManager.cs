using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput _playerInput;
    public PlayerInput.InGameActions InGame;
    public PlayerInput.UIActions UIInput;

    public GameObject InGameUI;
    public GameObject InventoryUI;

    [SerializeField]
    private float _jumpCoolTime = 0.3f;
    private float jumpTimer = 0.3f;
    [SerializeField]
    private float _dashCoolTime = 1.0f;
    private float dashTimer = 1.0f;
    [SerializeField]
    private float _subSkillCoolTime = 0.3f;
    private float subSkillTimer = 0.3f;
    [SerializeField]
    private float _skillCoolTime = 5f;
    private float skillTimer = 5f;

    void Awake()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        _playerInput = new PlayerInput();
        InGame = _playerInput.InGame;
        UIInput = _playerInput.UI;

        InGame.Jump.performed += ctx =>
        {
            if (jumpTimer > _jumpCoolTime)
            {
                jumpTimer = Player.Instance.Move.TryJump() ? 0f : jumpTimer;
            }
        };
        InGame.Dash.performed += ctx =>
        {
            if (dashTimer > _dashCoolTime)
            {
                Player.Instance.Move.Dash(InGame.Movement.ReadValue<Vector2>());
                dashTimer = 0f;
                Player.Instance.UI.SetDashIcon(dashTimer, _dashCoolTime);
            }
        };

        InGame.Skill.performed += ctx =>
        {
            if (skillTimer > _skillCoolTime)
            {
                Player.Instance.PWeapon.CastSkill();
                skillTimer = 0f;
                Player.Instance.UI.SetMainSkillIcon(skillTimer, _skillCoolTime);
            }
        };
        InGame.SubSkill.canceled += ctx =>
        {
            if (subSkillTimer > _subSkillCoolTime)
            {
                Player.Instance.PWeapon.CastSubSkill();
                subSkillTimer = 0f;
                Player.Instance.UI.SetSubSkillIcon(subSkillTimer, _subSkillCoolTime);
            }
        };
        InGame.OpenIventory.performed += ctx =>
        {
            UIManager.Instance.OpenPopUp(PopUpType.Scroll);
        };

        InGame.Enable();
        UIInput.Disable();
    }

    void Start()
    {
        if (Player.Instance.PWeapon.CurrentWeapon is not Rifle)
            InGame.FIre.performed += ctx => Player.Instance.PWeapon.CurrentWeapon.Fire();
        InGame.WeaponSkill.performed += ctx => Player.Instance.PWeapon.CurrentWeapon.Skill();
        InGame.Reload.performed += ctx => Player.Instance.PWeapon.CurrentWeapon.Reload();
        InGame.WeaponNum.performed += ctx => Player.Instance.PWeapon.SelectWeaponNum((int)ctx.ReadValue<float>());

        UIInput.CloseInventory.performed += ctx =>
        {
            if (UIManager.Instance.currentPopUp != PopUpType.Scroll && UIManager.Instance.currentPopUp != PopUpType.Weapon) return;
            UIManager.Instance.ClosePopUp();
        };
        UIInput.CloseNPCWindow.performed += ctx =>
        {
            UIManager.Instance.ClosePopUp();
        };
    }

    void Update()
    {
        if (InGame.FIre.IsPressed() && Player.Instance.PWeapon.CurrentWeapon is Rifle)
        {
            Player.Instance.PWeapon.CurrentWeapon.Fire();
        }
        if(InGame.ChangeWeapon.ReadValue<float>()>0)
        {
            Player.Instance.PWeapon.ChangeToBeforeWeapon();
        }
        if (InGame.ChangeWeapon.ReadValue<float>() < 0)
        {
            Player.Instance.PWeapon.ChangeToNextWeapon();
        }

        jumpTimer += Time.deltaTime;
        dashTimer += Time.deltaTime;
        subSkillTimer += Time.deltaTime;
        skillTimer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        Player.Instance.Move.ProcessMove(InGame.Movement.ReadValue<Vector2>());
    }

    void LateUpdate()
    {
        Player.Instance.Look.ProcessLook(InGame.Look.ReadValue<Vector2>(), Player.Instance.PWeapon.CurrentWeapon.CurrentRotation);
    }

    private void OnEnable()
    {
        InGame.Enable();
    }

    private void OnDisable()
    {
        InGame.Disable();
    }
}
