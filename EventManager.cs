using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EVENT_TYPE
{
    USP_CREATED,
    USP_TAKEN,
    USP_ENLARGED,           //when USP increased for use by USPEnlarger

    /// <summary>
    /// При создании корабля. Вторым аргументом в делегате OnEvent идет сам компонент вновь созданного корабля!
    /// </summary>
    SHIP_CREATED,
    SHIP_DESTROYED,
    //SHIPS_COUNT_CHANGED,
    //TAKEN_DAMAGE,
    SHIPS_SIZE_CHANGED,      // пока в основном для Giant_module

    QUIT_LEVEL


}

public class EventManager : MonoBehaviour
{
    #region Singleton
    private static EventManager instance;
    public static EventManager Instance
    {
        get { return instance; }
        set { }
    }
    #endregion

    #region Fields
    public delegate void OnEvent(EVENT_TYPE event_type, Component Sender, object Param = null);
    private Dictionary<EVENT_TYPE, List<OnEvent>> Listeners = new Dictionary<EVENT_TYPE, List<OnEvent>>();
    #endregion

    #region Methods
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this);

        SceneManager.sceneLoaded += RemoveRedundancies; //or    SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) => { RemoveRedundancies(); };

        AddListener(EVENT_TYPE.SHIP_DESTROYED, DeleteShipListenerWhenDestroyed);
    }

    ///<summary>
    ///Метод добавления получателя в массив
    ///</summary>
    ///<param name="Event_Type">Событие, ожидаемое получателем</param>
    ///<param name="Listener">Объект, ожидающий события</param>
    public void AddListener(EVENT_TYPE Event_Type, OnEvent Listener)
    {
        List<OnEvent> ListOfListeners;//список получателей (т.е. метод у объектов-получателей, который запустится при отправке "сообщения" через метод PostNotification)
        if (Listeners.TryGetValue(Event_Type, out ListOfListeners))
        {
            ListOfListeners.Add(Listener);
            return;
        }//проверяем наличие типа события в словаре. Если есть, то добавляем нового получателя
        //Если нет, то добавляем новый тип события, и добавляем в него нового получателя, и все это добавляем в словарь
        ListOfListeners = new List<OnEvent>();
        ListOfListeners.Add(Listener);
        Listeners.Add(Event_Type, ListOfListeners);
    }

    ///<summary>
    ///Посылает событие получателям (по сути вызывает OnEvent, занесенный в словарь, у получателя)
    ///</summary>
    ///<param name="Event_Type">Событие для вызова</param>
    ///<param name="Sender">Вызываемый объект</param>
    ///<param name="Param">Необязательный аргумент</param>
    public void PostNotification(EVENT_TYPE Event_Type, Component Sender, object Param = null)
    {
        List<OnEvent> ListOfListeners;//создаем список получателей для этого события
        if (!Listeners.TryGetValue(Event_Type, out ListOfListeners))
            return; //ищем получателей события данного типа в словаре. Если получателей нет - завершаем работу метода.
                    //если получатели найдены, проверяем, что там не пустая ссылка, и отправляем им событие. А точнее вызываем у объекта-получателя метод OnEvent
                    //с данными аргументами, получеными еще от отправителя сообщения(т.е. объекта, сгенерировавшего событие, и отправившего данные о нем в EventMeneger
        for (int i = 0; i < ListOfListeners.Count; i++)
            if (!ListOfListeners[i].Target.Equals(null))
                ListOfListeners[i](Event_Type, Sender, Param);
    }
    

    /// <summary>
    /// Удаляет целый тип события и всех его подписчиков. Использовать с осторожностью!
    /// </summary>
    /// <param name="Event_Type"></param>
    public void RemoveEvent(EVENT_TYPE Event_Type)
    {
        if(Listeners.TryGetValue(Event_Type, out _))
            Listeners.Remove(Event_Type);
    }

    /// <summary>
    /// Удаляет конкретного подписчика (т.е. делегат) из списка подписчиков конкретного события.
    /// </summary>
    /// <param name="Event_Type"></param>
    /// <param name="listener"></param>
    public void RemoveListener(EVENT_TYPE Event_Type, OnEvent listener)
    {
        if (Listeners.TryGetValue(Event_Type, out List<OnEvent> listeners) && listeners.Contains(listener))
        {
            listeners.Remove(listener);
            //Debug.Log($"listener deleted: {listener.Method.Name}");
        }
    }

    /// <summary>
    /// удаляем избыточные записи из словаря. Аргументы scene и mode добавлены, чтобы метод можно было добавить в список обработчиков события SceneManager.sceneLoaded
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    public void RemoveRedundancies(Scene scene, LoadSceneMode mode)
    {
        Dictionary<EVENT_TYPE, List<OnEvent>> TmpListeners = new Dictionary<EVENT_TYPE, List<OnEvent>>();
        foreach(KeyValuePair<EVENT_TYPE,List<OnEvent>> Item in Listeners)
        {
            for(int i = Item.Value.Count - 1; i >= 0;i--)
            {
                //print($"Item.Value[{i}] = " + Item.Value[i].Target);
                if (Item.Value[i].Target.Equals(null))
                {
                    Item.Value.RemoveAt(i);
                }
            }

            if (Item.Value.Count > 0)
                TmpListeners.Add(Item.Key, Item.Value);
        }
        
        Listeners = TmpListeners;

    }

    /// <summary>
    /// при каждом уничтожении микрота проверяются все подписчики во всех списках на пустые ссылки и записи с пустыми ссылками удаляются. 
    ///пока не получается убрать метод именно уничтоженного микрота, который числится в обработчиках.
    /// </summary>
    /// <param name="event_type"></param>
    /// <param name="component"></param>
    /// <param name="obj"></param>
    void DeleteShipListenerWhenDestroyed(EVENT_TYPE event_type, Component component, object obj)
    {
        //print("Ship destroyed!");
        foreach (List<OnEvent> onEvents in Listeners.Values)
        {
            for (int i = 0; i < onEvents.Count; i++)
                if (onEvents[i].Target.Equals(null))
                    onEvents.RemoveAt(i);
        }
    }

    #endregion
}
