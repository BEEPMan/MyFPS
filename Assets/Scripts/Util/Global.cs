using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class Global
{
    public const float OriginalFOV = 60.0f;

    public const int MaxNormalAmmo = 450;
    public const int MaxLargeAmmo = 120;
    public const int MaxSpecialAmmo = 30;

    public const int InitialCoin = 10000;
    public const int MaxSubSkillCount = 8;

    public const float JumpCoolTime = 0.3f;
    public const float DashCoolTime = 1.0f;
    public const float MainSkillCoolTime = 5.0f;
    public const float SubSkillCoolTime = 0.3f;

    public const int NumOfPeddlerItems = 8;

    public static class CharacterTag
    {
        public static readonly string NPC = "NPC";
        public static readonly string Player = "Player";
        public static readonly string Enemy = "Enemy";
    }

    public static class ObjectLayer
    {
        public static readonly string Player = "Player";
        public static readonly string Enemy = "Enemy";
        public static readonly string Interactable = "Interactable";
    }

    public static class UIPopUp
    {
        public static readonly string Craftsman = "UI_Craftsman";
        public static readonly string ScrollInventory = "UI_Inventory_Scroll";
        public static readonly string WeaponInventory = "UI_Inventory_Weapon";
        public static readonly string Peddler = "UI_Peddler";
    }

    public static Dictionary<string, int> MaxStackCount = new Dictionary<string, int>()
    {
        { "Miasma", 9 }
    };
}

namespace EnumTypes
{
    public enum ItemType
    {
        None,
        Weapon,
        HealthKit,
        AmmoSupply,
        Scroll
    }

    public enum AmmoType
    {
        Infinite,
        Normal,
        Large,
        Special
    }

    public enum CharacterType
    {
        NPC,
        Player,
        Enemy,
        Boss
    }

    public enum HPType
    {
        HPOnly,
        Shield,
        Armor
    }

    public enum ElementType
    {
        None,
        Fire,
        Lightning,
        Corrosion
    }

    public enum DamageType
    {
        WeaponDamage,
        SkillDamage,
    }
}