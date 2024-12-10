using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject flashlight; // 手电筒的GameObject

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // 按F键开关手电筒
        {
            // 切换手电筒的激活状态
            flashlight.SetActive(!flashlight.activeSelf);
        }
    }
}