using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PoisonArea : MonoBehaviour
{
    private Renderer _renderer;
    private LayerMask _enemyLayer;

    private float areaTimer;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        _renderer = GetComponent<Renderer>();
        Color color = _renderer.material.color;
        _renderer.material.color = new Color(color.r, color.g, color.b, 1f);
        _enemyLayer = 1 << LayerMask.NameToLayer("Enemy");
        areaTimer = 0f;
        StartCoroutine(DealDamage());
        StartCoroutine(FadeOut());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (NetworkManager.Singleton.IsServer)
                enemy.BuffManager.AddBuff(new Decay(enemy, 5.0f, 50));
        }
    }

    IEnumerator DealDamage()
    {
        for (int i = 0; i < 4; i++)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                RaycastHit[] hits = Physics.BoxCastAll(transform.position, new Vector3(transform.lossyScale.x * 0.5f, 0.5f, transform.lossyScale.x * 0.5f), transform.up, transform.rotation, 0.5f, _enemyLayer);
                foreach (RaycastHit hit in hits)
                {
                    EnemyController enemy = hit.transform.GetComponent<EnemyController>();
                    enemy.TakeDamage(10, EnumTypes.ElementType.Corrosion);
                }
            }
            if (i == 3) break;
            yield return new WaitForSeconds(1f);
        }
        ObjectPool.Instance.Push(gameObject);
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(2f);
        Color color = _renderer.material.color;
        while (areaTimer < 1f)
        {
            float alphaRate = areaTimer;
            alphaRate *= alphaRate;
            _renderer.material.color = Color.Lerp(color, new Color(color.r, color.g, color.b, 0f), alphaRate);
            areaTimer += Time.deltaTime;
            yield return null;
        }
        _renderer.material.color = new Color(color.r, color.g, color.b, 0f);
    }
}
