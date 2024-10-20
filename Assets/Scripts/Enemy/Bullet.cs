using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public string TargetTag { get { return _targetTag; } set { _targetTag = value; } }

    private string _targetTag;

    private void OnCollisionEnter(Collision collision)
    {
        Transform hitTransform = collision.transform;
        if(hitTransform.CompareTag(_targetTag))
        {
            hitTransform.GetComponent<Stat>().TakeDamage(10);
        }
        Destroy(gameObject);
    }
}
