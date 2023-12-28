using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooltimeSlotUI : MonoBehaviour
{
    public static CooltimeSlotUI instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(this.gameObject);
    }
    [SerializeField] public Image profile;
    [SerializeField] public CooltimeSlot slot1;
    [SerializeField] public CooltimeSlot slot2;

    public void cooltimeCheckTest(CooltimeSlot slot)
    {
        StartCoroutine(C_CoolDownSkillForTesting(slot, slot.data.coolDownTime));
    }

    IEnumerator C_CoolDownSkillForTesting(CooltimeSlot slot, float coolDownTime)
    {
        float timeMark = Time.time;
        while (true)
        {
            float elapsedTime = Time.time - timeMark;
            slot.OnSkillCoolDownChanged(elapsedTime);

            if (elapsedTime >= coolDownTime)
                break;

            yield return null;
        }
    }
}


