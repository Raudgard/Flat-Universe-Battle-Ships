using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// Полоса внизу экрана, отображающая текущую мощность игрока и количество его юнитов.
/// </summary>
public class PowerBar : MonoBehaviour
{
    public RectTransform rectTransform;
    [SerializeField] private Image bar;
    [SerializeField] private TextMeshProUGUI countsTMPro;
    [SerializeField] private RectTransform countsRectTransform;

    [Range(0, 1)]
    [SerializeField] private float alphaChannelForBar;


    private int playerNumber;

    /// <summary>
    /// Максимальная мощь среди всех игроков в данном уровне.
    /// </summary>
    private static int MaxPower { get; set; }


    private void Start()
    {
        EventManager.Instance.AddListener(EVENT_TYPE.SHIP_DESTROYED, CalculateAndShowPower);
        EventManager.Instance.AddListener(EVENT_TYPE.SHIP_CREATED, CalculateAndShowPower);
        //EventManager.Instance.AddListener(EVENT_TYPE.SHIPS_COUNT_CHANGED, CalculateAndShowPower);


        EventManager.Instance.AddListener(EVENT_TYPE.QUIT_LEVEL, OnLevelQuit);

        //Высчитываем и показываем первый раз мощность через 3 кадра после начала боя.
        //Tools.UnityTools.ExecuteWithDelay(delegate { CalculateAndShowPower(EVENT_TYPE.SHIPS_COUNT_CHANGED, null, null); }, 3);
        Tools.UnityTools.ExecuteWithDelay(delegate { CalculateAndShowPower(EVENT_TYPE.SHIP_CREATED, null, null); }, 3);

    }


    public void Initialize(int playerNumber)
    {
        this.playerNumber = playerNumber;
        gameObject.name = "Power bar player " + playerNumber;

        Color color = References.Instance.colors.player_background_colors[playerNumber];
        color = new Color(color.r, color.g, color.b, alphaChannelForBar);
        bar.color = color;
        countsTMPro.color = color;

        if (Global_Controller.Instance.powerBars.Any(v => v.key == playerNumber))
        {
            Global_Controller.Instance.powerBars.Single(v => v.key == playerNumber).value = this;
        }
        else
        {
            Global_Controller.Instance.powerBars.Add(new Tools.TwoValuePair<int, PowerBar>(playerNumber, this));
        }

        GetIntoStartingPosition();

        //Высчитываем максимальную мощность через 2 кадрa после начала боя.
        Tools.UnityTools.ExecuteWithDelay(CalculateMaxPower, 2);
    }

    private void GetIntoStartingPosition()
    {
        rectTransform.anchoredPosition = new Vector2(20, 20 * playerNumber);
    }

    /// <summary>
    /// Высчитывает максимально возможную мощность и устанавливаем ее, если она больше установленной. Таким образом установится максимальная среди всех игроков мощность.
    /// </summary>
    public void CalculateMaxPower()
    {
        //Debug.Log($"CalculateMaxPower. player: {playerNumber}");
        var ships = Global_Controller.Instance.ships[playerNumber];
        int maxPowerOfShip = ships.Count > 0 ? ships.Max(s => s.Power) : 1;
        int maxPower = maxPowerOfShip * Global_Controller.Instance.shipsMaxCapacity[playerNumber];
        if (maxPower > MaxPower)
            MaxPower = maxPower;

        //Debug.Log($"maxPower: {maxPower}, MaxPower: {MaxPower}");
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="eVENT_TYPE"></param>
    /// <param name="component"></param>
    /// <param name="obj"></param>
    public void CalculateAndShowPower(EVENT_TYPE eVENT_TYPE, Component component, object obj)
    {
        //Debug.Log($"CalculateAndShowPower. player: {playerNumber}");
        var ships = Global_Controller.Instance.ships[playerNumber];

        //int power = ships.Sum(s => s.Modules.Sum(m => m.Power));
        int power = ships.Sum(s => s.Power);

        //Debug.Log($"power: {power}");

        float ratio = (float)power / MaxPower;
        //Debug.Log($"ratio: {ratio}, rect.width: {rectTransform.rect.width}");

        bar.fillAmount = ratio;
        countsTMPro.text = $"{ships.Count} / {Global_Controller.Instance.shipsMaxCapacity[playerNumber]}";
        float xPosition = rectTransform.rect.width * ratio;
        xPosition = xPosition > rectTransform.rect.width ? rectTransform.rect.width + 5 : xPosition + 5;
        countsRectTransform.anchoredPosition = new Vector2(xPosition, 0);
    }



    private void OnLevelQuit(EVENT_TYPE eVENT_TYPE, Component component, object obj)
    {
        MaxPower = 0;
        EventManager.Instance.RemoveListener(EVENT_TYPE.SHIP_DESTROYED, CalculateAndShowPower);
        //EventManager.Instance.RemoveListener(EVENT_TYPE.SHIPS_COUNT_CHANGED, CalculateAndShowPower);
        EventManager.Instance.RemoveListener(EVENT_TYPE.SHIP_CREATED, CalculateAndShowPower);

    }

}
