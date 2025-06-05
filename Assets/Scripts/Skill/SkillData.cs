using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/SkillData", order = 100)]
public class SkillData : ScriptableObject
{
    [Header("Genel Bilgiler")]
    public SkillType skillType;
    public string skillName;
    [TextArea] public string description;
    public Sprite icon; 

    [Header("Yetenek Parametreleri")]
    public int extraArrowCount = 0;           
    public bool canBounce = false;            
    public float burnDuration = 0f;           
    public float attackSpeedMultiplier = 1f;  

    [Header("Öfke Modu Ýçin Ekstra Parametreler")]
    public int rageExtraArrowCount = 0;          
    public int rageBounceExtraTargets = 0;       
    public float rageBurnDuration = 0f;          
    public float rageAttackSpeedMultiplier = 1f; 
}
