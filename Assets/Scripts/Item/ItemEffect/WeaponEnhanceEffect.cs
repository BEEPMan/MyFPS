using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WeaponEnhanceEffect", menuName = "Scriptable Object/ScrollEffect/WeaponEnhanceEffect", order = 1)]
public class WeaponEnhanceEffect : ItemEffect
{
    public int EnhanceAmount;

    public override void ExecuteEffect(Item item, PlayerController player)
    {
        player.WeaponDamage += EnhanceAmount;
    }

    public override void ScrollDropEffect(Item item, PlayerController player)
    {
        player.WeaponDamage -= EnhanceAmount;
    }
}
