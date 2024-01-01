using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CooltimeSlot : MonoBehaviour
{
    [SerializeField] public Image _icon;
    [SerializeField] TMP_Text _coolDownRemain;
    [SerializeField] Image _coolDownFill;
    [HideInInspector] public SkillData data;

    public void OnSkillCoolDownChanged(float elapsedCoolDownTime)
    {
        if (elapsedCoolDownTime <= 0 ||
            elapsedCoolDownTime >= data.coolDownTime)
        {
            _coolDownFill.fillAmount = 0;
            _coolDownRemain.text = string.Empty;
            return;
        }
        _coolDownFill.fillAmount = (1f - elapsedCoolDownTime / data.coolDownTime);
        _coolDownRemain.text = ((int)(data.coolDownTime - elapsedCoolDownTime)).ToString();
    }
}
