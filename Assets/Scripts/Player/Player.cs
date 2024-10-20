using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player _instance = null;
    public static Player Instance
    {
        get
        {
            if (_instance == null) return null;
            return _instance;
        }
    }

    private PlayerStat _stat;
    private PlayerLook _look;
    private PlayerMove _move;
    private PlayerUI _ui;
    private PlayerWeapon _weapon;
    private InputManager _input;

    public PlayerStat PStat { get { return _stat; } }
    public PlayerLook Look { get { return _look; } }
    public PlayerMove Move { get { return _move; } }
    public PlayerUI UI { get { return _ui; } }
    public PlayerWeapon PWeapon { get { return _weapon; } }
    public InputManager Input { get { return _input; } }

    public int coin = 10000;

    public Camera mainCamera;
    public Camera weaponCamera;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            _stat = gameObject.GetComponent<PlayerStat>();
            _look = gameObject.GetComponent<PlayerLook>();
            _move = gameObject.GetComponent<PlayerMove>();
            _ui = gameObject.GetComponent<PlayerUI>();
            _weapon = gameObject.GetComponent<PlayerWeapon>();
            _input = gameObject.GetComponent<InputManager>();
        }
        else Destroy(gameObject);
    }

    void Update()
    {

    }

    public void GetItem(Item item)
    {
        switch(item.type)
        {
            case ItemType.Weapon:
                GameObject go = Instantiate(Resources.Load("Prefabs/Weapons/" + item.itemName) as GameObject);
                int index = go.name.IndexOf("(Clone)");
                if (index > 0)
                    go.name = go.name.Substring(0, index);
                go.transform.localPosition = item.equipedPos;
                go.transform.localRotation = Quaternion.Euler(item.equipedRot);
                go.layer = 9;
                PWeapon.EquipWeapon(go.GetComponent<Weapon>(), item.equipedPos, Quaternion.Euler(item.equipedRot));
                break;
            case ItemType.Scroll:
                PStat.GetScroll(item.scroll);
                break;
            case ItemType.AmmoSupply:
                PWeapon.ammo.Supply(item.ammoType, item.amount);
                break;
            case ItemType.HealthKit:
                PStat.RestoreHealth(item.amount);
                break;
        }
    }

    public void GetItem(GameObject go)
    {
        go.layer = 9;
        Item weapon = ItemTable.Instance.FindItem(go.name);
        PWeapon.EquipWeapon(go.GetComponent<Weapon>(), weapon.equipedPos, Quaternion.Euler(weapon.equipedRot));
    }

    public void MakePickup(GameObject go)
    {
        GameObject pickup = ObjectPool.Instance.Pop("Pickup", transform.position, Quaternion.identity);
        pickup.GetComponent<Pickup>().MakePickup(go);
        pickup.GetComponent<Rigidbody>().AddForce((transform.forward + transform.up) * 3.0f, ForceMode.Impulse);
    }

    public void MakePickup(Item item)
    {
        GameObject pickup = ObjectPool.Instance.Pop("Pickup", transform.position, Quaternion.identity);
        pickup.GetComponent<Pickup>().MakePickup(item);
        pickup.GetComponent<Rigidbody>().AddForce((transform.forward + transform.up) * 3.0f, ForceMode.Impulse);
    }
}
