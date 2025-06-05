using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }
    public List<ScriptableObject> allSkillObjects = new List<ScriptableObject>();

    
    private readonly List<IPlayerSkill> activeSkills = new List<IPlayerSkill>();

    [Header("Values ​​Calculated at Runtime")]
    public int currentArrowCount = 1;
    public bool currentCanBounce = false;
    public float currentBurnDuration = 0f;
    public int currentBurnTickDamage = 0;
    public float currentAttackSpeedMultiplier = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ValidateAllSkillsList();
            ResetAllStats();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ValidateAllSkillsList()
    {
        for (int i = allSkillObjects.Count - 1; i >= 0; i--)
        {
            if (!(allSkillObjects[i] is IPlayerSkill))
            {
                Debug.LogWarning($"SkillManager: “{allSkillObjects[i].name}” IPlayerSkill implement etmiyor; listeden çıkarılıyor.");
                allSkillObjects.RemoveAt(i);
            }
        }
    }

    private void ResetAllStats()
    {
        currentArrowCount = 1;
        currentCanBounce = false;
        currentBurnDuration = 0f;
        currentBurnTickDamage = 0;
        currentAttackSpeedMultiplier = 1f;
    }

    public void ActivateSkill(ScriptableObject skillObj)
    {
        if (skillObj == null) return;
        if (!(skillObj is IPlayerSkill skill)) return;
        if (activeSkills.Contains(skill)) return;

        activeSkills.Add(skill);
        skill.Activate(this);
    }

    public void DeactivateSkill(ScriptableObject skillObj)
    {
        if (skillObj == null) return;
        if (!(skillObj is IPlayerSkill skill)) return;
        if (!activeSkills.Contains(skill)) return;

        skill.Deactivate(this);
        activeSkills.Remove(skill);

        
        if (skill.Type == SkillType.BurnDamage)
        {
            
            
            foreach (var s in ActiveSkillsOfType(SkillType.BurnDamage))
            {
                s.Activate(this);
            }
        }
    }

    public void ToggleSkill(ScriptableObject skillObj, bool enable)
    {
        if (enable) ActivateSkill(skillObj);
        else DeactivateSkill(skillObj);
    }

    
    
    
    public IEnumerable<IPlayerSkill> ActiveSkillsOfType(SkillType type)
    {
        foreach (var s in activeSkills)
        {
            if (s.Type == type) yield return s;
        }
    }
}
