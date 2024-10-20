using System.Collections;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum ElementalType
{
    None,
    Fire,
    Lightning,
    Corrosion,
}

public abstract class Weapon : MonoBehaviour
{
    public int RemainAmmo
    {
        get { return remainAmmo; }
        set
        {
            remainAmmo = value;
        }
    }

    protected Vector3 currentRotation;
    protected Vector3 targetRotation;
    public Vector3 CurrentRotation { get { return currentRotation; } }

    //Hipfire Recoil
    [SerializeField] protected float recoilX;
    [SerializeField] protected float recoilY;
    [SerializeField] protected float recoilZ;

    //Settings
    [SerializeField] protected float snappiness;
    [SerializeField] protected float returnSpeed;

    public AmmoType ammoType = AmmoType.Infinite;
    [SerializeField] protected int maxAmmo;
    [SerializeField] protected float bulletDamage;
    [SerializeField] protected float range;

    [SerializeField] protected float fireRate;
    [SerializeField] protected float reloadTime;

    [SerializeField] protected ElementalType elementalType = ElementalType.None;
    [SerializeField] protected int elementalRate = 0;

    public Sprite weaponIcon;

    public Transform firePoint;
    public GameObject bulletHole;
    protected int remainAmmo;
    protected float fireTimer = -1f;
    protected float reloadTimer = -1f;

    public Camera cam;

    public abstract void Fire();
    public abstract void Skill();

    void Awake()
    {
        RemainAmmo = maxAmmo;
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        OnUpdate();
    }

    protected virtual void Init()
    {
        cam = Camera.main;
    }
        
    protected virtual void OnUpdate()
    {
        if (reloadTimer > 0)
        {
            reloadTimer += Time.deltaTime;
            if (reloadTimer > reloadTime)
            {
                RemainAmmo += Player.Instance.PWeapon.ammo.Reload(ammoType, remainAmmo, maxAmmo);
                reloadTimer = -1.0f;
            }
        }
        if (fireTimer > 0)
        {
            fireTimer += Time.deltaTime;
            if (fireTimer > 1 / fireRate)
            {
                fireTimer = -1.0f;
            }
        }

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
    }

    public void Reload()
    {
        // TODO: Do not Reload if Ammo.Check() is zero.
        if (reloadTimer < 0 && remainAmmo < maxAmmo)
            reloadTimer = Time.deltaTime;
    }

    public virtual void RecoilFire()
    {
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }

    protected IEnumerator CreateBulletHole(Vector3 position, Quaternion rotation)
    {
        GameObject bulletHoleGO = ObjectPool.Instance.Pop("BulletHole", position, rotation);
        yield return new WaitForSeconds(5.0f);
        ObjectPool.Instance.Push(bulletHoleGO);
    }

    protected void TriggerElementalEffect(EnemyStat target)
    {
        if (elementalType == ElementalType.None) return;
        int prob = Random.Range(0, 100);
        if(prob < elementalRate)
        {
            string elementalEffect = elementalType switch
            {
                ElementalType.Fire => "Burning",
                ElementalType.Lightning => "Shock",
                ElementalType.Corrosion => "Decay",
                _ => ""
            };
            target.AddBuff(Player.Instance.PStat, elementalEffect, 5.0f);
        }
    }

    public virtual string GetDescription()
    {
        StringBuilder sbDescription = new StringBuilder();
        sbDescription.Append("사거리: ");
        sbDescription.Append(range);
        sbDescription.Append("\n대미지: ");
        sbDescription.Append(bulletDamage);
        sbDescription.Append("\n탄창용량: ");
        sbDescription.Append(maxAmmo);
        sbDescription.Append("\n초당 발사량: ");
        sbDescription.Append(fireRate);
        return sbDescription.ToString();
    }
}
