using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LightningEnhanceEffect", menuName = "Scriptable Object/ScrollEffect/LightningEnhanceEffect", order = 1)]
public class LightningEnhanceEffect : ItemEffect
{
    public int EnhanceAmount;

    public override void ExecuteEffect(Item item, PlayerController player)
    {
        player.ElementalDamage[(int)EnumTypes.ElementType.Lightning] += EnhanceAmount;
    }

    public override void ScrollDropEffect(Item item, PlayerController player)
    {
        player.ElementalDamage[(int)EnumTypes.ElementType.Lightning] -= EnhanceAmount;
    }
}
