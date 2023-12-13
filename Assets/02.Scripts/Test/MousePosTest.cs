using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosTest : MonoBehaviour
{
    Vector3 _mousePos;

    void Update()
    {
        Debug.Log(transform.position);
        Vector3 tempMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.back * Camera.main.transform.position.z);
        Vector3 normalizedMousePos = new Vector3(tempMousePos.x, 0.0f, tempMousePos.z);
        Debug.Log($"tempMousePos : {tempMousePos}");
        Debug.Log(normalizedMousePos);
    }
}
