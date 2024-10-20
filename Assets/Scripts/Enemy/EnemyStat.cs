using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EnemyStat : Stat
{
    private Enemy _enemy;

    public UI_EnemyHPBar enemyHPBar;

    public override void Init()
    {
        base.Init();
        _enemy = GetComponent<Enemy>();
        enemyHPBar.SetEnemy(_enemy);
        ClearBuff();
        enemyHPBar.InitHPBer();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override int TakeDamage(float damage, ElementalType elementalType = ElementalType.None, Stat attacker = null)
    {
        int damageAmount = base.TakeDamage(damage);
        if(attacker != null && attacker.gameObject.GetInstanceID() == _enemy.Target.GetInstanceID())
        {
            _enemy.FSM.ChangeState(new SearchState());
        }
        enemyHPBar.PopDamageText(damageAmount);
        enemyHPBar.SetLerpTimer();
        if (_HP <= 0f)
        {
            Die();
        }
        return damageAmount;
    }

    public override void RestoreHealth(float healAmount)
    {
        base.RestoreHealth(healAmount);
        enemyHPBar.SetLerpTimer();
    }

    public void AddBuff(Stat buffer, string buffName, float duration, float damage = 0f, Vector3 force = default)
    {
        _buffSystem.AddBuff(buffer, buffName, duration, damage, force);
        enemyHPBar.EnableBuffIcon(buffName);
    }

    public bool FindBuff(string buffName)
    {
        return _buffSystem.FindBuff(buffName);
    }

    public void RemoveBuff(string buffName)
    {
        _buffSystem.RemoveBuff(buffName);
        _buffSystem.RemoveStackableBuff(buffName);
    }

    public void ClearBuff()
    {
        _buffSystem.ClearBuff();
        enemyHPBar.ClearBuffIcon();
    }

    public override void Die()
    {
        GameManager.Instance.OnEnemyDead();
        _enemy.MakePickup();
        ObjectPool.Instance.Push(gameObject);
    }
}
