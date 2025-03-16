using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : Interactable
{
    private PlayerController _player;

    public Weapon weapon;

    public WeaponController weaponScript;
    public Rigidbody rb;
    public BoxCollider coll;

    public bool isEquipped;

    public void Awake()
    {
        promptMessage = weapon.ItemName;
    }

    private void Start()
    {
        if(isEquipped)
        {
            _player = GameManager.Instance.Player;
            weaponScript.enabled = true;
            rb.isKinematic = true;
            rb.useGravity = false;
            coll.isTrigger = true;
        }
        else
        {
            weaponScript.enabled = false;
            rb.isKinematic = false;
            rb.useGravity = true;
            coll.isTrigger = false;
        }
    }

    protected override void Interact(PlayerController player)
    {
        _player = player;
        isEquipped = true;
        if (player.IsOwner)
            gameObject.layer = LayerMask.NameToLayer("Weapon");
        else
            gameObject.layer = LayerMask.NameToLayer("Default");
        weaponScript.enabled = true;
        weaponScript.Init(player, weapon);

        _player.EquipWeapon(transform);

        //transform.SetParent(player.Hand);
        //transform.localPosition = Vector3.zero;
        //transform.localRotation = Quaternion.identity;
        //transform.localScale = Vector3.one;

        rb.isKinematic = true;
        rb.useGravity = false;
        coll.isTrigger = true;
    }

    public void Drop()
    {
        isEquipped = false;
        gameObject.layer = LayerMask.NameToLayer("Interactable");

        transform.SetParent(null);

        rb.isKinematic = false;
        rb.useGravity = true;
        coll.isTrigger = false;

        rb.linearVelocity = _player.GetComponent<Rigidbody>().linearVelocity;

        rb.AddForce(Camera.main.transform.forward *  3f, ForceMode.Impulse);
        rb.AddForce(Camera.main.transform.up * 3f, ForceMode.Impulse);

        float rand = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(rand, rand, rand) * 10f);

        weaponScript.enabled = false;
    }
}
