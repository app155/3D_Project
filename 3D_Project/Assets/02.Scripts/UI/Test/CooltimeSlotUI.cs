using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public CooltimeSlot slots;

    public void cooltimeCheckTest()
    {
        StartCoroutine(C_CoolDownSkillForTesting(slots.data.coolDownTime));
    }

    IEnumerator C_CoolDownSkillForTesting(float coolDownTime)
    {
        float timeMark = Time.time;
        while (true)
        {
            float elapsedTime = Time.time - timeMark;
            slots.OnSkillCoolDownChanged(elapsedTime);
            
            if (elapsedTime >= coolDownTime)
                break;

            yield return null;
        }
    }
}


