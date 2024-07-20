using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Класс повторного использования объектов. Объекты при использовании не уничтожаются, а добавляются сюда и деактивируются. 
/// Затем можно их отсюда взять и использовать повторно. Такой подход избавляет от больших затрат времени при высвобождении памяти во время System.GC.Collect().
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
    /// Деактивирует использованный USP и добавляет в список ожидания нового использования.
    /// </summary>
    /// <param name="usp"></param>
    public void AddAndInactivateUSP(USP usp)
    {
        usp.gameObject.SetActive(false);
        inactiveUSPs.Add(usp);
        //Debug.Log($"ADD inactive USP. inactiveUSPs count: {inactiveUSPs.Count}");
    }

    /// <summary>
    /// Если есть деактивированные USP для повторного использования, возвращает true.
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
