using System;
using UnityEngine;

public static class Formulas
{

    public static float CalcDamage(int lv)
    {
        if (lv == 1)
        {
            float damage = 40.0f;
            return damage;
        }
        else if (lv >= 2)
        {
            float damage = 40.0f * Mathf.Pow(1.08f, lv - 1);
            return damage;
        }
        else
        {
            return 0.0f;
        }
    }
    

	public static float CalcExp(float damage, int lv)
	{
        float baseExp = 0.0f; // 경험치의 기본값, 필요에 따라 조절 가능

        // 데미지와 레벨에 따른 경험치 계산
        float exp = baseExp + damage * 0.5f * lv;
        Debug.Log("WTF");
        return exp;
    }

    public static float CalculateRequiredExp(int currentLevel)
    {
        // 레벨업할 때마다 요구 경험치가 특정 비율로 증가하도록 계산
        float baseRequiredExp = 100.0f; 
        float expIncreaseRatio = 1.2f; 

        float requiredExp = baseRequiredExp * Mathf.Pow(expIncreaseRatio, currentLevel - 1);

        return requiredExp;
    }
}
