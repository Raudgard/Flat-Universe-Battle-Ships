using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjects;

/// <summary>
/// Класс сингтон с ссылками на все служебные классы.
/// </summary>
public class References : MonoBehaviour, ISerializationCallbackReceiver
{
    #region Singleton
    private static References instance;
    public static References Instance => instance;

    public void OnBeforeSerialize() { }
    public void OnAfterDeserialize()
    {
        if (instance == null)
            instance = this;
    }
    #endregion


    public Player_Data player_Data;
    public Global_Controller global_Controller;
    public Colors colors;
    public GameEngineAssistant gameEngineAssistant;
    public Prefabs prefabs;
    public Settings settings;
    public ReusableObjects reusableObjects;



}
