using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : Interactable
{
    [SerializeField]
    private GameObject door;
    private bool doorOpen;

    void Start()
    {
        needToSyncronize = true;
    }

    void Update()
    {
        
    }

    protected override void Interact(PlayerController player)
    {
        doorOpen = !doorOpen;
        door.GetComponent<Animator>().SetBool("isOpen", doorOpen);
    }
}
