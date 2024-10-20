using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class BuffSystem : MonoBehaviour
{
    private Dictionary<string, Buff> _buffs = new();
    private Dictionary<string, List<Buff>> _stackableBuffs = new();
    private EnemyStat _stat;

    public int miasmaCount = 0;

    void Start()
    {
        _stat = GetComponent<EnemyStat>();
        ClearBuff();
    }

    void Update()
    {
        if (miasmaCount > 1)
        {
            _stat.enemyHPBar.EnableMiasmaStack();
            _stat.enemyHPBar.UpdateMiasmaCount(miasmaCount);
        }
        else
        {
            _stat.enemyHPBar.DisableMiasmaStack();
        }
    }

    private void OnEnable()
    {
        ClearBuff();
    }

    public void AddBuff(Stat buffer, string buffName, float duration, float damage, Vector3 force)
    {
        if (!gameObject.activeInHierarchy) return;
        if (_buffs.ContainsKey(buffName))
        {
            StopCoroutine(_buffs[buffName].Coroutine);
            _buffs.Remove(buffName);
        }
        else if(_stackableBuffs.ContainsKey(buffName))
        {
            if (_stackableBuffs[buffName].Count >= _stackableBuffs[buffName][0].MaxStack)
            {
                return;
            }
            if (buffName == "Miasma") miasmaCount++;
        }

        if(BuffList.Buffs.TryGetValue(buffName, out var buffFactory))
        {
            Buff buff = buffFactory.Invoke(buffer, _stat, duration, damage, force, DisableBuffIcon);
            if(buff.Stackable)
            {
                if (_stackableBuffs.ContainsKey(buffName))
                {
                    _stackableBuffs[buffName].Add(buff);
                }
                else
                {
                    _stackableBuffs.Add(buffName, new List<Buff> { buff });
                }
                buff.RemoveBuff += RemoveStackableBuff;
                StartCoroutine(buff.Coroutine);
            }
            else
            {
                _buffs.Add(buffName, buff);
                buff.RemoveBuff += RemoveBuff;
                StartCoroutine(buff.Coroutine);
            }
        }
    }

    public void RemoveBuff(string buffName)
    {
        if (_buffs.ContainsKey(buffName))
        {
            _buffs.Remove(buffName);
        }
    }

    public void RemoveStackableBuff(string buffName)
    {
        if(_stackableBuffs.ContainsKey(buffName))
        {
            _stackableBuffs[buffName].RemoveAt(0);
            if (buffName == "Miasma") miasmaCount--;
            if (_stackableBuffs[buffName].Count == 0)
                _stackableBuffs.Remove(buffName);
        }
    }

    public bool FindBuff(string buffName)
    {
        if (_buffs.ContainsKey(buffName)) return true;
        if (_stackableBuffs.ContainsKey(buffName)) return true;
        return false;
    }

    public int GetBuffStack(string buffName)
    {
        if(_stackableBuffs.ContainsKey(buffName))
        {
            return _stackableBuffs[buffName].Count;
        }
        return 0;
    }

    public void ClearBuff()
    {
        _buffs.Clear();
        _stackableBuffs.Clear();
    }

    public void DisableBuffIcon(string buffName)
    {
        if (buffName == "Miasma" && miasmaCount > 0) return;
        _stat.enemyHPBar.DisableBuffIcon(buffName);
    }
}
