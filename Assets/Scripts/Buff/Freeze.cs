using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Freeze : Buff
{
    private Enemy _enemy;

    public Freeze(string buffName, float duration, Stat buffer, EnemyStat buffTarget, Action<string> OnEndBuffAction) : base(buffName, duration, buffer, buffTarget, OnEndBuffAction)
    {
        _enemy = _buffTarget.gameObject.GetComponent<Enemy>();
    }

    protected override IEnumerator StartBuff()
    {
        _enemy.FSM.ChangeState(new NonState());
        yield return _delay;
        _enemy.FSM.ChangeState(new PatrolState());
        OnDisableBuff();
    }
}
