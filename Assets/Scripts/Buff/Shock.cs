using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shock : Buff
{
    public Shock(string buffName, float duration, Stat buffer, EnemyStat buffTarget, Action<string> OnEndBuffAction) : base(buffName, duration, buffer, buffTarget, OnEndBuffAction)
    {
        _delay = new WaitForSeconds(duration);
    }

    protected override IEnumerator StartBuff()
    {
        if (_buffTarget.FindBuff("Burning"))
        {
            _buffTarget.AddBuff(_buffer, "Manipulation", 5.0f);
        }
        if (_buffTarget.FindBuff("Decay"))
        {
            _buffTarget.AddBuff(_buffer, "Miasma", 5.0f, _buffTarget.MaxHP * 0.09f);
        }
        _buffTarget.damageCalculator.MulTakenDamage(0.1f);
        yield return _delay;
        _buffTarget.damageCalculator.DivTakenDamage(0.1f);
        OnDisableBuff();
    }
}
