using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MODULES;
using System;
using System.Linq;
using Tools;

//[ExecuteAlways]
public class Global_Controller : MonoBehaviour, ISerializationCallbackReceiver
{
    #region Singleton
    private static Global_Controller instance;
    public static Global_Controller Instance => instance;

    public void OnBeforeSerialize() { }
    public void OnAfterDeserialize()
    {
        if (instance == null)
            instance = this;
    }
    #endregion

    private Colors colors;
    public float timeToVanishingDepartingDigits;
    public float timeToFlyAwayDepartingDigits;
    public float speedOfDepartingDigits;

    public List<Ship> allShips = new List<Ship>();

    /// <summary>
    /// Список всех кораблей у всех игроков. int - номер игрока. List<Ship> - список кораблей игрока.
    /// </summary>
    public Dictionary<int, List<Ship>> ships = new Dictionary<int, List<Ship>>();   // int - player, List - ships of player, 1 - User player, 0 - neutral, other - AI.
    public List<Collider2D> notClickableGameObjects = new List<Collider2D>();

    [Tooltip("Текущий максимум юнитов для игроков.")]
    public int[] shipsMaxCapacity;
    

    private Coroutine searchingEnemysCoroutine;
    private GameEngineAssistant gameEngineAssistant;

    [Tooltip("Коэффициент усиления ударной волны.")]
    [SerializeField] private float forceMultiplier;
    private static LayerMask shipsLayer;


    public bool isMenuOpened = false;
    private bool isGamePaused = false;
    public bool isUIElementPressed = false;

    /// <summary>
    /// Словарь, содержащий пары: номер игрока и полоска мощи, которая отображает его состояние.
    /// </summary>
    public List<TwoValuePair<int, PowerBar>> powerBars = new List<TwoValuePair<int, PowerBar>>();

    [NaughtyAttributes.OnValueChanged("TimeScaleValueChange")]
    [Range(0, 10)]
    public float timeScale;
    private void TimeScaleValueChange() => Time.timeScale = timeScale;

    public bool IsGamePaused
    {
        get
        {
            return isGamePaused;
        }

        set
        {
            if (value) Time.timeScale = 0;
            else Time.timeScale = 1;
            isGamePaused = value;
        }
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        gameEngineAssistant = References.Instance.gameEngineAssistant;
        ships.Add(1, new List<Ship>());
        ships.Add(2, new List<Ship>());//возможно, можно сделать по-другому.


        colors = GetComponent<Colors>();
        shipsLayer = LayerMask.GetMask("ShipLayer");
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene loadedScene, LoadSceneMode arg1)
    {
        if (loadedScene.name == "ScirmishScene" || loadedScene.name == "Maze")
            searchingEnemysCoroutine = StartCoroutine(SearchingForEnemy());

    }

    private void Start()
    {
        //EventManager.Instance.AddListener(EVENT_TYPE.USP_TAKEN, DeleteUSP);
        EventManager.Instance.AddListener(EVENT_TYPE.SHIP_DESTROYED, DeleteShip);

    }

    //private void FixedUpdate()
    //{
    //    for (int i = 0; i < allShips.Count; i++)
    //        if (allShips[i] != null)
    //            allShips[i].FixUpdateMe();
    //}


    public void ShockwaveFromCreatedUSP(USP createdUSP)
    {
        if (createdUSP.team == 0)
            return;
        
        Vector2 shipPosition;
        Vector2 positionOfCreatedUSP = createdUSP.transform.position;
        float distanceSqr;
        List<Ship> _ships = ships[createdUSP.team];

        for (int i = 0; i < _ships.Count; i++)
        {
            if (_ships[i].InThirdDimention)
                continue;
            
            shipPosition = _ships[i].transform.position;
            distanceSqr = Vector2.SqrMagnitude(shipPosition - positionOfCreatedUSP);

            if(_ships[i].State == Ship.States.TO_USP)
                CompareDistancesToUSPs(_ships[i], shipPosition, distanceSqr, positionOfCreatedUSP, createdUSP);

            if (distanceSqr > _ships[i].SensivityRadiusSqr)
                continue;


            Vector2 force;

            // сила, применяемая к микроту прямо пропорциональна "силе нажатия"
            //(зависит от навыка в "лабе") и обратна пропорциональна расстоянию между кораблем и вновь появившейся едой
            force = (shipPosition - positionOfCreatedUSP) * _ships[i].SensivityRadiusSqr * (1 / distanceSqr);

            //print("force = " + force + "   Velosity = " + _body.velocity + "   velocity.magnitude = " + _body.velocity.magnitude);
            force *= forceMultiplier;
            //print("force = " + force.magnitude);

            _ships[i].MovingFromForce(force);
        }
    }


    //void DeleteUSP(EVENT_TYPE event_type, Component component, object obj)
    //{
    //    USP _USP = (USP)obj;
    //    if (BattleSceneController.Instance.freeUSPs.Contains(_USP))
    //        BattleSceneController.Instance.freeUSPs.Remove(_USP);
    //}


    public Ship CreateShip(int team, int playerNumber, Ship.Forms form, Vector2 startingPosition, params SaveUtility.ModuleSaved[] modules)
    {
        var ship = Instantiate(Prefabs.Instance.ship);

        ship.shipName = Player_Data.Instance.NamesOfShips[(int)form];
        ship.name = ship.GetInstanceID() + $". Ship ({ship.shipName})" + ". Team " + team + ". Player " + playerNumber;
        ship.shipVisualController.backLightRenderer.color = 
        ship.shipVisualController.originalShipBacklightColor = colors.player_background_colors[playerNumber];
        ship.Form = form;
        ship.team = team;
        ship.playerNumber = playerNumber;
        ship.startingPosition = ship.transform.position = new Vector3(startingPosition.x, startingPosition.y, -1);

        for (int i = 0; i < modules.Length; i++)
        {
            if (modules[i] == null) continue;

            //print(i + ". " + modules[i].typeOfModule);
            var module = (Module)ship.gameObject.AddComponent(modules[i].typeOfModule);
            module.LevelOfModule = modules[i].level;
            module.energy = modules[i].energy;
        }
        

        return ship;
    }


    public Ship[] CreateShips(SaveUtility.SaveData data)
    {
        var ships = new Ship[data.countShips];
        for (int i = 0; i < data.countShips; i++)
        {
            int team = data.ships[i].team;
            int playerNumber = data.ships[i].playerNumber;
            var form = data.ships[i].form;
            var modules = data.ships[i].modules;
            var startingPosition = data.ships[i].startingPosition;
            ships[i] = CreateShip(team, playerNumber, form, new Vector2(startingPosition.X, startingPosition.Y), modules.ToArray());
        }
        return ships;
    }


    public void AddShip(Ship ship)
    {
        //ship.transform.parent = battle_Scene_Controller?.ShipsPlayer_number[ship.team];
        if(BattleSceneController.Instance != null)
            ship.transform.parent = BattleSceneController.Instance.ShipsPlayer_number_Transforms[ship.team];

        allShips.Add(ship);
        if (!ships.ContainsKey(ship.team)) ships.Add(ship.team, new List<Ship>());
        ships[ship.team].Add(ship);
        //if (ship.team % 2 == 1)
        //    firstTeamShips.Add(ship);
        //else if (ship.team % 2 == 0)
        //    secondTeamShips.Add(ship);
        
        //EventManager.Instance.PostNotification(EVENT_TYPE.SHIPS_COUNT_CHANGED, this);
        EventManager.Instance.PostNotification(EVENT_TYPE.SHIP_CREATED, ship);

    }

    /// <summary>
    /// Выполняется при смерти какого-либо корабля.
    /// </summary>
    /// <param name="event_type"></param>
    /// <param name="component"></param>
    /// <param name="obj"></param>
    public void DeleteShip(EVENT_TYPE event_type, Component component, object obj)
    {
        var shipToDelete = (Ship)component;
        for(int i = 0; i < allShips.Count; i++)
        {
            if (allShips[i].enemyCurrent == shipToDelete) allShips[i].enemyCurrent = null;
            if (allShips[i].discoveredEnemies.Contains(shipToDelete)) allShips[i].discoveredEnemies.Remove(shipToDelete);
            if (allShips[i] == shipToDelete) allShips.RemoveAt(i);
        }


        foreach(var kvp in ships)
        {
            if (ships[kvp.Key].Contains(shipToDelete)) ships[kvp.Key].Remove(shipToDelete);
        }


        if (ships[1].Count == 0)
        {
            PlayerLose();
            return;
        }

    }


    /// <summary>
    /// Возвращает true, если игрок еще не достиг максимум кораблей.
    /// </summary>
    /// <param name="playerNumber"></param>
    /// <returns></returns>
    public bool IsEnoughCapacityForShip(int playerNumber)
    {
        return ships[playerNumber].Count() < shipsMaxCapacity[playerNumber];
    }







    private void CompareDistancesToUSPs(Ship ship, Vector2 shipPosition, float distanceSqrToNewUSP, Vector2 positoinOfNewUSP, USP USP)
    {
        if (ship.USPCurrent != null)
        {
            float distanceToCurrentUSP_Sqr = (shipPosition - (Vector2)ship.USPCurrent.transform.position).sqrMagnitude;
            float distanceToNewUSP_Sqr = distanceSqrToNewUSP;
            if (distanceToNewUSP_Sqr < distanceToCurrentUSP_Sqr)
            {
                Vector2 vectorToUSP = positoinOfNewUSP - shipPosition;
                vectorToUSP = vectorToUSP.normalized;
                ship.moveDirection = vectorToUSP;
                ship.USPCurrent = USP;
                ship.rotation_Controller.Rotate(vectorToUSP);
                //print("new USP closest! new vector = " + forUSPComponent.directionToUSP);
            }
        }
    }


    private IEnumerator SearchingForEnemy()
    {
        List<Ship> _shipsWhoSearch;
        List<Ship> __otherShips;

        while (true)
        {
            for (int teamWhoSearch = 1; teamWhoSearch < ships.Count + 1; teamWhoSearch++)
            {
                _shipsWhoSearch = ships[teamWhoSearch];
                for (int otherTeam = 1; otherTeam < ships.Count + 1; otherTeam++)
                {
                    if (teamWhoSearch == otherTeam) continue;
                    __otherShips = ships[otherTeam];

                    for (int i = 0; i < _shipsWhoSearch.Count; i++)
                    {
                        if ((_shipsWhoSearch[i].State != Ship.States.IDLE 
                            && _shipsWhoSearch[i].State != Ship.States.TO_USP 
                            && _shipsWhoSearch[i].State != Ship.States.FIGHT
                            && _shipsWhoSearch[i].State != Ship.States.SEARCHING_FOR_ENEMY)
                            || _shipsWhoSearch[i] == null)
                        {
                            //print("проверка пропущена. Ship = " + firstTeamShips[i].shipName);
                            continue;
                        }


                        uint jobNumber = gameEngineAssistant.JobsCounter;
                        yield return gameEngineAssistant.StartCoroutine(gameEngineAssistant.CheckShipsInVisionRadius(_shipsWhoSearch[i], __otherShips, jobNumber));
                        var resultsOfJob = gameEngineAssistant.CheckShipsInVisionRadius_Results[jobNumber];
                        //print($"results of job: {resultsOfJob.Count}");
                        //for (int j = 0; j < resultsOfJob.Count; j++)
                        {
                            if (resultsOfJob.Count > 0 && i < _shipsWhoSearch.Count)
                            {
                                _shipsWhoSearch[i].Enemys_detected(resultsOfJob, new List<Ship>());
                            }

                        }
                        gameEngineAssistant.CheckShipsInVisionRadius_Results.Remove(jobNumber);
                    }
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }


    /// <summary>
    /// Проверка на то, что лежит между кораблями. Если там есть что-то кроме кораблей, значит другой корабль "не видно"
    /// </summary>
    /// <param name="ship"></param>
    /// <param name="otherShip"></param>
    /// <returns></returns>
    public static bool MustSeeShip(Ship ship, Ship otherShip)
    {
        Vector2 vectorToEnemy = otherShip.transform.position - ship.transform.position;
        RaycastHit2D[] raycastHits = Physics2D.RaycastAll(ship.transform.position, vectorToEnemy, vectorToEnemy.magnitude, shipsLayer);
        bool res = true;

        for (int i = 0; i < raycastHits.Length; i++)
        {
            if (!raycastHits[i].transform.TryGetComponent(out Ship _))
                res = false;
        }

        return res;
    }

    public static bool IsDirectSightShip(Ship ship, Ship otherShip)
    {
        Vector2 vectorToEnemy = otherShip.transform.position - ship.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(ship.transform.position, vectorToEnemy, Mathf.Infinity, shipsLayer, -3, 0);

        if (hit.transform == null)
        {
            print(ship.gameObject.name + ": " + "hit.transform = null");
            //_enemyShipHit = null;
            return false;
        }//иногда случаются ошибки (непонятно почему) и выдает nullReferenceException, поэтому пришлось ввести проверку на null

        if (hit.transform.TryGetComponent(out Ship shipUnderHit) && shipUnderHit == otherShip)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Возвращает true, если второй корабль находится у первого в радиусе обзора и между ними нет препятствий кроме других кораблей.
    /// </summary>
    /// <param name="ship"> Корабль, который "смотрит"</param>
    /// <param name="otherShip">Корабль, на который "смотрят"</param>
    /// <returns></returns>
    public static bool DoISeeThisShip(Ship ship, Ship otherShip)
    {
        float distanceSqr = ((Vector2)ship.transform.position - (Vector2)otherShip.transform.position).sqrMagnitude;

        var dist = ship.VisionRadius + otherShip.radiusSize;
        if (distanceSqr < dist * dist && MustSeeShip(ship, otherShip))
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// Создает отлетающие цифры урона/лечения/яда
    /// </summary>
    /// <param name="value"></param>
    /// <param name="position">Где создаются</param>
    /// <param name="direction">куда движутся</param>
    /// <param name="color">цвет (зависит от типа воздействия)</param>
    /// <param name="duration">длительность</param>
    /// <returns></returns>
    public IEnumerator VisualizationOfDamage(int value, int maxHPofShip, Vector2 position, Vector2 direction, Color color, float duration = -0.1f)
    {
        //print("damage = " + value + "   position = " + position + "   direction = "+direction + "   color = " + color);
        DepartingDigits digits = Instantiate(Prefabs.Instance.departingDigits);
        //digits.transform.parent = battle_Scene_Controller.DepartingDigits;
        if(BattleSceneController.Instance != null)
            digits.digitsTransform.parent = BattleSceneController.Instance.DepartingDigits_Transform;

        //DontDestroyOnLoad(digits);//сделал для красоты, и чтобы при смене сцены не возникало ошибок от уничтоженного объекта
        digits.textMesh.text = value.ToString();
        //Transform digitsTransform = digits.GetComponent<Transform>();
        digits.digitsTransform.position = new Vector3(position.x, position.y, -2);
        float size = digits.digitsTransform.localScale.x + (float)value / maxHPofShip;
        digits.digitsTransform.localScale = new Vector3(size, size, size);

        Color transparentColor = new Color(color.r, color.g, color.b, 0);
        digits.textMesh.color = transparentColor;

        //время на то,чтобы проявиться
        float t = 0.5f;
        while (t > 0)
        {
            digits.textMesh.color = Color.Lerp(color, transparentColor, t);
            digits.digitsTransform.Translate(speedOfDepartingDigits * Time.deltaTime * direction);
            yield return null;
            t -= Time.deltaTime;
        }

        //время на то, чтобы отлететь (в полном цвете)
        t = timeToFlyAwayDepartingDigits;
        while(t > 0)
        {
            digits.digitsTransform.Translate(speedOfDepartingDigits * Time.deltaTime * direction);
            yield return null;
            t -= Time.deltaTime;
        }

        //время, чтобы замедлить отлет и исчезнуть
        t = timeToVanishingDepartingDigits;
        float a;
        while(t >= 0)
        {
            a = t / timeToVanishingDepartingDigits;
            digits.textMesh.color = Color.Lerp(transparentColor, color, a);
            digits.digitsTransform.Translate(speedOfDepartingDigits * Time.deltaTime * Vector2.Lerp(Vector2.zero, direction, a));

            yield return null;
            t -= Time.deltaTime;
        }

        Destroy(digits.gameObject);
    }


    public IEnumerator TextVisualization(string text, Vector2 position, Vector2 direction, Color color, float duration = -0.1f)
    {
        //print("damage = " + value + "   position = " + position + "   direction = "+direction + "   color = " + color);
        DepartingDigits digits = Instantiate(Prefabs.Instance.departingDigits);
        //digits.transform.parent = battle_Scene_Controller.DepartingDigits;
        if (BattleSceneController.Instance != null)
            digits.digitsTransform.parent = BattleSceneController.Instance.DepartingDigits_Transform;

        //DontDestroyOnLoad(digits);//сделал для красоты, и чтобы при смене сцены не возникало ошибок от уничтоженного объекта
        digits.textMesh.text = text;
        Transform digitsTransform = digits.digitsTransform;
        digitsTransform.position = new Vector3(position.x, position.y, -2);
        //float size = digitsTransform.localScale.x + (float)value / maxHPofShip;
        //digitsTransform.localScale = new Vector3(size, size, size);

        Color transparentColor = new Color(color.r, color.g, color.b, 0);
        digits.textMesh.color = transparentColor;

        //время на то,чтобы проявиться
        float t = 0.5f;
        while (t > 0)
        {
            digits.textMesh.color = Color.Lerp(color, transparentColor, t);
            digitsTransform.Translate(speedOfDepartingDigits * Time.deltaTime * direction);
            yield return null;
            t -= Time.deltaTime;
        }

        //время на то, чтобы отлететь (в полном цвете)
        t = timeToFlyAwayDepartingDigits;
        while (t > 0)
        {
            digitsTransform.Translate(speedOfDepartingDigits * Time.deltaTime * direction);
            yield return null;
            t -= Time.deltaTime;
        }

        //время, чтобы замедлить отлет и исчезнуть
        t = timeToVanishingDepartingDigits;
        float a;
        while (t >= 0)
        {
            a = t / timeToVanishingDepartingDigits;
            digits.textMesh.color = Color.Lerp(transparentColor, color, a);
            digitsTransform.Translate(speedOfDepartingDigits * Time.deltaTime * Vector2.Lerp(Vector2.zero, direction, a));

            yield return null;
            t -= Time.deltaTime;
        }

        Destroy(digits.gameObject);
    }


    /// <summary>
    /// Устанавливает для всех игроков (кроме нейтрального) максимальное количество кораблей из настроек.
    /// </summary>
    public void SetShipsMaxCapacityFromSettings()
    {
        for (int i = 1; i < shipsMaxCapacity.Length; i++)
        {
            shipsMaxCapacity[i] = References.Instance.settings.maxShipsCapacity;
        }
    }


    private void PlayerWin()
    {
        //scene_UI_Controller.ShowWinOrLosePanel(1);
    }
    private void PlayerLose()
    {
        //scene_UI_Controller.ShowWinOrLosePanel(2);
    }


   

    /// <summary>
    /// Останавливает все корутины Global_Data, очищает списки еды и микротов
    /// </summary>
    public void Clear()
    {
        StopCoroutine(searchingEnemysCoroutine);
        allShips.Clear();

        foreach (var shipsList in ships.Values)
            shipsList.Clear();

        foreach (var team in ships)
            ships[team.Key].Clear();

        //ships.Clear();
        //firstTeamShips.Clear();
        //secondTeamShips.Clear();
        BattleSceneController.Instance.freeUSPs.Clear();

    }



    
}
