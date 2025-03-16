using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CoinEffect", menuName = "Scriptable Object/ItemEffect/CoinEffect", order = 1)]
public class CoinEffect : ItemEffect
{
    public int CoinAmount;
    public bool isPercentage;

    public override void ExecuteEffect(Item item, PlayerController player)
    {
        if (isPercentage)
            player.GainCoin(player.Coin.Value * (100 + CoinAmount) / 100);
        else
            player.GainCoin(CoinAmount);
    }

    public override void ScrollDropEffect(Item item, PlayerController player)
    {
        throw new System.NotImplementedException();
    }
}
