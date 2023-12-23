using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CharacterControllers = Project3D.Controller.CharacterControllers;

public class TestUI_Hp : MonoBehaviour
{
    public static TestUI_Hp testHp;
    public CharacterControllers chara;
    public TMP_Text text;

    private void Awake()
    {
        testHp = this;
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (chara != null)
        {
            text.text = chara.hpValue.ToString();
        }
    }
}
