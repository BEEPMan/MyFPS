using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniferRifle : Weapon
{
    //ADS Recoil
    [SerializeField] protected float aimRecoilX;
    [SerializeField] protected float aimRecoilY;
    [SerializeField] protected float aimRecoilZ;
    [SerializeField] protected float zoomRate;

    public bool isAiming = false;
    private Vector3 originalPos;
    private float originalFOV;
    private float zoomTimer;

    protected override void Init()
    {
        base.Init();
        originalPos = transform.localPosition;
        originalFOV = Player.Instance.mainCamera.fieldOfView;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (isAiming)
        {
            zoomTimer += Time.deltaTime;
            if (Player.Instance.mainCamera.fieldOfView > originalFOV / zoomRate)
            {
                float zoomRatio = zoomTimer;
                zoomRatio *= zoomRatio;
                Vector3 destPos = new Vector3(0f, -0.2f, originalPos.z);
                transform.localPosition = Vector3.Lerp(transform.localPosition, destPos, zoomRatio);
                Player.Instance.mainCamera.fieldOfView = Mathf.Lerp(Player.Instance.mainCamera.fieldOfView, originalFOV / zoomRate, zoomRatio);
                if(zoomRatio > 0.2f) Player.Instance.weaponCamera.enabled = false;
            }
        }
        else
        {
            zoomTimer += Time.deltaTime;
            if (Player.Instance.mainCamera.fieldOfView < originalFOV)
            {
                float zoomRate = zoomTimer;
                zoomRate *= zoomRate;
                transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, zoomRate);
                Player.Instance.mainCamera.fieldOfView = Mathf.Lerp(Player.Instance.mainCamera.fieldOfView, originalFOV, zoomRate);
            }
        }
    }

    public override void Fire()
    {
        if (RemainAmmo <= 0 || fireTimer > 0 || reloadTimer > 0)
        {
            return;
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, range))
        {
            EnemyStat target;
            if (target = hitInfo.transform.GetComponent<EnemyStat>())
            {
                float damage = Player.Instance.PStat.damageCalculator.CalculateGivenDamage(bulletDamage, elementalType, true, false);
                target.TakeDamage(damage, elementalType, Player.Instance.PStat);
                TriggerElementalEffect(target);
            }
            else if (hitInfo.transform.gameObject.activeSelf)
            {
                StartCoroutine(CreateBulletHole(hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal)));
            }
        }

        RecoilFire();
        RemainAmmo--;
        Player.Instance.UI.SetAmmoText(ammoType, RemainAmmo, Player.Instance.PWeapon.ammo.Check(ammoType));
        if (RemainAmmo <= 0)
        {
            Reload();
        }
        else
            fireTimer = Time.deltaTime;
    }

    public override void Skill()
    {
        isAiming = !isAiming;
        if(isAiming) ZoomIn();
        else ZoomOut();
    }

    public void ZoomIn()
    {
        originalPos = transform.localPosition;
        originalFOV = Player.Instance.mainCamera.fieldOfView;
        zoomTimer = 0f;
    }

    public void ZoomOut()
    {
        Player.Instance.weaponCamera.enabled = true;
        zoomTimer = 0f;
    }

    public override void RecoilFire()
    {
        if (isAiming) targetRotation += new Vector3(aimRecoilX, Random.Range(-aimRecoilY, aimRecoilY), Random.Range(-aimRecoilZ, aimRecoilZ));
        else base.RecoilFire();
    }
}
