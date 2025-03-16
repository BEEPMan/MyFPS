using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using Unity.Netcode;

public class Bullet : MonoBehaviour
{
    public string TargetTag { get { return _targetTag; } set { _targetTag = value; } }

    private string _targetTag;

    private void OnEnable()
    {
        DestroyBullet().Forget();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Transform hitTransform = collision.transform;
        if(hitTransform.CompareTag(_targetTag))
        {
            if (NetworkManager.Singleton.IsServer)
            {
                if (_targetTag == Global.CharacterTag.Player)
                    hitTransform.GetComponent<PlayerController>().TakeDamage(1);
                else if (_targetTag == Global.CharacterTag.Enemy)
                    hitTransform.GetComponent<EnemyController>().TakeDamage(1);
            }
        }
        ObjectPool.Instance.Push(gameObject);
    }

    private async UniTaskVoid DestroyBullet()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(5.0f));
        ObjectPool.Instance.Push(gameObject);
    }
}
