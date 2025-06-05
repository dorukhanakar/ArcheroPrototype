using UnityEngine;

[CreateAssetMenu(fileName = "SK_BounceDamage", menuName = "Skills/BounceDamageSkill")]
public class BounceDamageSkill : ScriptableObject, IPlayerSkill
{
    [Header("Bounce Damage Parameter")]
    public bool canBounce = true;

    
    public SkillType Type => SkillType.BounceDamage;

    public void Activate(SkillManager manager)
    {
        manager.currentCanBounce = true;
    }

    public void Deactivate(SkillManager manager)
    {
        manager.currentCanBounce = false;
    }
}
