using UnityEngine;

[CreateAssetMenu(fileName = "SK_RageMode", menuName = "Skills/RageModeSkill")]
public class RageModeSkill : ScriptableObject, IPlayerSkill
{
    [Header("Rage Mode Extra Parameters")]
    public int rageExtraArrowCount = 2;
    public bool rageCanBounce = true;
    public float rageBurnDuration = 6f;
    public int rageBurnTickDamage = 2;
    public float rageAttackSpeedMultiplier = 4f;

    public SkillType Type => SkillType.RageMode;

    public void Activate(SkillManager manager)
    {
        
        manager.currentArrowCount += rageExtraArrowCount;

        
        if (rageCanBounce)
            manager.currentCanBounce = true;

        
        manager.currentBurnDuration = Mathf.Max(manager.currentBurnDuration, rageBurnDuration);

        
        manager.currentBurnTickDamage = Mathf.Max(manager.currentBurnTickDamage, rageBurnTickDamage);

        
        manager.currentAttackSpeedMultiplier *= rageAttackSpeedMultiplier;
    }

    public void Deactivate(SkillManager manager)
    {
        
        manager.currentArrowCount -= rageExtraArrowCount;
        manager.currentArrowCount = Mathf.Max(1, manager.currentArrowCount);

        
        bool anyBounce = false;
        foreach (var s in manager.ActiveSkillsOfType(SkillType.BounceDamage))
            anyBounce = true;
        if (!anyBounce)
            manager.currentCanBounce = false;

        
        manager.currentBurnDuration = 0f;

        
        manager.currentBurnTickDamage = 0;

        
        if (rageAttackSpeedMultiplier != 0f)
            manager.currentAttackSpeedMultiplier /= rageAttackSpeedMultiplier;
        else
            manager.currentAttackSpeedMultiplier = 1f;

        
        foreach (var s in manager.ActiveSkillsOfType(SkillType.BurnDamage))
        {
            s.Activate(manager);
        }
    }
}
