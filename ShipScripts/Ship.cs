using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MODULES;
using System.Linq;
using NaughtyAttributes;
using Tools;

[RequireComponent(typeof(NotClickableGameObject))]
public class Ship : MonoBehaviour, INeedFixUpdate
{
    public enum States
    {
        /// <summary>
        /// Бездействует в бою (нет USP, не видно врагов)
        /// </summary>
        IDLE,
        /// <summary>
        /// Двигается к USP
        /// </summary>
        TO_USP,
        /// <summary>
        /// Двигается какое-то время в ту сторону, откуда прилетел ударивший его снаряд.
        /// </summary>
        SEARCHING_FOR_ENEMY,
        /// <summary>
        /// Сражается.
        /// </summary>
        FIGHT,
        /// <summary>
        /// Процесс создания своей копии из собранных USP
        /// </summary>
        REPRODUCTION,
        /// <summary>
        /// Оглушен, заморожен
        /// </summary>
        STUNNED,
        /// <summary>
        /// Отбит ударной волной.
        /// </summary>
        FLEE,
        /// <summary>
        /// Увеличивает себя в размерах за счет поглощения USP (у кораблей с модулем GIANT)
        /// </summary>
        DIGESTION,
        /// <summary>
        /// Лечит союзные корабли (У кораблей с модулем MEDICUS)
        /// </summary>
        HEALING

    }

    public enum Forms
    {
        Alpha,
        Beta,
        Gamma,
        Delta,
        Epsilon,
        Zeta,
        Eta,
        Iota,
        Kappa,
        Lambda
    }


    [Foldout("States")] [SerializeField] private States state = States.IDLE;
    [Foldout("States")] [Tooltip("коэффициент изменения localScale относительно оригинала.")] public float changedSize;
    [Foldout("States")] public int playerNumber; // 0 - neutral, 1 - player, 2 etc. - AI
    [Foldout("States")] public int team;
    [Foldout("States")] public string shipName;
    [Foldout("States")] public int healthMax;
    [Foldout("States")] [SerializeField] private int healthCurrent;
    [Foldout("States")] public int attack_damage = 0;
    [Foldout("States")] [Tooltip("Время перезарядки основного оружия.")] public float reload_time = 2.0f;
    [Foldout("States")] [Tooltip("Расстояние атаки (БЕЗ учета размера корабля), то есть от края коллайдера.")] public float attack_range;
    [Foldout("States")] public float armor = 0;
    [Foldout("States")] public float move_speed = 20.0f;
    [Foldout("States")] [SerializeField] private float visionRadius;
    [Foldout("States")] public float chanceToStun = 0; // %
    [Foldout("States")] public float stunnedTime = 0.0f;
    [Foldout("States")] public float timePassedFromStartReproduction = 0.0f;
    [Foldout("States")] public Vector2 moveDirection = new Vector2(0, 0);
    [Tooltip("текущий вражеский микрот")] 
    [Foldout("States")] public Ship enemyCurrent = null;
    [Tooltip("последний нанесший урон микрот (может быть и союзным). Передается в том числе 3-м аргументом в сообщении о смерти")]
    [Foldout("States")] public Ship shipLastToDoDamage = null;
    [Foldout("States")] [SerializeField] private int _USPTakens = 0;
    [Foldout("States")] public bool movingFromForce;
    [Foldout("States")] [SerializeField] private bool isOriginal = true;
    public bool IsOriginal { get { return isOriginal; } set { isOriginal = value; } }

    [Foldout("States")] [SerializeField] private bool isInvisible = false;
    public bool IsInvisible { get { return isInvisible; } set { isInvisible = value; } }

    [Foldout("States")] [SerializeField] private Module[] modules;
    public Module[] Modules { get { return modules; } set { modules = value; } }

    [HideInInspector]
    public USP USPCurrent = null;
    [HideInInspector]
    public int USPNeedToReproduction = 15;
    [HideInInspector]
    public float secondsNeedToReproduction = 16.0f;
    [HideInInspector]
    public bool isReadyToHit = true;
    [HideInInspector]
    public bool isDebuffApplied = false;


    [Foldout("Parameters")] public float deathExplosionScaleSize;
    [Foldout("Parameters")] public Vector3 startingPosition;
    [Foldout("Parameters")] public float radiusSize;
    [Foldout("Parameters")] public float SensivityRadiusSqr;
    [Tooltip("Коэф замедления от ударной волны")]
    [Foldout("Parameters")] public float deseleration_coeff;
    [Foldout("Parameters")] public float deseleration_treshold;


    [Foldout("References")] public Ship motherShip = null;
    [Foldout("References")] public GameObject shipBars;
    [Foldout("References")] public Rotation_Controller rotation_Controller;
    [Foldout("References")] public Avoiding_Obstacle avoiding_Obstacle;
    [Foldout("References")] public CircleCollider2D mainCollider;
    [Foldout("References")] public ShipTakeHit takeHitComponent;
    [Foldout("References")] public ShipVisualController shipVisualController;
    [Foldout("References")] public SearchingForUSP searchingForUSP;
    [Foldout("References")] [SerializeField] private Rigidbody2D body;
    [Foldout("References")] [SerializeField] private MovingWhenIdle movingWhenIdleComponent;


    

    /// <summary>
    /// При изменении State корабля.
    /// </summary>
    public event Action StateChanged;

    /// <summary>
    /// При назначении moveDirection = Vector2.zero
    /// </summary>
    public event Action MoveDirectionZeroEvent;
    public void InvokeMoveDirectionZeroEvent() => MoveDirectionZeroEvent?.Invoke();

    /// <summary>
    /// При создании снаряда вражеским кораблем, который полетит в данный корабль.
    /// </summary>
    public event Action ShotDetected;
    public Action USP_taken;
    public Action<Vector2> search_for_enemy;

    /// <summary>
    /// Вызывается непосредственно перед началом репродуцирования.
    /// </summary>
    public Action beforeStartReproduction;
    /// <summary>
    /// Вызывается сразу после окончания ожидания времени репродуцирования до создания нового корабля.
    /// </summary>
    public Action afterEndTimeReproduction;


    private Forms form;
    
    /// <summary>
    /// Находится в третьем измерении. Изменяется модулем Уворота.
    /// </summary>
    public bool InThirdDimention { get; set; } = false;

    /// <summary>
    /// Мощность корабля.
    /// </summary>
    public int Power => (int)Math.Cbrt(Modules.Sum(m => m.Power));

    private Coroutine stunnedCoroutine = null;
    private Coroutine reproductionCoroutine = null;
    private Coroutine checkForCapacityCoroutine = null;
    


    public States State 
    { 
        get { return state;}
        set
        {
            var oldState = state;
            state = value;
            if(StateChanged != null && oldState != value)
                StateChanged.Invoke();
        }
    }

    /// <summary>
    /// Возвращает true, если состояние корабля TO_USP or IDLE
    /// </summary>
    /// <returns></returns>
    public bool IsStateFree => State == States.TO_USP || State == States.IDLE /*|| State == States.SEARCHING_FOR_ENEMY*/;

    public Forms Form
    {
        get { return form; }
        set
        {
            form = value;
            GetComponent<SpriteRenderer>().sprite = Player_Data.Instance.ships_Skins[(int)value];
        }
    }

    public int HealthCurrent
    {
        get { return healthCurrent; }
        set
        {

            if (value >= healthMax)
            {
                healthCurrent = healthMax;
            }
            else if (value <= 0)
            {
                Death();
                return;
            }
            else
            {
                healthCurrent = value;
            }

            shipVisualController.HealthBarFillAmount = (float)HealthCurrent / healthMax;
        }
    }

    public int USPTakens
    {
        get { return _USPTakens; }
        set
        {
            _USPTakens = value > USPNeedToReproduction ? USPNeedToReproduction : value;
            float coeff = (float)_USPTakens / USPNeedToReproduction;
            USPIndicatorValue = coeff;
        }
    }

    public float USPIndicatorValue { set { shipVisualController.USPBarFillAmount = value; } }
    public float VisionRadius
    {
        get { return visionRadius; }
        set 
        { 
            visionRadius = value + radiusSize;
        }
    }
    


    private void Awake()
    {
        radiusSize = mainCollider.bounds.extents.x;

        VisionRadius = 5f;
        armor = 0;

        USP_taken = USPTaken;
        //Take_hit = TakeHit;

        Updater.Instance.RegisterNeedUpdateObject(this);
        //EffectFromDamageProjectile = shipVisualController.ExplosionAndDebrisWhenDamageTaken;
    }

    void Start()
    {
        HealthCurrent = healthMax;

        modules = GetComponents<Module>();

        //если при появлении имеется скорость движения (например, в конце размножения двигался от удара)
        if (moveDirection.x != 0 && moveDirection.y != 0)
        {
            //State = States.FLEE;
            MovingFromForce(moveDirection);
        }
        else Idle();
    }

    public void FixUpdateMe()
    {
        //if (State == States.IDLE) // перенесено в MovingWhenIdle
        Move();
    }


    public void Move()
    {
        if (movingFromForce)
        {
            if (body.velocity.sqrMagnitude > deseleration_treshold)
                return;
            else
            {
                movingFromForce = false;
                moveDirection = Vector2.zero;
                InvokeMoveDirectionZeroEvent();
                if (State == States.FLEE)
                    Idle();
            }
        }

        body.velocity = moveDirection * move_speed * Time.fixedDeltaTime;
    }

    public void Idle()
    {
        State = States.IDLE;
        if (BattleSceneController.Instance != null && searchingForUSP.SearchingNearestUSP(out USP usp))
        {
            searchingForUSP.SendShipForUSP(usp);
        }
        else
        {
            movingWhenIdleComponent.Crawling();
        }
    }

    public void MovingFromForce(Vector2 force, bool flee = true, ForceMode2D forceMode = ForceMode2D.Impulse)
    {
        if (State != States.STUNNED && State != States.REPRODUCTION && flee)
            State = States.FLEE;
        movingFromForce = true;
        body.AddForce(force, forceMode);
        
    }

    /// <summary>
    /// Останавливает корабль, но не резко, а плавно, как будто присутствует небольшая инерция.
    /// </summary>
    public void DeselerateSmoothly()
    {
        MovingFromForce(Vector2.zero, false, ForceMode2D.Force);
    }


    public void Reproduction()
    {
        if(!Global_Controller.Instance.IsEnoughCapacityForShip(playerNumber))
        {
            if (checkForCapacityCoroutine == null)
            {
                checkForCapacityCoroutine = StartCoroutine(CheckForCapacity());
                Idle();
            }
            return;
        }

        State = States.REPRODUCTION;

        DeselerateSmoothly();
        reproductionCoroutine = StartCoroutine(ReproductionGo());
    }
    
    private IEnumerator ReproductionGo()
    {
        beforeStartReproduction?.Invoke();
        shipVisualController.ReproductionBegun();

        if (timePassedFromStartReproduction < 0.01f)
            timePassedFromStartReproduction += secondsNeedToReproduction;

        while (timePassedFromStartReproduction > 0)
        {
            yield return null;
            timePassedFromStartReproduction -= Time.deltaTime;
            //fillingWithUSPForm.fillAmount = timePassedFromStartReproduction / secondsNeedToReproduction;
            //USP_bar_filling(timePassedFromStartReproduction / secondsNeedToReproduction);
            USPIndicatorValue = timePassedFromStartReproduction / secondsNeedToReproduction;

        }
        //fillingWithUSPForm.fillAmount = 0;
        //USP_bar_filling(0);
        USPIndicatorValue = 0;
        USPTakens = 0;
        //SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        //renderer.color = new Color(1, 1, 1);
        shipVisualController.ReproductionEnd();
        afterEndTimeReproduction?.Invoke();
        
        yield return null;//ждем 1 кадр во избежание глюков

        int numberOfNewShips = 1;
        if(TryGetComponent(out Multifertilis_Module multifertilis_Module))
        {
            numberOfNewShips = multifertilis_Module.Multifertilis();
        }

        for (int i = 0; i < numberOfNewShips; i++)
        {
            GameObject shipGO = Instantiate(gameObject);
            shipGO.name = $"Ship({shipName})" + ". Team " + team + ". Player " + playerNumber + ".";
            shipGO.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1); //в иерархии объектов в редакторе перемещаем новый корабль на 1 ниже, чем его сделавший

            Ship newShip = shipGO.GetComponent<Ship>();
            shipGO.transform.Translate(UnityEngine.Random.insideUnitCircle * 0.1f);

            Global_Controller.Instance.AddShip(newShip);
            newShip.USPTakens = 0;
            newShip.IsOriginal = false;
            newShip.motherShip = this;
            if (IsInvisible)
                newShip.IsInvisible = true;

        }

        reproductionCoroutine = null;

        while(movingFromForce)
        {
            yield return null;
        }
        Idle();
    }


    /// <summary>
    /// Проверяет, стало ли достаточно место для нового корабля. Если место появилось, то при подходящем State отправляет на репродукцию.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckForCapacity()
    {
        while(true)
        {
            if (!Global_Controller.Instance.IsEnoughCapacityForShip(playerNumber))
            {
                yield return new WaitForSeconds(1);
                continue;
            }
            else if(State != States.FIGHT && State != States.FLEE && State != States.STUNNED)
            {
                Reproduction();
                checkForCapacityCoroutine = null;
                break;
            }
            else
            {
                yield return new WaitForSeconds(1);
            }
        }
    }



    public void StunProjectileHit(Ship shipWhoFired, float stunTime, Vector2 direction, Vector3 impactPoint)
    {
        if (reproductionCoroutine != null)
            StopCoroutine(reproductionCoroutine);

        if (stunnedCoroutine == null)
        {
            stunnedTime = stunTime;
            stunnedCoroutine = StartCoroutine(GetStunned(stunTime));
        }
    }

    public IEnumerator GetStunned(float stunTime)
    {
        State = States.STUNNED;
        shipVisualController.Stun(stunTime);
        DeselerateSmoothly();

        while (stunnedTime > 0)
        {
            //print("stunnedTime = " + stunnedTime);
            yield return null;
            stunnedTime -= Time.deltaTime;
        }

        //shipVisualController.StunEnd();

        if (reproductionCoroutine != null)
        {
            stunnedCoroutine = null;
            Reproduction();//если стан произошел во время воспроизводства, то после истечения времени стана
                           //возвращаем микрота в состояние воспроизводства
            yield break;
        }

        if(TryGetComponent(out Giant_Module giant_Module) && giant_Module.digestionCoroutine != null)
        {
            stunnedCoroutine = null;
            giant_Module.Digestion();
            yield break;
        }
       
        if (TryGetComponent(out Attack_Module attack_Module) && attack_Module.toFightCoroutine != null && enemyCurrent != null)
        {
            stunnedCoroutine = null;
            //spriteRenderer.color = new Color(1, 1, 1, alfaChannel);
            attack_Module.toFightCoroutine = StartCoroutine(attack_Module.ToFight(enemyCurrent));
            yield break;
        }

        //spriteRenderer.color = new Color(1, 1, 1, alfaChannel);
        stunnedCoroutine = null;

        if (!movingFromForce)
            Idle();
        else State = States.FLEE;
        
    }

    ///// <summary>
    ///// Метод, исполняемый при столкновении со снарядом. Может быть заменен модулем уворота.
    ///// </summary>
    ///// <param name="damage">Получаемый урон.</param>
    ///// <param name="direction">Вектор направления полученного урона.</param>
    ///// <param name="impactPoint">Точка удара снаряда.</param>
    ///// <param name="visualEffect">Визуальный эффект, проигрываемый при попадании.</param>
    ///// <returns></returns>
    //public delegate bool TakeHitDelegate(int damage, Vector2 direction, Vector3 impactPoint, Action visualEffect);
    //public TakeHitDelegate Take_hit;
    //public Action<int, Vector2, Vector3> EffectFromDamageProjectile { get; set; }

    //public bool DamageProjectileHit(Ship shipWhoFired, int damage, Vector2 direction, Vector3 impactPoint, Action visualEffect)
    //{
    //    if (shipWhoFired != null)
    //        shipLastToDoDamage = shipWhoFired;
    //    else
    //        shipLastToDoDamage = null;

    //    return Take_hit(damage, direction, impactPoint, visualEffect);

    //}


    //private bool TakeHit(int damage, Vector2 direction, Vector3 impactPoint, Action visualEffect)
    //{
    //    taking_Damage_component.Take_Damage(damage, direction, impactPoint, visualEffect);
    //    return true;
    //}

    //public void TakeHeal(int healPoints, float chanceHealDebuff, Vector2 direction)
    //{
    //    int _healApplied = (HealthCurrent + healPoints > healthMax) ? (healthMax - HealthCurrent) : healPoints;
    //    HealthCurrent += healPoints;

    //    if(isDebuffApplied && GameEngineAssistant.GetProbability(chanceHealDebuff))
    //    {
    //        isDebuffApplied = false;
    //        print("debuff is healed");
    //    }

    //    Global_Controller.Instance.StartCoroutine(Global_Controller.Instance.VisualizationOfDamage(_healApplied, healthMax, transform.position, direction, References.Instance.colors.digits_healing_color));
    //    shipVisualController.Heal();
    //}


    public void Hit()
    {
        StartCoroutine(ReloadingHit());
    }

    IEnumerator ReloadingHit()
    {
        isReadyToHit = false;
        //Debug.Log($"ship script reload_time: {reload_time}. frame: {Time.frameCount}");
        yield return new WaitForSeconds(reload_time);
        isReadyToHit = true;
    }


    private void USPTaken()
    {
        USPTakens++;
        shipVisualController.USP_Taken();
        
        if (USPTakens >= USPNeedToReproduction)
        {
            Reproduction();
        }
    }


    #region Shot Detected

    /// <summary>
    /// Добавляет обработчик события ShotDetected.
    /// </summary>
    /// <param name="action">Делегат - обработчик события.</param>
    /// <param name="mustBeOnlyOne">Должен ли этот делегат быть единственным таким в обработчиках?</param>
    public void AddActionToShotDetectedEvent(Action action, bool mustBeOnlyOne = false)
    {
        if (mustBeOnlyOne)
        {
            var delegates = ShotDetected.GetInvocationList();
            if (!delegates.Contains(action))
            {
                ShotDetected += action;
            }
            else
            {
                Debug.LogWarning($"Shot Detected Event уже содержит делегат {action.Method.Name}");
            }
        }
        else
        {
            ShotDetected += action;
        }
    }

    /// <summary>
    /// Удаляет обработчик события ShotDetected.
    /// </summary>
    /// <param name="action"></param>
    public void DeleteActionFromShotDetectedEvent(Action action) => ShotDetected -= action;


    /// <summary>
    /// Вызывает событие ShotDetected.
    /// </summary>
    public void OnShotDetected()
    {
        ShotDetected?.Invoke();
    }

    #endregion





    #region Enemy detected

    public Action<List<Ship>, List<Ship>> Enemys_detected;
    public List<Ship> discoveredEnemies = new();
    [HideInInspector]
    public List<int> enemysDetectedIDs;

    #endregion

    private void Death()
    {
        EventManager.Instance.PostNotification(EVENT_TYPE.SHIP_DESTROYED, this, shipLastToDoDamage);
        
        GameObject deadShip = Instantiate(Prefabs.Instance.explosionOfDestroyedShip);
        deadShip.transform.position = transform.position;
        deadShip.transform.localScale = transform.localScale * deathExplosionScaleSize;
        deadShip.transform.rotation = transform.rotation;

        Updater.Instance.UnregisterNeedUpdateObject(this);
        Destroy(gameObject);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.TransformPoint(moveDirection));
        //Gizmos.DrawLine(origin, end);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, VisionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusSize + attack_range);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radiusSize);

    }

    
}
