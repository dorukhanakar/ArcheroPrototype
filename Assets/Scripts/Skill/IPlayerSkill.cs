using UnityEngine;




public interface IPlayerSkill
{
    
    
    
    SkillType Type { get; }

    
    
    
    
    
    void Activate(SkillManager manager);

    
    
    
    
    
    void Deactivate(SkillManager manager);
}




public enum SkillType
{
    ArrowDuplication,
    BounceDamage,
    BurnDamage,
    AttackSpeedIncrease,
    RageMode
}
