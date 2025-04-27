using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectManager : MonoBehaviour
{
    public PlayerController player;

    private Dictionary<EffectTriggerType, List<TriggeredEffect>> effectsByTrigger = new();

    public void RegisterItem(ItemData item)
    {
        foreach (var effect in item.effects)
        {
            if (!effectsByTrigger.ContainsKey(effect.trigger))
                effectsByTrigger[effect.trigger] = new List<TriggeredEffect>();

            var runtimeCopy = new TriggeredEffect
            {
                name = effect.name,
                trigger = effect.trigger,
                actionType = effect.actionType,
                value = effect.value,
                duration = effect.duration,
                cooldown = effect.cooldown,
                maxStacks = effect.maxStacks,
                condition = effect.condition,
                lastTriggeredTime = 0,
                currentStacks = 0,
            };

            effectsByTrigger[effect.trigger].Add(runtimeCopy);
        }
    }

    public void UnregisterItem(ItemData item)
    {
        foreach (var effect in item.effects)
        {
            if (!effectsByTrigger.TryGetValue(effect.trigger, out var effects))
            {
                effects.RemoveAll(e => e.name == effect.name);
            }
        }
    }

    public void Trigger(EffectTriggerType trigger)
    {
        if(!effectsByTrigger.TryGetValue(trigger, out var effects))
            return;

        foreach (var effect in effects)
        {
            if (!CanTrigger(effect))
                continue;
            ApplyEffect(effect);
            effect.lastTriggeredTime = Time.time;
            if(effect.maxStacks > 0 && effect.currentStacks < effect.maxStacks)
                effect.currentStacks++;
        }
    }

    private bool CanTrigger(TriggeredEffect effect)
    {
        // Check cooldown
        if (Time.time - effect.lastTriggeredTime < effect.cooldown)
            return false;

        // Check stack limit
        if (effect.currentStacks >= effect.maxStacks)
            return false;

        // Check condition (ex. "HP<50")
        // TODO: Implement condition string by list(using Editor code)
        

        return true;
    }

    private void ApplyEffect(TriggeredEffect effect)
    {
        switch(effect.actionType)
        {
            case EffectActionType.ModifyHP:
                if(effect.value < 0)
                    player.TakeDamage(effect.value);
                else
                    player.RestoreHealth(effect.value);
                break;
            case EffectActionType.ModifySpeed:
                player.SpeedFactor += effect.value;
                break;
            default:
                Debug.LogWarning($"Unhandled action type: {effect.actionType}");
                break;
        }
    }
}
