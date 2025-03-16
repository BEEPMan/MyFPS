using Cysharp.Threading.Tasks;
using EnumTypes;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : BaseController
{
    public NetworkVariable<int> SubSkillRemain = new NetworkVariable<int>();
    public NetworkVariable<int> Coin = new NetworkVariable<int>();
    public WeaponController[] Weapons { get; set; }
    public NetworkVariable<int> CurrentWeaponNum = new NetworkVariable<int>();
    public WeaponController CurrentWeapon { get { return Weapons[CurrentWeaponNum.Value]; } }
    
    public int[] Ammos;
    public int[] MaxAmmos;
    public ScrollManager ScrollManager { get; set; }

    public PlayerMove Move;
    public PlayerLook Look;

    public Camera weaponCamera;
    public Camera minimapCamera;

    public Transform Hand;
    public Transform HandWeapons;
    public Transform SkillPos;
    public Transform GroundCheck;

    public bool isStaggered;

    private float mainSkillTimer;
    private float subSkillTimer;
    private float dashTimer;

    void Awake()
    {
        ScrollManager = new ScrollManager();
        Weapons = new WeaponController[3];
        Ammos = new int[4]
        {
            0,
            Global.MaxNormalAmmo / 2,
            Global.MaxLargeAmmo / 2,
            Global.MaxSpecialAmmo / 2,
        };
        MaxAmmos = new int[4]
        {
            0,
            Global.MaxNormalAmmo,
            Global.MaxLargeAmmo,
            Global.MaxSpecialAmmo,
        };
        Move = new PlayerMove();
        Look = new PlayerLook();
        int i = 0;
        foreach (Transform weapon in HandWeapons)
        {
            Weapons[i] = weapon.GetComponent<WeaponController>();
            Weapons[i].Init(this);
            Weapons[i].GetComponent<WeaponPickUp>().isEquipped = true;
            i++;
        }
        mainSkillTimer = Global.MainSkillCoolTime;
        subSkillTimer = Global.SubSkillCoolTime;
        dashTimer = Global.DashCoolTime;
        isStaggered = false;
    }

    protected override void Start()
    {
        base.Start();
        Move.Init(this);
        Look.Init(this);
        if(UIManager.Instance.InGame != null)
        {
            UIManager.Instance.InGame.UpdateHPType(HPType);
            UIManager.Instance.InGame.UpdateAmmoFillAmount(AmmoType.Normal, Ammos[(int)AmmoType.Normal], MaxAmmos[(int)AmmoType.Normal]);
            UIManager.Instance.InGame.UpdateAmmoFillAmount(AmmoType.Large, Ammos[(int)AmmoType.Large], MaxAmmos[(int)AmmoType.Large]);
            UIManager.Instance.InGame.UpdateAmmoFillAmount(AmmoType.Special, Ammos[(int)AmmoType.Special], MaxAmmos[(int)AmmoType.Special]);
        }
        SelectWeapon();
    }

    protected override void Update()
    {
        base.Update();
        Move.OnUpdate();
        Look.OnUpdate();

        mainSkillTimer += Time.deltaTime;
        subSkillTimer += Time.deltaTime;
        dashTimer += Time.deltaTime;
    }

    public override void OnNetworkSpawn()
    {
        SubSkillRemain.OnValueChanged += OnSubSkillRemainChanged;
        CurrentWeaponNum.OnValueChanged += OnWeaponNumChanged;
        if (NetworkManager.Singleton.IsServer)
        {
            SubSkillRemain.Value = Global.MaxSubSkillCount / 2;
            Coin.Value = 100;
            CurrentWeaponNum.Value = 0;
        }
        if (IsOwner)
        {
            Camera.main.transform.SetParent(Hand);
            Instantiate(weaponCamera, Hand);
            Instantiate(minimapCamera, transform);
            GameManager.Instance.OnPlayerSpawned(this);
        }
        else
        {
            foreach (WeaponController weapon in Weapons)
            {
                weapon.gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        SubSkillRemain.OnValueChanged -= OnSubSkillRemainChanged;
        CurrentWeaponNum.OnValueChanged -= OnWeaponNumChanged;
    }

    public void OnSubSkillRemainChanged(int oldValue, int newValue)
    {
        if (IsOwner && UIManager.Instance.InGame != null)
        {
            UIManager.Instance.InGame.UpdateSubSkillText(SubSkillRemain.Value);
        }
    }

    public void GainCoin(int amount)
    {
        GainCoinServerRPC(amount);
    }

    [Rpc(SendTo.Server)]
    public void GainCoinServerRPC(int amount)
    {
        Coin.Value += amount;
    }

    public void InitUI()
    {
        UIManager.Instance.InGame.UpdateHPType(HPType);
        UIManager.Instance.InGame.UpdateAmmoFillAmount(AmmoType.Normal, Ammos[(int)AmmoType.Normal], MaxAmmos[(int)AmmoType.Normal]);
        UIManager.Instance.InGame.UpdateAmmoFillAmount(AmmoType.Large, Ammos[(int)AmmoType.Large], MaxAmmos[(int)AmmoType.Large]);
        UIManager.Instance.InGame.UpdateAmmoFillAmount(AmmoType.Special, Ammos[(int)AmmoType.Special], MaxAmmos[(int)AmmoType.Special]);
        UIManager.Instance.InGame.UpdateCurrentAmmoType(CurrentWeapon.WeaponData.ammoType);
        UIManager.Instance.InGame.UpdateAmmoText(CurrentWeapon.WeaponData.ammoType, CurrentWeapon.RemainAmmo, Ammos[(int)CurrentWeapon.WeaponData.ammoType]);
        UIManager.Instance.InGame.UpdateWeaponIcon(CurrentWeapon.WeaponData.Icon);
        UIManager.Instance.InGame.UpdateWeaponNum(CurrentWeaponNum.Value);
        UIManager.Instance.InGame.UpdateSubSkillText(SubSkillRemain.Value);
        UIManager.Instance.InGame.SetPromptText(string.Empty);
    }

    #region Skill
    public void CastMainSkill()
    {
        MainSkillServerRPC();
    }

    [Rpc(SendTo.Server)]
    void MainSkillServerRPC()
    {
        if (mainSkillTimer < Global.MainSkillCoolTime) return;
        GameObject eneryOrb = Instantiate(Resources.Load($"Prefabs/EnergyOrb") as GameObject, SkillPos.position, SkillPos.rotation);
        eneryOrb.GetComponent<Rigidbody>().linearVelocity = Hand.forward * 20f;
        mainSkillTimer = 0f;
        MainSkillClientRPC();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void MainSkillClientRPC()
    {
        if (!IsHost)
        {
            GameObject eneryOrb = Instantiate(Resources.Load($"Prefabs/EnergyOrb") as GameObject, SkillPos.position, SkillPos.rotation);
            eneryOrb.GetComponent<Rigidbody>().linearVelocity = Hand.forward * 20f;
            mainSkillTimer = 0f;
        }
        if (IsOwner)
        {
            UIManager.Instance.InGame.UpdateMainSkillCoolDown(mainSkillTimer, Global.MainSkillCoolTime);
        }
    }

    public void CastSubSkill()
    {
        SubSkillServerRPC();
    }

    [Rpc(SendTo.Server)]
    void SubSkillServerRPC()
    {
        if (subSkillTimer < Global.SubSkillCoolTime || SubSkillRemain.Value <= 0) return;
        GameObject poisonBomb = ObjectPool.Instance.Pop("PoisonBomb", SkillPos.position, Quaternion.identity);
        poisonBomb.GetComponent<Rigidbody>().linearVelocity = Hand.forward * 10f;
        SubSkillRemain.Value--;
        subSkillTimer = 0f;
        SubSkillClientRPC();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void SubSkillClientRPC()
    {
        if (!IsHost)
        {
            GameObject poisonBomb = ObjectPool.Instance.Pop("PoisonBomb", SkillPos.position, Quaternion.identity);
            poisonBomb.GetComponent<Rigidbody>().linearVelocity = Hand.forward * 10f;
        }
        if (IsOwner)
        {
            subSkillTimer = 0f;
            UIManager.Instance.InGame.UpdateSubSkillCoolDown(subSkillTimer, Global.SubSkillCoolTime);
        }
    }
    #endregion

    #region Weapon
    [Rpc(SendTo.Server)]
    protected override void AttackServerRPC()
    {
        CurrentWeapon.TryAttack().Forget();
        AttackClientRPC();
    }

    [Rpc(SendTo.ClientsAndHost)]
    protected override void AttackClientRPC()
    {
        if (!IsHost)
        {
            CurrentWeapon.TryAttack().Forget();
        }
    }

    public void Reload()
    {
        ReloadServerRPC();
    }

    [Rpc(SendTo.Server)]
    void ReloadServerRPC()
    {
        CurrentWeapon.Reload().Forget();
        ReloadClientRPC();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void ReloadClientRPC()
    {
        if (!IsHost)
        {
            CurrentWeapon.Reload().Forget();
        }
    }

    public void EquipWeapon(Transform weaponTransform)
    {
        if (HandWeapons.childCount >= 3)
        {
            DropWeapon(CurrentWeaponNum.Value, false);
            Weapons[CurrentWeaponNum.Value] = weaponTransform.GetComponent<WeaponController>();
            weaponTransform.SetParent(HandWeapons);
            weaponTransform.localPosition = Vector3.zero;
            weaponTransform.localRotation = Quaternion.identity;
            weaponTransform.localScale = Vector3.one;
            ChangeWeaponNum(CurrentWeaponNum.Value);
            //SelectWeapon();
        }
        else
        {
            int i = 0;
            for (i = 0; i < 3; i++)
            {
                if (Weapons[i] == null) break;
            }
            Weapons[i] = weaponTransform.GetComponent<WeaponController>();
            weaponTransform.SetParent(HandWeapons);
            weaponTransform.SetSiblingIndex(i);
            weaponTransform.localPosition = Vector3.zero;
            weaponTransform.localRotation = Quaternion.identity;
            weaponTransform.localScale = Vector3.one;
            SelectWeaponNum(i);
        }
    }

    public void DropWeapon(int dropNum, bool autoChange = true)
    {
        DropWeaponServerRPC(dropNum, autoChange);
    }

    [Rpc(SendTo.Server)]
    void DropWeaponServerRPC(int dropNum, bool autoChange)
    {
        if (HandWeapons.childCount == 1) return;
        if (dropNum == CurrentWeaponNum.Value && autoChange)
        {
            ChangeToNextWeapon();
        }
        Weapons[dropNum].gameObject.SetActive(true);
        Weapons[dropNum].GetComponent<WeaponPickUp>().Drop();
        Weapons[dropNum] = null;
        DropWeaponClientRPC(dropNum, autoChange);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void DropWeaponClientRPC(int dropNum, bool autoChange)
    {
        if (!IsHost)
        {
            if (dropNum == CurrentWeaponNum.Value && autoChange)
            {
                ChangeToNextWeapon();
            }
            Weapons[dropNum].gameObject.SetActive(true);
            Weapons[dropNum].GetComponent<WeaponPickUp>().Drop();
            Weapons[dropNum] = null;
        }
    }

    private void SelectWeapon()
    {
        SelectWeaponServerRPC();
    }

    [Rpc(SendTo.Server)]
    public void SelectWeaponServerRPC()
    {
        for (int i = 0; i < 3; i++)
        {
            if (Weapons[i] == null) continue;
            if (i == CurrentWeaponNum.Value)
                Weapons[i].gameObject.SetActive(true);
            else
                Weapons[i].gameObject.SetActive(false);
        }
        SelectWeaponClientRPC();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SelectWeaponClientRPC()
    {
        if(!IsHost)
        {
            for (int i = 0; i < 3; i++)
            {
                if (Weapons[i] == null) continue;
                if (i == CurrentWeaponNum.Value)
                    Weapons[i].gameObject.SetActive(true);
                else
                    Weapons[i].gameObject.SetActive(false);
            }
        }
        if(IsOwner)
        {
            Weapon weaponData = CurrentWeapon.WeaponData;
            Sprite weaponIcon = weaponData.Icon;
            if (UIManager.Instance.InGame != null)
            {
                UIManager.Instance.InGame.UpdateCurrentAmmoType(weaponData.ammoType);
                UIManager.Instance.InGame.UpdateAmmoText(CurrentWeapon.WeaponData.ammoType, CurrentWeapon.RemainAmmo, Ammos[(int)CurrentWeapon.WeaponData.ammoType]);
                UIManager.Instance.InGame.UpdateWeaponIcon(weaponIcon);
                UIManager.Instance.InGame.UpdateWeaponNum(CurrentWeaponNum.Value);
            }
        }
    }

    public void SelectWeaponNum(int selectedNum)
    {
        if (selectedNum < 0 || selectedNum >= HandWeapons.childCount) return;
        if (Weapons[selectedNum] == null) return;
        ChangeWeaponNum(selectedNum);
        //SelectWeapon();
    }

    public void ChangeToNextWeapon()
    {
        do
        {
            if (CurrentWeaponNum.Value >= 2)
                ChangeWeaponNum(0);
            else
                ChangeWeaponNum(CurrentWeaponNum.Value + 1);
        } while (CurrentWeapon == null);
        //SelectWeapon();
    }

    public void ChangeToBeforeWeapon()
    {
        do
        {
            if (CurrentWeaponNum.Value == 0)
                ChangeWeaponNum(2);
            else
                ChangeWeaponNum(CurrentWeaponNum.Value - 1);
        } while (CurrentWeapon == null);
        //SelectWeapon();
    }

    public void ChangeWeaponNum(int num)
    {
        ChangeWeaponNumRPC(num);
    }

    [Rpc(SendTo.Server)]
    public void ChangeWeaponNumRPC(int num)
    {
        CurrentWeaponNum.Value = num;
    }

    public void OnWeaponNumChanged(int oldValue, int newValue)
    {
        for (int i = 0; i < 3; i++)
        {
            if (Weapons[i] == null) continue;
            if (i == newValue)
                Weapons[i].gameObject.SetActive(true);
            else
                Weapons[i].gameObject.SetActive(false);
        }
        Debug.Log(newValue);
        if (IsOwner)
        {
            Weapon weaponData = Weapons[newValue].WeaponData;
            Sprite weaponIcon = weaponData.Icon;
            if (UIManager.Instance.InGame != null)
            {
                UIManager.Instance.InGame.UpdateCurrentAmmoType(weaponData.ammoType);
                UIManager.Instance.InGame.UpdateAmmoText(weaponData.ammoType, Weapons[newValue].RemainAmmo, Ammos[(int)weaponData.ammoType]);
                UIManager.Instance.InGame.UpdateWeaponIcon(weaponIcon);
                UIManager.Instance.InGame.UpdateWeaponNum(newValue);
            }
        }
    }
    #endregion

    public override void TakeDamage(int damage, EnumTypes.ElementType elementType = EnumTypes.ElementType.None, bool isTrueDamage = false)
    {
        int finalDamage = damage;
        switch (HPType)
        {
            case EnumTypes.HPType.HPOnly:
                if (!isTrueDamage)
                {
                    finalDamage = damage * (100 + TakenDamage + CalcDamageByHPType(HPType, elementType)) / 100;
                }
                HP.Value = Mathf.Clamp(HP.Value - finalDamage, 0, MaxHP.Value);
                if (UIManager.Instance.InGame != null)
                {
                    UIManager.Instance.InGame.UpdateHPBar(HP.Value, MaxHP.Value);
                    UIManager.Instance.InGame.ShowDamageOverlay(HP.Value);
                }
                break;
            case EnumTypes.HPType.Shield:
                if (Shield.Value > 0)
                {
                    if (!isTrueDamage)
                    {
                        finalDamage = damage * (100 + TakenDamage + CalcDamageByHPType(HPType, elementType)) / 100;
                    }
                    Shield.Value = Mathf.Clamp(Shield.Value - finalDamage, 0, MaxShield.Value);
                    if (UIManager.Instance.InGame != null)
                    {
                        UIManager.Instance.InGame.UpdateSABar(Shield.Value, MaxShield.Value);
                    }
                }
                else
                {
                    if (!isTrueDamage)
                    {
                        finalDamage = damage * (100 + TakenDamage + CalcDamageByHPType(EnumTypes.HPType.HPOnly, elementType)) / 100;
                    }
                    HP.Value = Mathf.Clamp(HP.Value - finalDamage, 0, MaxHP.Value);
                    if (UIManager.Instance.InGame != null)
                    {
                        UIManager.Instance.InGame.UpdateHPBar(HP.Value, MaxHP.Value);
                        UIManager.Instance.InGame.ShowDamageOverlay(HP.Value);
                    }
                }
                break;
            case EnumTypes.HPType.Armor:
                if (Armor.Value > 0)
                {
                    if (!isTrueDamage)
                    {
                        finalDamage = damage * (100 + TakenDamage + CalcDamageByHPType(HPType, elementType)) / 100;
                    }
                    Armor.Value = Mathf.Clamp(Armor.Value - finalDamage, 0, MaxArmor.Value);
                    if (UIManager.Instance.InGame != null)
                    {
                        UIManager.Instance.InGame.UpdateSABar(Armor.Value, MaxArmor.Value);
                    }
                }
                else
                {
                    if (!isTrueDamage)
                    {
                        finalDamage = damage * (100 + TakenDamage + CalcDamageByHPType(EnumTypes.HPType.HPOnly, elementType)) / 100;
                    }
                    HP.Value = Mathf.Clamp(HP.Value - finalDamage, 0, MaxHP.Value);
                    if (UIManager.Instance.InGame != null)
                    {
                        UIManager.Instance.InGame.UpdateHPBar(HP.Value, MaxHP.Value);
                        UIManager.Instance.InGame.ShowDamageOverlay(HP.Value);
                    }
                }
                break;
        }
        if(NetworkManager.Singleton.IsServer)
            TriggerElementalEffect(elementType, finalDamage);
        if (HP.Value <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {

    }

    public override void RestoreHealth(int healAmount)
    {
        base.RestoreHealth(healAmount);
        if (UIManager.Instance.InGame != null)
        {
            UIManager.Instance.InGame.UpdateHPBar(HP.Value, MaxHP.Value);
        }
    }

    public override void AddBuff(BaseBuff newBuff)
    {
        if (newBuff is Manipulation) return;
        base.AddBuff(newBuff);
    }

    public void Dash(Vector2 input)
    {
        if (dashTimer < Global.DashCoolTime) return;
        Move.Dash(input);
        dashTimer = 0f;
        if (UIManager.Instance.InGame != null)
        {
            UIManager.Instance.InGame.UpdateDashCoolDown(dashTimer, Global.DashCoolTime);
        }
    }
}
