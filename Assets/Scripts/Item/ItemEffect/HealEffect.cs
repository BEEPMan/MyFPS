using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HealEffect", menuName = "Scriptable Object/ItemEffect/HealEffect", order = 1)]
public class HealEffect : ItemEffect
{
    public int HealAmount;
    public bool isPercentage;

    public override void ExecuteEffect(Item item, PlayerController player)
    {
        if (isPercentage)
            player.RestoreHealth(player.MaxHP.Value * HealAmount / 100);
        else
            player.RestoreHealth(HealAmount);
    }

    public override void ScrollDropEffect(Item item, PlayerController player)
    {
        throw new System.NotImplementedException();
    }
}
