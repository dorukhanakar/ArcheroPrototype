using UnityEngine;
using UnityEngine.UI;

public class SkillToggle : MonoBehaviour
{
    public ScriptableObject skillObject;
    [SerializeField]
    private Toggle toggle;
    
    public void OnToggleChanged()
    {

        if (skillObject == null) return;
        SkillManager.Instance.ToggleSkill(skillObject, toggle.isOn);
        //Debug.Log(skillObject.name + " used");
    }
}
