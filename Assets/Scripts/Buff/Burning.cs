using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Burning : Damagable
{
    public Burning(string buffName, float duration, float damage, Stat buffer, EnemyStat buffTarget, Action<string> OnEndBuffAction) : base(buffName, duration, damage, buffer, buffTarget, OnEndBuffAction)
    {
    }

    protected override IEnumerator StartBuff()
    {
        if (_buffTarget.FindBuff("Decay"))
        {
            _buffTarget.AddBuff(_buffer, "Explosion", 5.0f, 5.0f);
        }
        if (_buffTarget.FindBuff("Shock"))
        {
            _buffTarget.AddBuff(_buffer, "Manipulation", 5.0f);
        }
        for (int i = 0; i < _duration/_damageInterval; i++)
        {
            yield return _delay;
            _buffTarget.TakeDamage(_damage);
        }
        OnDisableBuff();
    }
}
