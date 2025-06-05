using UnityEngine;

[CreateAssetMenu(fileName = "SK_AttackSpeedIncrease", menuName = "Skills/AttackSpeedSkill")]
public class AttackSpeedSkill : ScriptableObject, IPlayerSkill
{
    [Header("Attack Speed ​​Multiplier")]
    public float attackSpeedMultiplier = 2f;

    
    public SkillType Type => SkillType.AttackSpeedIncrease;

    public void Activate(SkillManager manager)
    {
        manager.currentAttackSpeedMultiplier *= attackSpeedMultiplier;
    }

    public void Deactivate(SkillManager manager)
    {
        
        if (attackSpeedMultiplier != 0f)
            manager.currentAttackSpeedMultiplier /= attackSpeedMultiplier;
        else
            manager.currentAttackSpeedMultiplier = 1f;
    }
}
