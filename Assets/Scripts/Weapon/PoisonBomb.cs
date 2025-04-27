using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PoisonBomb : NetworkBehaviour
{
    private LayerMask _groundLayer;

    [SerializeField]
    private GameObject m_PoisonArea_Prefab;

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
            NetworkObject poisonArea = NetworkObjectPool.Instance.GetNetworkObject("PoisonArea", hitInfo.point, Quaternion.identity);
            poisonArea.Spawn();
            NetworkObject.Despawn(gameObject);
        }
    }
}
