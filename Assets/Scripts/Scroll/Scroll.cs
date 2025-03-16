using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Scroll", menuName = "Scriptable Object/Scroll", order = 1)]
public class Scroll : Item
{
    public override void Gain(PlayerController player)
    {
        base.Gain(player);
        player.ScrollManager.AddScroll(ItemName);
    }

    public virtual void Drop(PlayerController player)
    {
        foreach(ItemEffect effect in GainEffects)
        {
            effect.ScrollDropEffect(this, player);
        }
        player.ScrollManager.RemoveScroll(ItemName, out _);
    }
}
