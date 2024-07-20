using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ��������� ������ �� ��������� ������� ������� � ������ �����������, ��� ������� �� ������� �� ����� ��������� USP
/// </summary>
public class NotClickableGameObject : MonoBehaviour
{
    private void Start()
    {
        if(TryGetComponent(out Collider2D collider) && Global_Controller.Instance != null)
        {
            Global_Controller.Instance.notClickableGameObjects.Add(collider);
        }
        else
        {
            Debug.Log("� ������� ������� ��� ����������!");
        }
    }

    private void OnDestroy()
    {
        if (TryGetComponent(out Collider2D collider) && Global_Controller.Instance != null)
        {
            for(int i = 0; i < Global_Controller.Instance.notClickableGameObjects.Count; i++)
            {
                if(Global_Controller.Instance.notClickableGameObjects[i] == collider)
                    Global_Controller.Instance.notClickableGameObjects.Remove(collider);
            }
        }
    }


}
