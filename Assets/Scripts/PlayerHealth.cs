using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 5;
    public HealthBar healthBar;

    
    private int currentHealth;

    
    public bool isDead { get; private set; } = false;

    
    private Animator animator;
    private PlayerController playerController;
    public GameObject gameOverPanel;
    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
            Debug.LogWarning("PlayerHealth: PlayerController not found!");
    }

    
    
    
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        
        if (animator != null)
        {
            
        }

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
            animator.SetTrigger("Death");
        }

        
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true; 
        }
        gameOverPanel.SetActive(true);
        StartCoroutine(ResetScene());
    }

    private void UpdateHealthUI()
    {
        float normalized = (float)currentHealth / maxHealth;
        if (healthBar != null)
            healthBar.SetHealthPercent(normalized);

        if (healthBar.healthText != null)
            healthBar.healthText.text = currentHealth.ToString();
    }

    IEnumerator ResetScene()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(0);
    }

}
