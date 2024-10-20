using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class BuffList
{
    public static Dictionary<string, System.Func<Stat, EnemyStat, float, float?, Vector3?, Action<string>, Buff>> Buffs = new()
    {
        { "Decay", (bufferStat, targetStat, duration, _, _, OnEndBuffAction) => new Decay("Decay", duration, bufferStat, targetStat, OnEndBuffAction) },
        { "Shock", (bufferStat, targetStat, duration, _, _, OnEndBuffAction) => new Shock("Shock", duration, bufferStat, targetStat, OnEndBuffAction) },
        { "Freeze", (bufferStat, targetStat, duration, _, _, OnEndBuffAction) => new Freeze("Freeze", duration, bufferStat, targetStat, OnEndBuffAction) },
        { "Manipulation", (bufferStat, targetStat, duration, _, _, OnEndBuffAction) => new Manipulation("Manipulation", duration, bufferStat, targetStat, OnEndBuffAction) },
        { "Stagger", (bufferStat, targetStat, duration, _, force, OnEndBuffAction) => new Stagger("Stagger", duration, force.GetValueOrDefault(Vector3.zero), bufferStat, targetStat, OnEndBuffAction) },
        { "Burning", (bufferStat, targetStat, duration, damage, _, OnEndBuffAction) => new Burning("Burning", duration, damage.GetValueOrDefault(0f), bufferStat, targetStat, OnEndBuffAction) },
        { "Explosion", (bufferStat, targetStat, duration, damage, _, OnEndBuffAction) => new Explosion("Explosion", duration, damage.GetValueOrDefault(0f), bufferStat, targetStat, OnEndBuffAction) },
        { "Miasma", (bufferStat, targetStat, duration, damage, _, OnEndBuffAction) => new Miasma("Miasma", duration, damage.GetValueOrDefault(0f), bufferStat, targetStat, OnEndBuffAction) },
    };
}
