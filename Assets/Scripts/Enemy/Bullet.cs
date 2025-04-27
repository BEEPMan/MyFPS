using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using Unity.Netcode;
using System.IO.Pipes;

public class Bullet : MonoBehaviour
{
    public string TargetTag { get { return _targetTag; } set { _targetTag = value; } }

    private string _targetTag;

    public Vector3 direction;

    void OnEnable()
    {
        //transform.Rotate(direction, Space.Self);
        //GetComponent<Rigidbody>().linearVelocity = direction;
        DestroyBullet().Forget();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Transform hitTransform = collision.transform;
            if (hitTransform.CompareTag(_targetTag))
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
        if (gameObject.activeSelf)
            ObjectPool.Instance.Push(gameObject);
    }
}
