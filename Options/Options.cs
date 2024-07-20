using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public static float doubleClickTime = 0.2f; //время между двумя нажатиями для отсчета двойного клика
    public Toggle showFPS_Toggle;

    private void Awake()
    {
        showFPS_Toggle.onValueChanged.AddListener(ShowFPS_ValueChanged);
    }

    private void ShowFPS_ValueChanged(bool value)
    {
        Player_Data.Instance.showFPS = value;
    }
}
