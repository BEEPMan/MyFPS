using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager
{
    private PlayerInput _playerInput;
    public PlayerInput.InGameActions InGame { get; set; }
    public PlayerInput.UIActions UIInput { get; set; }

    public void Init()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _playerInput = new PlayerInput();
        InGame = _playerInput.InGame;
        UIInput = _playerInput.UI;

        InGame.Jump.performed += ctx =>
        {
            if (!GameManager.Instance.Player.isStaggered)
                GameManager.Instance.Player.Move.Jump();
        };
        InGame.Dash.performed += ctx =>
        {
            if (!GameManager.Instance.Player.isStaggered)
                GameManager.Instance.Player.Move.Dash(InGame.Movement.ReadValue<Vector2>());
        };

        InGame.Skill.performed += ctx =>
        {
            if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
            {
                if (!GameManager.Instance.Player.isStaggered)
                    GameManager.Instance.Player.CastMainSkill();
            }
        };
        InGame.SubSkill.canceled += ctx =>
        {
            if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
            {
                if (!GameManager.Instance.Player.isStaggered)
                    GameManager.Instance.Player.CastSubSkill();
            }
        };
        InGame.DropWeapon.performed += ctx =>
        {
            if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
            {
                GameManager.Instance.Player.DropWeapon(GameManager.Instance.Player.CurrentWeaponNum.Value);
            }
        };
        InGame.OpenIventory.performed += ctx =>
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPanel("UI_Inventory_Scroll");
            }
        };

        InGame.Enable();
        UIInput.Disable();

        InGame.Fire.performed += ctx =>
        {
            if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
            {
                if (!GameManager.Instance.Player.isStaggered)
                    GameManager.Instance.Player.Attack();
            }
        };
        InGame.WeaponSkill.performed += ctx =>
        {
            if (!GameManager.Instance.Player.isStaggered)
                GameManager.Instance.Player.CurrentWeapon.Skill();
        };
        InGame.Reload.performed += ctx => { if (!GameManager.Instance.Player.isStaggered) GameManager.Instance.Player.Reload(); };
        InGame.WeaponNum.performed += ctx => { if (!GameManager.Instance.Player.isStaggered) GameManager.Instance.Player.SelectWeaponNum((int)ctx.ReadValue<float>() - 1); };

        UIInput.CloseInventory.performed += ctx =>
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HidePanel("UI_Inventory_Scroll");
                UIManager.Instance.HidePanel("UI_Inventory_Weapon");
            }
        };
        UIInput.CloseNPCWindow.performed += ctx =>
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideTopPanel();
            }
        };
    }

    public void OnUpdate()
    {
        if (InGame.Fire.IsPressed() && GameManager.Instance.Player.CurrentWeapon.WeaponData.isAutomatic)
        {
            if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
            {
                if (!GameManager.Instance.Player.isStaggered)
                    GameManager.Instance.Player.Attack();
            }
        }
        if(InGame.ChangeWeapon.ReadValue<float>()>0)
        {
            if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
            {
                if (!GameManager.Instance.Player.isStaggered)
                    GameManager.Instance.Player.ChangeToBeforeWeapon();
            }
        }
        if (InGame.ChangeWeapon.ReadValue<float>() < 0)
        {
            if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
            {
                if (!GameManager.Instance.Player.isStaggered)
                    GameManager.Instance.Player.ChangeToNextWeapon();
            }
        }

        if (!GameManager.Instance.Player.isStaggered)
        {
            GameManager.Instance.Player.Move.ProcessMove(InGame.Movement.ReadValue<Vector2>());
            GameManager.Instance.Player.Look.ProcessLook(InGame.Look.ReadValue<Vector2>());
        }
    }

    public void SetUIMode()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UIInput.Enable();
        InGame.Disable();
    }

    public void SetInGameMode()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        InGame.Enable();
        UIInput.Disable();
    }
}
