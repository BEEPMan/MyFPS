using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New FireEnhanceEffect", menuName = "Scriptable Object/ScrollEffect/FireEnhanceEffect", order = 1)]
public class FireEnhanceEffect : ItemEffect
{
    public int EnhanceAmount;

    public override void ExecuteEffect(Item item, PlayerController player)
    {
        player.ElementalDamage[(int)EnumTypes.ElementType.Fire] += EnhanceAmount;
    }

    public override void ScrollDropEffect(Item item, PlayerController player)
    {
        player.ElementalDamage[(int)EnumTypes.ElementType.Fire] -= EnhanceAmount;
    }
}
