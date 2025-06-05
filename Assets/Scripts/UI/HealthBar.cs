using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HealthBar : MonoBehaviour
{
    private Camera mainCamera;
    public Image fillImage;
    public TextMeshProUGUI healthText;

    private void Awake()
    {
        
        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogWarning("Billboard: Camera.main bulunamadý!");
    }

    private void LateUpdate()
    {
        if (mainCamera == null) return;

        
        Vector3 direction = transform.position - mainCamera.transform.position;
        
        direction.y = 0f;
        if (direction.sqrMagnitude <= 0.001f) return;

        
        Quaternion lookRot = Quaternion.LookRotation(direction);
        transform.rotation = lookRot;
    }

    public void SetHealthPercent(float normalized)
    {
        fillImage.fillAmount = Mathf.Clamp01(normalized);
    }
}
