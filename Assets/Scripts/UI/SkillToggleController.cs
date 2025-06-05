using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillToggleController : MonoBehaviour
{
    [Header("Rage Mode Toggle")]
    public Toggle rageToggle;

    [Header("Other Skill Toggles")]
    public List<Toggle> otherSkillToggles = new List<Toggle>();

    private void Start()
    {
        if (rageToggle == null)
        {
            Debug.LogError("SkillToggleController: Rage Toggle atanmamýþ!");
            enabled = false;
            return;
        }
        if (otherSkillToggles.Count == 0)
        {
            Debug.LogWarning("SkillToggleController: Diðer skill toggle'larý listesi boþ.");
        }

        
        rageToggle.onValueChanged.AddListener(OnRageToggleChanged);

        
        foreach (var t in otherSkillToggles)
        {
            t.onValueChanged.AddListener(OnOtherSkillToggleChanged);
        }

        
        UpdateInteractables();
    }

    
    
    
    private void OnRageToggleChanged(bool isOn)
    {
        UpdateInteractables();
    }

    
    
    
    private void OnOtherSkillToggleChanged(bool isOn)
    {
        UpdateInteractables();
    }

    
    
    
    private void UpdateInteractables()
    {
        bool anyOtherOn = false;
        foreach (var t in otherSkillToggles)
        {
            if (t.isOn)
            {
                anyOtherOn = true;
                break;
            }
        }

        
        if (rageToggle.isOn)
        {
            foreach (var t in otherSkillToggles)
            {
                t.interactable = false;
            }
            
            rageToggle.interactable = true;
            return;
        }

        
        
        foreach (var t in otherSkillToggles)
        {
            t.interactable = true;
        }

        if (anyOtherOn)
        {
            rageToggle.interactable = false;
        }
        else
        {
            rageToggle.interactable = true;
        }
    }

    private void OnDestroy()
    {
        
        if (rageToggle != null)
            rageToggle.onValueChanged.RemoveListener(OnRageToggleChanged);

        foreach (var t in otherSkillToggles)
        {
            t.onValueChanged.RemoveListener(OnOtherSkillToggleChanged);
        }
    }
}
