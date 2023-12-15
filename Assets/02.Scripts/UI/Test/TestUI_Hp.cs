using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CharacterController = Project3D.Controller.CharacterController;

public class TestUI_Hp : MonoBehaviour
{
    public static TestUI_Hp testHp;
    public CharacterController chara;
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
            text.text = chara.HpValue.ToString();
        }
    }
}
