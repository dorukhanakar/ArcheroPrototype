using UnityEngine;

[CreateAssetMenu(fileName = "SK_ArrowDuplication", menuName = "Skills/ArrowDuplicationSkill")]
public class ArrowDuplicationSkill : ScriptableObject, IPlayerSkill
{
    [Header("Arrow Duplication Parameter")]
    public int extraArrowCount = 1;

    
    public SkillType Type => SkillType.ArrowDuplication;

    public void Activate(SkillManager manager)
    {
        
        manager.currentArrowCount += extraArrowCount;
    }

    public void Deactivate(SkillManager manager)
    {
        
        manager.currentArrowCount -= extraArrowCount;
        
        manager.currentArrowCount = Mathf.Max(1, manager.currentArrowCount);
    }
}
