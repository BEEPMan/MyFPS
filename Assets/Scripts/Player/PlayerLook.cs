using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLook : MonoBehaviour
{
    private Camera _cam;
    private InputManager _inputManager;

    [SerializeField]
    private float _distance = 3f;
    [SerializeField]
    private LayerMask _interactMask;

    private float _xRotation = 0f;
    private float _yRotation = 0f;

    [SerializeField]
    private float _xSensitivity = 30f;
    [SerializeField]
    private float _ySensitivity = 30f;

    private void Awake()
    {
        _cam = Player.Instance.mainCamera;
        _inputManager = GetComponent<InputManager>();
    }

    private void Update()
    {
        UpdateInteract();
    }

    public void ProcessLook(Vector2 input, Vector3 recoilRotation)
    {
        float mouseX = input.x;
        float mouseY = input.y;
        _xRotation -= (mouseY * Time.deltaTime) * _ySensitivity;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        _yRotation += (mouseX * Time.deltaTime) * _xSensitivity;
        _cam.transform.localRotation = Quaternion.Euler(_xRotation + recoilRotation.x, 0, recoilRotation.z);
        transform.localRotation = Quaternion.Euler(0, _yRotation + recoilRotation.y, 0);
    }

    private void UpdateInteract()
    {
        Player.Instance.UI.SetPromptText(string.Empty);
        Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, _distance, _interactMask))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                Player.Instance.UI.SetPromptText(interactable.promptMessage);
                if (_inputManager.InGame.Interact.triggered)
                {
                    interactable.BaseInteract();
                }
            }
        }
    }
}
