using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniferRifle : WeaponController
{
    public bool isAiming = false;

    public override void Skill()
    {
        if (isAiming) ZoomOut();
        else ZoomIn();
    }

    protected void Zoom(Camera cam, float zoomRate)
    {
        cam.DOFieldOfView(Global.OriginalFOV / zoomRate, 0.5f);
    }

    protected void ZoomIn()
    {
        GameManager.Instance.Player.weaponCamera.enabled = false;
        Zoom(Camera.main, WeaponData.zoomRate);
        isAiming = true;
    }

    protected void ZoomOut()
    {
        GameManager.Instance.Player.weaponCamera.enabled = true;
        Zoom(Camera.main, 1f);
        isAiming = false;
    }
}
