using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CorrosionEnhanceEffect", menuName = "Scriptable Object/ScrollEffect/CorrosionEnhanceEffect", order = 1)]
public class CorrosionEnhanceEffect : ItemEffect
{
    public int EnhanceAmount;

    public override void ExecuteEffect(Item item, PlayerController player)
    {
        player.ElementalDamage[(int)EnumTypes.ElementType.Corrosion] += EnhanceAmount;
    }

    public override void ScrollDropEffect(Item item, PlayerController player)
    {
        player.ElementalDamage[(int)EnumTypes.ElementType.Corrosion] -= EnhanceAmount;
    }
}
