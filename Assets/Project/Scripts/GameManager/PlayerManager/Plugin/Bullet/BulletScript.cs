using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour
{
    private UnityNightPool.PoolObject poolObject;
    [SerializeField] private LayerMask botMask;
    [SerializeField] private float maxLifeTime = 2f;
    [SerializeField] private float explosionRadius = 0.25f;

    [Tooltip("Minimum time after impact that the bullet is destroyed")]
    public float minDestroyTime;
    [Tooltip("Maximum time after impact that the bullet is destroyed")]
    public float maxDestroyTime;

    [Header("Impact Effect Prefabs")]
    public Transform[] metalImpactPrefabs;

    private void Start()
    {
        poolObject = GetComponent<UnityNightPool.PoolObject>();

    }

    private void OnCollisionEnter(Collision other)
    {

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, botMask);

        for (int i = 0; i < colliders.Length; i++)
        {

            Transform targetTransform = colliders[i].GetComponent<Transform>();

            if (!targetTransform)
                continue;

            AIHealth targetHealth = targetTransform.GetComponent<AIHealth>();
            BotManager botManager = targetTransform.GetComponent<BotManager>();
            if (!targetHealth || !botManager)
            {
                continue;
            }
            targetHealth.TakeDamage(Weapon.TakeDamage);
            botManager.isOnAttack = true;
            break;
        }

        StartCoroutine(DestroyTimer());
    }

    private IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(Random.Range(minDestroyTime, maxDestroyTime));
        poolObject.Return();
    }
}
