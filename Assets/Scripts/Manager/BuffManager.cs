using System;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager
{
    public event Action<BaseBuff> buffEnabled;
    public event Action<BaseBuff> buffDisabled;

    private BaseController _controller;

    private Dictionary<string, List<BaseBuff>> activeBuffs;

    public BuffManager(BaseController controller)
    {
        _controller = controller;
        activeBuffs = new Dictionary<string, List<BaseBuff>>();
    }

    public void Init()
    {
        activeBuffs.Clear();
    }

    public void OnUpdate()
    {
        float deltaTime = Time.deltaTime;
        foreach (List<BaseBuff> buffs in activeBuffs.Values)
        {
            for (int i = buffs.Count - 1; i >= 0; i--)
            {
                buffs[i].UpdateBuff(deltaTime);
                if (buffs[i].IsExpired == true)
                {
                    buffs[i].OnExpire();
                    buffDisabled?.Invoke(buffs[i]);
                    buffs.RemoveAt(i);
                }
            }
        }
    }

    public void AddBuff(BaseBuff newBuff)
    {
        if(activeBuffs.ContainsKey(newBuff.BuffName) == false)
        {
            activeBuffs[newBuff.BuffName] = new List<BaseBuff>();
        }

        List<BaseBuff> buffs = activeBuffs[newBuff.BuffName];

        if (newBuff.IsStackable == true && Global.MaxStackCount[newBuff.BuffName] <= buffs.Count)
            return;
        if (newBuff.IsStackable == false && buffs.Count > 0)
        {
            if (newBuff.BuffName == "Stagger") return;
            else if(newBuff is Freeze freeze)
            {
                freeze.Duration = (buffs[0].Duration - buffs[0].ElapsedTime) + freeze.Duration / 2;
            }
            buffs.Clear();
        }

        buffs.Add(newBuff);
        newBuff.ApplyEffect();
        TriggerElementalFusion(newBuff);
        buffEnabled?.Invoke(newBuff);
    }

    public BaseBuff FindBuff(string buffName)
    {
        List<BaseBuff> buff;
        activeBuffs.TryGetValue(buffName, out buff);
        if(buff == null || buff.Count == 0) return null;
        return buff[0];
    }

    public void ClearBuffs()
    {
        activeBuffs.Clear();
    }

    public void TriggerElementalFusion(BaseBuff triggerBuff)
    {
        if (!(_controller is EnemyController enemyController)) return;
        switch (triggerBuff)
        {
            case Burning burning:
                if (FindBuff("Decay") != null)
                {
                    AddBuff(new Combustion(enemyController, burning.Damage));
                }
                if (FindBuff("Shock") != null)
                {
                    AddBuff(new Manipulation(enemyController, 5.0f));
                }
                break;
            case Decay:
                if (FindBuff("Burning") != null)
                {
                    Burning burning = (Burning)FindBuff("Burning");
                    AddBuff(new Combustion(enemyController, burning.Damage));
                }
                if (FindBuff("Shock") != null)
                {
                    AddBuff(new Miasma(enemyController, 5.0f));
                }
                break;
            case Shock:
                if (FindBuff("Burning") != null)
                {
                    AddBuff(new Manipulation(enemyController, 5.0f));
                }
                if (FindBuff("Decay") != null)
                {
                    AddBuff(new Miasma(enemyController, 5.0f));
                }
                break;
        }
    }

    public int GetMiasmaCount()
    {
        if (activeBuffs.ContainsKey("Miasma") == false) return 0;
        return activeBuffs["Miasma"].Count;
    }

    public void SyncBuffs()
    {
        foreach (List<BaseBuff> buffs in activeBuffs.Values)
        {
            foreach (BaseBuff buff in buffs)
            {
                
            }
        }
    }
}