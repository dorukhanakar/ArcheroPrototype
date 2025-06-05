using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    public static ArrowManager Instance { get; private set; }
    public GameObject arrowPrefab;
    public float defaultArrowSpeed = 15f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public void FireArrow(
        Vector3 startPos,
        Vector3 targetPos,
        bool canBounce,
        float burnDuration,
        float speed = -1f
    )
    {
        if (arrowPrefab == null) return;

        GameObject arrowObj = Instantiate(arrowPrefab, startPos, Quaternion.identity);
        Arrow arrow = arrowObj.GetComponent<Arrow>();
        if (arrow == null) return;

        
        arrow.canBounce = canBounce;
        arrow.burnDuration = burnDuration;

        
        float actualSpeed = (speed > 0f) ? speed : defaultArrowSpeed;

        
        Vector3 direction = (targetPos - startPos).normalized;
        Vector3 launchVel = direction * actualSpeed;
        arrow.Launch(launchVel);
    }
}
