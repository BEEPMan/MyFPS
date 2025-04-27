using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public EnemyController Enemy;

    public BaseState activeState;

    public void Init()
    {
        ChangeState(new PatrolState());
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(activeState != null && NetworkManager.Singleton.IsServer)
        {
            activeState.Perform();
        }
    }

    public void ChangeState(BaseState newState)
    {
        if(activeState != null)
        {
            if (NetworkManager.Singleton.IsServer)
                activeState.Exit();
        }
        activeState = newState;

        if(activeState != null)
        {
            activeState.stateMachine = this;
            if(NetworkManager.Singleton.IsServer)
                activeState.Enter();
        }
    }
}
