using System.Collections;
using System.Collections.Generic;
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
            EnemyStat enemy = other.GetComponent<EnemyStat>();
            enemy.AddBuff(Player.Instance.PStat, "Decay", 5.0f);
        }
    }

    IEnumerator DealDamage()
    {
        for (int i = 0; i < 4; i++)
        {
            RaycastHit[] hits = Physics.BoxCastAll(transform.position, new Vector3(transform.lossyScale.x * 0.5f, 0.5f, transform.lossyScale.x * 0.5f), transform.up, transform.rotation, 0.5f, _enemyLayer);
            foreach (RaycastHit hit in hits)
            {
                Stat enemy = hit.transform.GetComponent<Stat>();
                enemy.TakeDamage(10f);
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
