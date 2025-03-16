using System.Collections;
using System.Collections.Generic;
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
        if(activeState != null)
        {
            activeState.Perform();
        }
    }

    public void ChangeState(BaseState newState)
    {
        if(activeState != null)
        {
            activeState.Exit();
        }
        activeState = newState;

        if(activeState != null)
        {
            activeState.stateMachine = this;
            activeState.Enter();
        }
    }
}
