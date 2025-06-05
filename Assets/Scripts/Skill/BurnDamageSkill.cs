using UnityEngine;

[CreateAssetMenu(fileName = "SK_BurnDamage", menuName = "Skills/BurnDamageSkill")]
public class BurnDamageSkill : ScriptableObject, IPlayerSkill
{
    [Header("Yanma Hasarý Parametreleri")]
    public float burnDuration = 3f;
    public int burnTickDamage = 1;

    public SkillType Type => SkillType.BurnDamage;

    public void Activate(SkillManager manager)
    {
        
        manager.currentBurnDuration = Mathf.Max(manager.currentBurnDuration, burnDuration);
        
        manager.currentBurnTickDamage = Mathf.Max(manager.currentBurnTickDamage, burnTickDamage);
    }

    public void Deactivate(SkillManager manager)
    {
        
        manager.currentBurnDuration = 0f;
        manager.currentBurnTickDamage = 0;
        
    }
}
