using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    private Rigidbody rb;

    private PlayerControls controls;
    private Vector2 moveInput = Vector2.zero;
    private bool isMoving => moveInput.sqrMagnitude > 0.01f;

    [Header("Attack Settings")]
    public float fireRate = 1f;  
    private float fireCooldownTimer = 0f;
    public float rotationSpeed = 5f;
    public float detectionRange = 10f;

    
    private bool isRotatingAndShooting = false;

    
    private Animator animator;
    private PlayerHealth playerHealth;

    #region Unity Callbacks

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();

        controls = new PlayerControls();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled  += ctx => moveInput = Vector2.zero;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        
        if (playerHealth != null && playerHealth.isDead)
            return;

        
        if (animator != null)
            animator.SetBool("isRunning", isMoving);

        
        if (!isMoving && !isRotatingAndShooting)
        {
            float effectiveFireRate = fireRate * SkillManager.Instance.currentAttackSpeedMultiplier;
            fireCooldownTimer -= Time.deltaTime;
            if (fireCooldownTimer <= 0f)
            {
                fireCooldownTimer = 1f / Mathf.Max(effectiveFireRate, 0.0001f);
                AttackNearestEnemy();
            }
        }
        else
        {
            fireCooldownTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        
        if (playerHealth != null && playerHealth.isDead)
            return;

        HandleMovementPhysics();
    }

    #endregion

    #region Movement

    private void HandleMovementPhysics()
    {
        
        Vector3 camForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight   = Vector3.Scale(Camera.main.transform.right,   new Vector3(1, 0, 1)).normalized;
        Vector3 desiredMove = camForward * moveInput.y + camRight * moveInput.x;
        desiredMove = desiredMove.normalized * moveSpeed;

        rb.velocity = new Vector3(desiredMove.x, rb.velocity.y, desiredMove.z);

        
        if (isMoving)
        {
            Vector3 lookDir = new Vector3(desiredMove.x, 0, desiredMove.z);
            if (lookDir.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(lookDir),
                    Time.fixedDeltaTime * 10f
                );
            }
        }
    }

    #endregion

    #region Attack & Targeting

    private void AttackNearestEnemy()
    {
        
        if (playerHealth != null && playerHealth.isDead)
            return;

        
        EnemyController nearestEC = null;
        float minSqrDist = Mathf.Infinity;
        Vector3 myPos = transform.position;

        
        foreach (EnemyController ec in EnemyController.AllEnemies)
        {
            if (ec == null || ec.isDead)
                continue;

            float sqrDist = (ec.transform.position - myPos).sqrMagnitude;
            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                nearestEC = ec;
            }
        }

        if (nearestEC == null)
            return;

        
        if (minSqrDist > detectionRange * detectionRange)
            return;

        if (isRotatingAndShooting || isMoving)
            return;

        StartCoroutine(RotateAndShoot(nearestEC.gameObject));
    }

    private IEnumerator RotateAndShoot(GameObject target)
    {
        
        if (isMoving || (playerHealth != null && playerHealth.isDead))
        {
            isRotatingAndShooting = false;
            yield break;
        }

        isRotatingAndShooting = true;

        
        Vector3 toTarget = target.transform.position - transform.position;
        toTarget.y = 0f;
        toTarget.Normalize();
        Quaternion desiredRot = Quaternion.LookRotation(toTarget);

        while (Quaternion.Angle(transform.rotation, desiredRot) > 0.5f)
        {
            if (isMoving || (playerHealth != null && playerHealth.isDead))
            {
                isRotatingAndShooting = false;
                yield break;
            }

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                desiredRot,
                Time.deltaTime * rotationSpeed
            );
            yield return null;
        }
        transform.rotation = desiredRot;

        
        if (animator != null)
        {
            animator.SetBool("isShooting", true);
            yield return null; 
        }

        
        Vector3 spawnOffset = transform.forward * 1.5f + Vector3.up * 1.2f;
        Vector3 spawnPos = transform.position + spawnOffset;

        int arrowCount = SkillManager.Instance.currentArrowCount;
        bool canBounce = SkillManager.Instance.currentCanBounce;
        float burnDuration = SkillManager.Instance.currentBurnDuration;

        for (int i = 0; i < arrowCount; i++)
        {
            
            if (isMoving || (playerHealth != null && playerHealth.isDead))
            {
                isRotatingAndShooting = false;
                if (animator != null) animator.SetBool("isShooting", false);
                yield break;
            }

            if (target == null)
            {
                isRotatingAndShooting = false;
                if (animator != null) animator.SetBool("isShooting", false);
                yield break;
            }

            Vector3 flatTargetPos = target.transform.position;
            flatTargetPos.y = spawnPos.y;
            Vector3 direction = (flatTargetPos - spawnPos).normalized;

            if (arrowCount > 1)
            {
                float spreadAngle = 5f;
                float angleOffset = (i - (arrowCount - 1) / 2f) * spreadAngle;
                direction = Quaternion.Euler(0, angleOffset, 0) * direction;
                direction.Normalize();
            }

            ArrowManager.Instance.FireArrow(
                spawnPos,
                spawnPos + direction * 30f,
                canBounce,
                burnDuration
            );
        }

        
        float effectiveFireRate = fireRate * SkillManager.Instance.currentAttackSpeedMultiplier;
        float waitTime = 1f / Mathf.Max(effectiveFireRate, 0.0001f);
        float timer = 0f;
        while (timer < waitTime)
        {
            if (isMoving || (playerHealth != null && playerHealth.isDead))
            {
                isRotatingAndShooting = false;
                if (animator != null) animator.SetBool("isShooting", false);
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        
        if (animator != null)
            animator.SetBool("isShooting", false);

        isRotatingAndShooting = false;
    }

    #endregion
}
