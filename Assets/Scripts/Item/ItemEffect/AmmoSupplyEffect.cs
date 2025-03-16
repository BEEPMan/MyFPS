using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AmmoSupplyEffect", menuName = "Scriptable Object/ItemEffect/AmmoSupplyEffect", order = 1)]
public class AmmoSupplyEffect : ItemEffect
{
    public int NormalAmmoAmount;
    public int LargeAmmoAmount;
    public int SpecialAmmoAmount;
    public bool isPercentage;

    public override void ExecuteEffect(Item item, PlayerController player)
    {
        if (isPercentage)
        {
            Mathf.Clamp(player.Ammos[(int)EnumTypes.AmmoType.Normal]+ Global.MaxNormalAmmo * (NormalAmmoAmount) / 100, 0, player.MaxAmmos[(int)EnumTypes.AmmoType.Normal]);
            Mathf.Clamp(player.Ammos[(int)EnumTypes.AmmoType.Large] + Global.MaxLargeAmmo * (LargeAmmoAmount) / 100, 0, player.MaxAmmos[(int)EnumTypes.AmmoType.Large]);
            Mathf.Clamp(player.Ammos[(int)EnumTypes.AmmoType.Special] + Global.MaxSpecialAmmo * (SpecialAmmoAmount) / 100, 0, player.MaxAmmos[(int)EnumTypes.AmmoType.Special]);        }
        else
        {
            Mathf.Clamp(player.Ammos[(int)EnumTypes.AmmoType.Normal] + NormalAmmoAmount, 0, player.MaxAmmos[(int)EnumTypes.AmmoType.Normal]);
            Mathf.Clamp(player.Ammos[(int)EnumTypes.AmmoType.Large] + LargeAmmoAmount, 0, player.MaxAmmos[(int)EnumTypes.AmmoType.Large]);
            Mathf.Clamp(player.Ammos[(int)EnumTypes.AmmoType.Special] + SpecialAmmoAmount, 0, player.MaxAmmos[(int)EnumTypes.AmmoType.Special]);
        }
        EnumTypes.AmmoType ammoType = player.CurrentWeapon.WeaponData.ammoType;
        UIManager.Instance.InGame.UpdateAmmoText(ammoType, player.CurrentWeapon.RemainAmmo, player.Ammos[(int)ammoType]);
        UIManager.Instance.InGame.UpdateAmmoFillAmount(EnumTypes.AmmoType.Normal, player.Ammos[(int)EnumTypes.AmmoType.Normal], player.MaxAmmos[(int)EnumTypes.AmmoType.Normal]);
        UIManager.Instance.InGame.UpdateAmmoFillAmount(EnumTypes.AmmoType.Large, player.Ammos[(int)EnumTypes.AmmoType.Large], player.MaxAmmos[(int)EnumTypes.AmmoType.Large]);
        UIManager.Instance.InGame.UpdateAmmoFillAmount(EnumTypes.AmmoType.Special, player.Ammos[(int)EnumTypes.AmmoType.Special], player.MaxAmmos[(int)EnumTypes.AmmoType.Special]);
    }

    public override void ScrollDropEffect(Item item, PlayerController player)
    {
        throw new System.NotImplementedException();
    }
}
