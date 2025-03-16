using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;

public class PlayerLook
{
    public PlayerController Player { get; private set; }

    private Transform hand;

    private float _distance = 3f;

    private float _xRotation = 0f;
    private float _yRotation = 0f;

    private float _xSensitivity = 30f;
    private float _ySensitivity = 30f;

    private Vector3 _currentRotation = Vector3.zero;
    private Vector3 _targetRotation = Vector3.zero;

    public void Init(PlayerController player)
    {
        Player = player;
        hand = Player.Hand;
    }

    public void OnUpdate()
    {
        if (Player.IsOwner)
            UpdateInteract();
    }

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;
        _xRotation -= (mouseY * Time.deltaTime) * _ySensitivity;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        _yRotation += (mouseX * Time.deltaTime) * _xSensitivity;
        _targetRotation = Vector3.Lerp(_targetRotation, Vector3.zero, Player.CurrentWeapon.WeaponData.recoilSpeed * Time.deltaTime);
        _currentRotation = Vector3.Slerp(_currentRotation, _targetRotation, Player.CurrentWeapon.WeaponData.returnSpeed * Time.deltaTime);

        hand.localRotation = Quaternion.Euler(new Vector3(Mathf.Clamp(_xRotation + _currentRotation.x, -90f, 90f), 0f, 0f));
        Player.transform.localRotation = Quaternion.Euler(new Vector3(0f, _yRotation + _currentRotation.y, 0f));
    }

    public void Recoil(Vector2 recoil)
    {
        _targetRotation += new Vector3(-recoil.x, UnityEngine.Random.Range(-recoil.y, recoil.y), 0f);
    }

    private void UpdateInteract()
    {
        if (UIManager.Instance.InGame != null)
        {
            UIManager.Instance.InGame.SetPromptText(string.Empty);
        }
        Ray ray = new Ray(hand.position, hand.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, _distance, 1 << LayerMask.NameToLayer(Global.ObjectLayer.Interactable)))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                if (UIManager.Instance.InGame != null)
                {
                    UIManager.Instance.InGame.SetPromptText(interactable.promptMessage);
                }
                if (GameManager.Instance.Input.InGame.Interact.triggered)
                {
                    interactable.BaseInteract(Player);
                }
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void InteractServerRPC(Interactable interactable)
    {
        interactable.BaseInteract(Player);
        InteractClientRPC(interactable);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void InteractClientRPC(Interactable interactable)
    {
        if(!NetworkManager.Singleton.IsHost)
        {
            interactable.BaseInteract(Player);
        }
    }
}
