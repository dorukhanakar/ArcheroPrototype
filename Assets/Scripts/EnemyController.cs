using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    
    public static readonly List<EnemyController> AllEnemies = new List<EnemyController>();

    [Header("Movement & Attack Settings")]
    public float moveSpeed = 2f;
    public float attackRange = 1.5f;
    public int damagePerHit = 1;
    public float attackCooldown = 1.0f;

    [Header("Health Settings")]
    public int maxHealth = 3;

    [Header("Respawn Settings")]
    public Vector3 spawnAreaCenter = Vector3.zero;
    public Vector3 spawnAreaSize = new Vector3(20f, 0f, 20f);
    public float minSpawnDistanceFromPlayer = 10f;

    [Header("UI & Effects")]
    public HealthBar healthBar;
    public GameObject fire;

    
    private Transform playerTransform;
    private PlayerHealth playerHealth;
    private float lastAttackTime = 0f;

    [HideInInspector] public bool isDead = false;
    private int currentHealth;
    private Animator animator;
    private Rigidbody rb;

    #region Unity Callbacks

    private void OnEnable()
    {
        
        AllEnemies.Add(this);
    }

    private void OnDisable()
    {
        
        AllEnemies.Remove(this);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;

        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
            if (playerHealth == null)
                Debug.LogWarning("EnemyController: PlayerHealth component not found!");
        }
        else
        {
            Debug.LogWarning("EnemyController: No object with 'Player' tag found on stage!");
        }

        
        if (fire != null)
            fire.SetActive(false);

        UpdateHealthUI();
    }

    private void Update()
    {
        
        if (playerTransform == null || playerHealth == null)
            return;

        
        if (playerHealth.isDead)
        {
            if (animator != null)
                animator.SetTrigger("Idle");
            return;
        }

        
        if (currentHealth <= 0)
            return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);

        if (dist <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            
            if (animator != null)
                animator.SetTrigger("Attack");

            playerHealth.TakeDamage(damagePerHit);
            lastAttackTime = Time.time;
        }
    }

    private void FixedUpdate()
    {
        
        if (playerTransform == null || playerHealth == null || playerHealth.isDead || currentHealth <= 0)
            return;

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist > attackRange)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            Vector3 moveDelta = direction * moveSpeed * Time.fixedDeltaTime;
            Vector3 newPos = rb.position + new Vector3(moveDelta.x, 0f, moveDelta.z);
            rb.MovePosition(newPos);

            Vector3 lookDir = new Vector3(direction.x, 0f, direction.z);
            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRot, Time.fixedDeltaTime * 5f));
            }
        }
    }

    #endregion

    #region Health & Respawn

    
    
    
    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        //if (animator != null)
        //    animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
            currentHealth = 0;
        }

        UpdateHealthUI();
    }

    private void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Die");
            StartCoroutine(DestroyAndRespawnAfterAnimation());
        }
        else
        {
            RespawnNewEnemy();
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAndRespawnAfterAnimation()
    {
        
        yield return new WaitForSeconds(1f);

        RespawnNewEnemy();
        Destroy(gameObject);
    }

    private void RespawnNewEnemy()
    {
        if (playerTransform == null) return;

        Vector3 spawnPos;
        int safetyCounter = 0;

        
        do
        {
            float halfX = spawnAreaSize.x / 2f;
            float halfZ = spawnAreaSize.z / 2f;
            float randomX = Random.Range(spawnAreaCenter.x - halfX, spawnAreaCenter.x + halfX);
            float randomZ = Random.Range(spawnAreaCenter.z - halfZ, spawnAreaCenter.z + halfZ);
            spawnPos = new Vector3(randomX, spawnAreaCenter.y, randomZ);

            safetyCounter++;
            if (safetyCounter > 50) break;

        } while (Vector3.Distance(spawnPos, playerTransform.position) < minSpawnDistanceFromPlayer);

        
        GameObject prefab = Resources.Load<GameObject>("Enemy");
        if (prefab != null)
            Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    private void UpdateHealthUI()
    {
        float normalized = (float)currentHealth / maxHealth;
        if (healthBar != null)
            healthBar.SetHealthPercent(normalized);

        if (healthBar != null && healthBar.healthText != null)
            healthBar.healthText.text = currentHealth.ToString();
    }

    #endregion
}
