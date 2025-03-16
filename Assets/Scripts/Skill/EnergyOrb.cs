using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnergyOrb : MonoBehaviour
{
    [SerializeField] private float orbRange;
    private float explodeTimer;
    private bool isExploding;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyController enemy;
        if (NetworkManager.Singleton.IsServer)
            if (enemy = other.GetComponent<EnemyController>()) enemy.BuffManager.AddBuff(new Freeze(enemy, 3.0f));
        if (!isExploding)
        {
            explodeTimer = 0f;
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            StartCoroutine(Explode());
        }
        isExploding = true;
    }

    private IEnumerator Explode()
    {
        Renderer renderer = GetComponent<Renderer>();
        Color color = renderer.material.color;
        while (explodeTimer < 0.5f)
        {
            float scaleRate = explodeTimer;
            scaleRate *= scaleRate;
            transform.localScale = Vector3.Lerp(Vector3.one * 0.3f, Vector3.one * orbRange, scaleRate * 4);
            renderer.material.color = Color.Lerp(color, new Color(color.r, color.g, color.b, 0f), scaleRate * 4);
            explodeTimer += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.one * orbRange;
        renderer.material.color = new Color(color.r, color.g, color.b, 0f);
        Destroy(gameObject);
    }
}
