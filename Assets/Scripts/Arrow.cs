using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Arrow : MonoBehaviour
{
    [Header("Arrow Settings")]
    public int damage = 1;

    [HideInInspector] public bool canBounce = false;
    [HideInInspector] public float burnDuration = 0f;

    
    private bool hasBounced = false;

    
    private Transform firstHitEnemy;

    private Rigidbody rb;
    private bool hasLaunched = false;
    [SerializeField]
    private GameObject fire;

    
    private static readonly Quaternion modelOffsetRotation = Quaternion.Euler(-90f, 0f, 0f);

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        Destroy(gameObject, 5f);
    }

    
    
    
    
    
    
    public void Launch(Vector3 velocity)
    {
        rb.velocity = velocity;
        Quaternion lookRotation = Quaternion.LookRotation(velocity.normalized);
        transform.rotation = lookRotation * modelOffsetRotation;
        hasLaunched = true;
        if (burnDuration > 0f && fire != null)
        {
            fire.SetActive(true);
        }
    }

    private void Update()
    {
        if (!hasLaunched) return;

        Vector3 vel = rb.velocity;
        if (vel.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(vel.normalized);
            transform.rotation = lookRotation * modelOffsetRotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            return;
        }

        
        Transform hitTransform = other.transform;
        EnemyController hitEnemy = other.GetComponent<EnemyController>();
        if (hitEnemy != null)
        {
            
            hitEnemy.TakeDamage(damage);

            
            if (burnDuration > 0f && SkillManager.Instance.currentBurnTickDamage > 0)
            {
                
                hitEnemy.StartCoroutine(
                    ApplyBurn(
                        hitEnemy,
                        burnDuration,
                        SkillManager.Instance.currentBurnTickDamage
                    )
                );
            }
        }

        
        if (canBounce && !hasBounced)
        {
            EnemyController bounceTargetEC = null;
            float minSqrDist = Mathf.Infinity;
            Vector3 arrowPos = transform.position;

            foreach (EnemyController ec in EnemyController.AllEnemies)
            {
                if (ec == null || ec.isDead)
                    continue;

                
                if (ec.transform == hitTransform || ec.transform == firstHitEnemy)
                    continue;

                
                float sqrDist = (ec.transform.position - arrowPos).sqrMagnitude;
                if (sqrDist < 0.04f) 
                    continue;

                if (sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    bounceTargetEC = ec;
                }
            }

            if (bounceTargetEC != null)
            {
                Vector3 startPos = transform.position;
                Vector3 targetPos = bounceTargetEC.transform.position + Vector3.up * 0.5f;

                float bounceSpeed = ArrowManager.Instance.defaultArrowSpeed * 0.5f;
                Vector3 direction = (targetPos - startPos).normalized;
                Vector3 newVelocity = direction * bounceSpeed;

                firstHitEnemy = hitTransform;
                hasBounced = true;
                canBounce = false;

                rb.velocity = newVelocity;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = lookRotation * modelOffsetRotation;

                
                return;
            }
        }

        
        Destroy(gameObject);
    }

    
    
    
    
    
    private IEnumerator ApplyBurn(EnemyController enemy, float duration, int dotDamage)
    {
        float timer = 0f;
        float tickInterval = 1f;
        
        if (enemy != null && enemy.fire != null)
            enemy.fire.SetActive(true);

        while (timer < duration)
        {
            if (enemy == null)
                yield break;

            enemy.TakeDamage(dotDamage);
            yield return new WaitForSeconds(tickInterval);
            timer += tickInterval;
        }

        
        if (enemy != null && enemy.fire != null)
            enemy.fire.SetActive(false);
    }
}
