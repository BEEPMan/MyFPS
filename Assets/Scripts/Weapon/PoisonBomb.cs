using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonBomb : MonoBehaviour
{
    private LayerMask _groundLayer;

    void Start()
    {
        _groundLayer = 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Interactable");
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 0.2f, _groundLayer))
        {
            GameObject poisonArea = ObjectPool.Instance.Pop("PoisonArea", hitInfo.point, Quaternion.identity);
            ObjectPool.Instance.Push(gameObject);
        }
    }
}
