using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerWeapon : MonoBehaviour
{
    public Weapon CurrentWeapon { get { return _currentWeapon; } }
    public int RemainSubSkills { get {  return _remainSubSkills; } }

    public Transform hand;
    public Transform skillPos;
    private int _currentWeaponNum;
    private Weapon _currentWeapon;

    private int _remainSubSkills;

    public Ammo ammo = new Ammo();

    void Awake()
    {

    }

    void Start()
    {
        ammo.Init();
        _currentWeaponNum = 0;
        _remainSubSkills = 8;
        Player.Instance.UI.SetSubSkillText(_remainSubSkills);
        SelectWeapon();
    }

    void Update()
    {

    }

    public Weapon GetWeapon(int weaponNum)
    {
        weaponNum--;
        if (weaponNum >=0 && weaponNum < hand.childCount)
        {
            return hand.transform.GetChild(weaponNum).GetComponent<Weapon>();
        }
        else return null;
    }

    public void EquipWeapon(Weapon weapon, Vector3 pos, Quaternion rot)
    {
        if(hand.childCount >= 3)
        {
            Player.Instance.MakePickup(_currentWeapon.gameObject);
            weapon.transform.SetParent(hand);
            weapon.transform.SetSiblingIndex(_currentWeaponNum);
            weapon.transform.localPosition = pos;
            weapon.transform.localRotation = rot;
            SelectWeapon();
        }
        else
        {
            weapon.transform.SetParent(hand);
            weapon.transform.localPosition = pos;
            weapon.transform.localRotation = rot;
            SelectWeaponNum(hand.childCount);
        }
    }

    private void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in hand)
        {
            if (i == _currentWeaponNum)
            {
                weapon.gameObject.SetActive(true);
                _currentWeapon = weapon.GetComponent<Weapon>();
            }
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
        ammo.currentAmmoType = _currentWeapon.ammoType;
        ammo.currentWeapon = _currentWeapon;
        Player.Instance.UI.SetWeaponIcon(_currentWeapon.weaponIcon);
        Player.Instance.UI.SetWeaponNum(_currentWeaponNum);
        Player.Instance.UI.SetAmmoSize(_currentWeapon.ammoType);
        Player.Instance.UI.SetAmmoText(_currentWeapon.ammoType, _currentWeapon.RemainAmmo, ammo.Check(_currentWeapon.ammoType));
    }

    public void SelectWeaponNum(int selectedNum)
    {
        selectedNum--;
        if (selectedNum < 0 || selectedNum >= hand.childCount) return;
        _currentWeaponNum = selectedNum;
        SelectWeapon();
    }

    public void ChangeToNextWeapon()
    {
        if (_currentWeaponNum >= hand.childCount - 1)
            _currentWeaponNum = 0;
        else
            _currentWeaponNum++;
        SelectWeapon();
    }

    public void ChangeToBeforeWeapon()
    {
        if (_currentWeaponNum == 0)
            _currentWeaponNum = hand.childCount - 1;
        else
            _currentWeaponNum--;
        SelectWeapon();
    }

    public void CastSkill()
    {
        GameObject eneryOrb = Instantiate(Resources.Load($"Prefabs/EnergyOrb") as GameObject, skillPos.position, skillPos.rotation);
        eneryOrb.GetComponent<Rigidbody>().velocity = Player.Instance.mainCamera.transform.forward * 20f;
    }

    public void CastSubSkill()
    {
        if (_remainSubSkills <= 0) return;
        GameObject poisonBomb = ObjectPool.Instance.Pop("PoisonBomb", skillPos.position, Quaternion.identity);
        poisonBomb.GetComponent<Rigidbody>().velocity = Player.Instance.mainCamera.transform.forward * 10f;
        _remainSubSkills--;
        Player.Instance.UI.SetSubSkillText(_remainSubSkills);
    }
}
