using TMPro;
using UnityEngine;

public class CooltimeSlot : MonoBehaviour
{
    [SerializeField] TMP_Text _coolDownRemain;
    [HideInInspector] public SkillData data;

    public void OnSkillCoolDownChanged(float elapsedCoolDownTime)
    {
        if (elapsedCoolDownTime <= 0 ||
            elapsedCoolDownTime >= data.coolDownTime)
        {
            _coolDownRemain.text = string.Empty;
            return;
        }
        _coolDownRemain.text = ((int)(data.coolDownTime - elapsedCoolDownTime)).ToString();
    }
}
