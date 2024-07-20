using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ����� ���������� ������������� ��������. ������� ��� ������������� �� ������������, � ����������� ���� � ��������������. 
/// ����� ����� �� ������ ����� � ������������ ��������. ����� ������ ��������� �� ������� ������ ������� ��� ������������� ������ �� ����� System.GC.Collect().
/// </summary>
public class ReusableObjects : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        inactiveUSPs.Clear();
    }

    #region USP

    private List<USP> inactiveUSPs = new List<USP>();

    /// <summary>
    /// ������������ �������������� USP � ��������� � ������ �������� ������ �������������.
    /// </summary>
    /// <param name="usp"></param>
    public void AddAndInactivateUSP(USP usp)
    {
        usp.gameObject.SetActive(false);
        inactiveUSPs.Add(usp);
        //Debug.Log($"ADD inactive USP. inactiveUSPs count: {inactiveUSPs.Count}");
    }

    /// <summary>
    /// ���� ���� ���������������� USP ��� ���������� �������������, ���������� true.
    /// </summary>
    /// <param name="usp"></param>
    /// <returns></returns>
    public bool TryToGetUSP(out USP usp)
    {
        int count = inactiveUSPs.Count;
        if (count > 0)
        {
            count--;
            usp = inactiveUSPs[count];
            usp.gameObject.SetActive(true);
            inactiveUSPs.RemoveAt(count);
            //Debug.Log($"GET inactive USP. inactiveUSPs count: {inactiveUSPs.Count}");
            return true;
        }

        usp = null;
        return false;
    }

    #endregion


}
