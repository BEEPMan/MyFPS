using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool needToSyncronize = false;
    //Add or remove an InteractionEvent component to this gameobject.
    public bool useEvents;
    //message displayed to player when looking at an interactable.
    public string promptMessage;
    //this function will be called from our player.
    public void BaseInteract(PlayerController player)
    {
        if (useEvents)
            GetComponent<InteractionEvent>().OnInteract.Invoke();
        Interact(player);
    }

    protected virtual void Interact(PlayerController player)
    {

    }
}
