using System.Collections.Generic;
using System.Globalization;
//using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using MODULES;
using Newtonsoft.Json;
using SaveUtility;

public class Develop_Mode_Option : MonoBehaviour
{
    public Text textDevelopMode;
    public bool isDevelopMode = false;
    //[SerializeField] private ScrollRect scrollRectDM;
    [SerializeField] private Ship shipPrefab;
    [SerializeField] private RectTransform rectContentOptions;
    [SerializeField] private GameObject DM_Options;
    //[SerializeField] private GameObject blockOfShipStatsPrefab;
    [SerializeField] private GameObject blockOfShipModulesPrefab;
    [SerializeField] private Toggle canAICreateUSPToggle;
    [SerializeField] private Toggle isFogOfWar;
    [SerializeField] private TMP_InputField rateOfAICreationUSPInputField;
    [SerializeField] private TMP_InputField USPCountPlayerInputField;
    [SerializeField] private TMP_InputField USPCountAIInputField;
    [SerializeField] private TMP_InputField sizeOfBattlefield;
    [SerializeField] private TMP_InputField capsuleLength;
    [SerializeField] private TMP_Dropdown shapeOfBattlefield;

    [SerializeField] private RectTransform numberPlayerShips_Header;
    [SerializeField] private RectTransform numberAIShips_Header;
    [SerializeField] private GameObject parceResultGO;
    [SerializeField] private TMP_Text parceResult;
    [SerializeField] private Button ApplyButton;
    [SerializeField] private float sizeOfBlockShipModules;
    [SerializeField] private float additionToContentVecticalSize;


    private Global_Controller global_Controller;
    private Colors colors;

    private Vector2 originalSizeOfRectContent;
    private Vector2 defaultPositionOfNumberAIShips_Header;

    private List<GameObject> blocksPlayersShips = new List<GameObject>();
    private List<GameObject> blocksAIShips = new List<GameObject>();


    private const float _defaultRateOfCreationUSPAI = 0.1f;
    private const int _defaultUSPCount = 500;


    //private int[,] _geneAndLvlValues; //массив значений полей названий генов и значений их уровней
    private int[] _valuesOfallNamesOfModules;

    //UnityAction<string> endEditAction;
    //TMP_InputField.SubmitEvent submitEvent;

    private void Awake()
    {
        global_Controller = Global_Controller.Instance;
        colors = global_Controller.GetComponent<Colors>();
        isDevelopMode = Player_Data.Instance.isDevelopMode;
        //isDevelopMode = false;
        textDevelopMode.color = new Color(0, 0, 0);

        originalSizeOfRectContent = rectContentOptions.sizeDelta;
        defaultPositionOfNumberAIShips_Header = numberAIShips_Header.anchoredPosition;

        parceResultGO.SetActive(false);

        ActivateDMOption();
    }

    private void Start()
    {
        //ShowCameraData();
        TMP_Dropdown[] __blocks = blockOfShipModulesPrefab.GetComponentsInChildren<TMP_Dropdown>();
        sizeOfBlockShipModules = __blocks[0].GetComponent<RectTransform>().rect.height * __blocks.Length + 120;

        if (isDevelopMode) LoadShips();
    }

    public void OnDevelopModeClick()
    {
        isDevelopMode = !isDevelopMode;
        Player_Data.Instance.isDevelopMode = isDevelopMode;

        ActivateDMOption();

        if (isDevelopMode)
        {
            textDevelopMode.color = new Color(1, 1, 1);
            LoadShips();
        }
        else
        {
            textDevelopMode.color = new Color(0, 0, 0);
        }

    }

    private void ActivateDMOption()
    {
        if (isDevelopMode)
        {
            rectContentOptions.sizeDelta = new Vector2(0, 1200);

            DM_Options.gameObject.SetActive(true);
            ApplyButton.gameObject.SetActive(true);
            SetDropdownOptionsForBattlefieldShape(shapeOfBattlefield);

        }
        else
        {
            rectContentOptions.sizeDelta = originalSizeOfRectContent;
            DM_Options.gameObject.SetActive(false);
            ApplyButton.gameObject.SetActive(false);

            foreach (GameObject GM in blocksPlayersShips)
            {
#if UNITY_EDITOR
                DestroyImmediate(GM);
#else
                Destroy(GM);
#endif
            }


            foreach (GameObject GM in blocksAIShips)
            {
#if UNITY_EDITOR
                DestroyImmediate(GM);
#else
                Destroy(GM);
#endif
            }

            blocksPlayersShips.Clear();
            blocksAIShips.Clear();
            numberPlayerShips_Header.GetComponentInChildren<TMP_Dropdown>().value = 0;
            numberAIShips_Header.GetComponentInChildren<TMP_Dropdown>().value = 0;
        }
    }


    /// <summary>
    /// Загружает корабли из PlayerPrefs
    /// </summary>
    private void LoadShips()
    {
        var data = PlayerPrefs.GetString(SCFPP.DevelopMode.shipsData, string.Empty);
        if (string.IsNullOrEmpty(data)) return;

        var shipsData = JsonConvert.DeserializeObject(data, typeof(Dictionary<int, Dictionary<int, Dictionary<int, int>>>)) as Dictionary<int, Dictionary<int, Dictionary<int, int>>>;

        int[] moduls;
        if (shipsData.ContainsKey(0))
        {
            var tMP_DropdownPlayer = numberPlayerShips_Header.GetComponentInChildren<TMP_Dropdown>();
            tMP_DropdownPlayer.value = shipsData[0].Count;
            moduls = new int[shipsData[0][0].Keys.Count];
            for (int ship = 0; ship < shipsData[0].Count; ship++)
            {
                for (int numberOfModule = 0; numberOfModule < shipsData[0][ship].Count; numberOfModule++)
                {
                    shipsData[0][ship].Keys.CopyTo(moduls, 0);
                    if (moduls[numberOfModule] < 0) continue;
                    //print("player: " + 0 + "   ship: " + ship + "    " + numberOfModule)
                    blocksPlayersShips[ship].GetComponentsInChildren<TMP_Dropdown>()[numberOfModule].value = moduls[numberOfModule];
                    blocksPlayersShips[ship].GetComponentsInChildren<TMP_InputField>()[numberOfModule].text = shipsData[0][ship][moduls[numberOfModule]].ToString();
                }
            }
        }

        if (shipsData.ContainsKey(1))
        {
            var tMP_DropdownAI = numberAIShips_Header.GetComponentInChildren<TMP_Dropdown>();
            tMP_DropdownAI.value = shipsData[1].Count;
            moduls = new int[shipsData[1][0].Keys.Count];
            for (int ship = 0; ship < shipsData[1].Count; ship++)
            {
                for (int numberOfModule = 0; numberOfModule < shipsData[1][ship].Count; numberOfModule++)
                {
                    shipsData[1][ship].Keys.CopyTo(moduls, 0);
                    if (moduls[numberOfModule] < 0) continue;
                    //print("player: " + 0 + "   ship: " + ship + "    " + numberOfModule)
                    blocksAIShips[ship].GetComponentsInChildren<TMP_Dropdown>()[numberOfModule].value = moduls[numberOfModule];
                    blocksAIShips[ship].GetComponentsInChildren<TMP_InputField>()[numberOfModule].text = shipsData[1][ship][moduls[numberOfModule]].ToString();
                }
            }
        }




        

        //var fileToSave = JsonConvert.SerializeObject(data);
        //print("fileToSave: " + fileToSave);


        //numberPlayerShips_Header.GetComponentInChildren<TMP_Dropdown>().value = ;
        //numberAIShips_Header.GetComponentInChildren<TMP_Dropdown>().value = ;
    }

     
    public void CanAICreateUSP()
    {
        Player_Data.Instance.canAICreateUSP = canAICreateUSPToggle.isOn;
    }
    public void IsFogOfWar()
    {
        Player_Data.Instance.fogOfWar = isFogOfWar.isOn;

    }

    public void SetRateOfAICreationUSP()
    {
        //string defaultValue = rateOfAICreationUSP.GetComponentInChildren<TMP_Text>().text;
        string fieldValue = rateOfAICreationUSPInputField.text;
        //print("defaultValue = " + defaultValue + "   fieldValue = " + fieldValue);

        float rate = GetValueFromField(fieldValue, _defaultRateOfCreationUSPAI, out bool success);

        if (success)
        {
            Player_Data.Instance.rateOfCreationUSPAI = rate;
            //print("parse is OK. \nPlayer_Data.Instance.rateOfCreationUSPAI =" + Player_Data.Instance.rateOfCreationUSPAI);
        }
        else
        {
            rateOfAICreationUSPInputField.GetComponentInChildren<TMP_Text>().text = _defaultRateOfCreationUSPAI.ToString();
            Player_Data.Instance.rateOfCreationUSPAI = _defaultRateOfCreationUSPAI;
            rateOfAICreationUSPInputField.text = "";
            //print("parse is NOT OK! \nPlayer_Data.Instance.rateOfCreationUSPAI =" + Player_Data.Instance.rateOfCreationUSPAI);
        }

    }

    public void SetUSPCountToPlayer()
    {
        string fieldValue = USPCountPlayerInputField.text;
        int count = GetValueFromField(fieldValue, _defaultUSPCount, out bool success);

        if (success)
        {
            Player_Data.Instance.USPCount = count;
            Player_Data.Instance.USPCountInBattle = count;
            Player_Data.Instance.howMuchUSPPlayerCouldTakeIntoBattle = count;
            //print("USPCOunt = " + Player_Data.Instance.USPCount + "    USPCountInBattle = " + Player_Data.Instance.USPCountInBattle);
        }
        else
        {
            USPCountPlayerInputField.GetComponentInChildren<TMP_Text>().text = _defaultUSPCount.ToString();
            Player_Data.Instance.USPCount = _defaultUSPCount;
            USPCountPlayerInputField.text = "";
        }
    }

    public void SetUSPCountToAI()
    {
        string fieldValue = USPCountAIInputField.text;
        int count = GetValueFromField(fieldValue, _defaultUSPCount, out bool success);

        if (success)
        {
            Player_Data.Instance.USPCountAI = count;
        }
        else
        {
            USPCountAIInputField.GetComponentInChildren<TMP_Text>().text = _defaultUSPCount.ToString();
            Player_Data.Instance.USPCountAI = _defaultUSPCount;
            USPCountAIInputField.text = "";
        }
    }

    private float GetValueFromField(string fieldValue, float defaultValue, out bool success)
    {
        if (fieldValue == "")
        {
            success = true;
            return defaultValue;
        }

        NumberFormatInfo currentNumberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;//текущие настройки для разделителя дробных чисел
        NumberFormatInfo engNumberFormatInfo = new NumberFormatInfo
        {
            NumberDecimalSeparator = "."
        };
        //print("defaultValue = " + defaultValue + "   fieldValue = " + fieldValue);

        if (float.TryParse(fieldValue, NumberStyles.Float, currentNumberFormatInfo, out float res))
        {
            success = true;
            return res;
        }
        else if (float.TryParse(fieldValue, NumberStyles.Float, engNumberFormatInfo, out res))
        {
            success = true;
            return res;
        }
        else
        {
            success = false;
            return defaultValue;
        }
    }
    private int GetValueFromField(string fieldValue, int defaultValue, out bool success)
    {
        if (fieldValue == "")
        {
            success = true;
            return defaultValue;
        }

        NumberFormatInfo currentNumberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;//текущие настройки для разделителя дробных чисел
        NumberFormatInfo engNumberFormatInfo = new NumberFormatInfo
        {
            NumberDecimalSeparator = "."
        };

        if (int.TryParse(fieldValue, NumberStyles.Integer, currentNumberFormatInfo, out int res))
        {
            success = true;
            return res;
        }
        else if (int.TryParse(fieldValue, NumberStyles.Integer, engNumberFormatInfo, out res))
        {
            success = true;
            return res;
        }
        else
        {
            success = false;
            return defaultValue;
        }
    }

    private int GetDefaultValueFromField(string fieldValue, System.Type type)
    {
        NumberFormatInfo currentNumberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;//текущие настройки для разделителя дробных чисел
        NumberFormatInfo engNumberFormatInfo = new NumberFormatInfo
        {
            NumberDecimalSeparator = "."
        };

        if (int.TryParse(fieldValue, NumberStyles.Integer, currentNumberFormatInfo, out int res))
        {
            return res;
        }
        else
        {
            int.TryParse(fieldValue, NumberStyles.Integer, engNumberFormatInfo, out res);
            return res;
        }
    }


    public void SetNumberOfShips()
    {
        //float sizeOfBlockMicStates = 400.0f;

        TMP_Dropdown tMP_DropdownPlayer = numberPlayerShips_Header.GetComponentInChildren<TMP_Dropdown>();
        //int delta = tMP_Dropdown.value - currentValueOfDDPlayerShips;
        int deltaPlayer = tMP_DropdownPlayer.value - blocksPlayersShips.Count;

        TMP_Dropdown tMP_DropdownAI = numberAIShips_Header.GetComponentInChildren<TMP_Dropdown>();
        int deltaAI = tMP_DropdownAI.value - blocksAIShips.Count;


        //print("deltaAI = " + deltaAI);

        if (deltaPlayer > 0)
        {
            for (int i = 0; i < deltaPlayer; i++)
            {
                //создаем блок для генов микрота 
                GameObject _blockOfShipsModules = Instantiate(blockOfShipModulesPrefab) as GameObject;
                TMP_Dropdown[] tMP_Dropdowns = _blockOfShipsModules.GetComponentsInChildren<TMP_Dropdown>();
                SetDropdownOptionsForModuls(tMP_Dropdowns); //меняем опции на названия всех генов

                blocksPlayersShips.Add(_blockOfShipsModules);
                _blockOfShipsModules.GetComponentInChildren<TMP_Text>().text = "Ship " + (blocksPlayersShips.Count);
                RectTransform __blockRect = _blockOfShipsModules.GetComponent<RectTransform>();
                //__blockRect.SetParent(rectContentOptions, false);

                __blockRect.SetParent(DM_Options.transform, false);


                //__blockRect.position = new Vector2(numberPlayerShips_Header.position.x - 10.0f, numberPlayerShips_Header.position.y
                //    - blocksPlayersShips.Count * 50  - (blocksPlayersShips.Count - 1) * sizeOfBlockMicStates);

                __blockRect.anchoredPosition = blocksPlayersShips.Count == 1 ?
                                       new Vector2(numberPlayerShips_Header.anchoredPosition.x - 15.0f, numberPlayerShips_Header.anchoredPosition.y - numberPlayerShips_Header.rect.height) :
                                       new Vector2(numberPlayerShips_Header.anchoredPosition.x - 15.0f, blocksPlayersShips[blocksPlayersShips.Count - 2].GetComponent<RectTransform>().anchoredPosition.y - sizeOfBlockShipModules - 10.0f);
                //print("position = "+__blockRect.position);
                //print("anchoredPosition = " + __blockRect.anchoredPosition);


                for (int j = 0; j < tMP_Dropdowns.Length; j++)
                {
                    tMP_Dropdowns[j].onValueChanged.AddListener(ModuleChoosen);
                    if (tMP_Dropdowns[j].template.TryGetComponent<ScrollRect>(out var scrollRect))
                    {
                        scrollRect.scrollSensitivity = 100;
                    };
                }



            }

            for (int i = 0; i < blocksAIShips.Count; i++)
            {
                RectTransform _blockRect = blocksAIShips[i].GetComponent<RectTransform>();
                Vector2 currentPosition = _blockRect.anchoredPosition;
                _blockRect.anchoredPosition = new Vector2(currentPosition.x, blocksPlayersShips[blocksPlayersShips.Count - 1].GetComponent<RectTransform>().anchoredPosition.y - sizeOfBlockShipModules - 10.0f - numberAIShips_Header.rect.height);
            }
        }

        if (deltaPlayer < 0)
        {
            int a = Mathf.Abs(deltaPlayer);
            for (int i = 0; i < a; i++)
            {
#if UNITY_EDITOR

                DestroyImmediate(blocksPlayersShips[blocksPlayersShips.Count - 1].gameObject);
#else
                Destroy(blocksPlayersShips[blocksPlayersShips.Count - 1].gameObject);

#endif
                blocksPlayersShips.RemoveAt(blocksPlayersShips.Count - 1);
            }
        }

        if (deltaAI > 0)
        {
            for (int i = 0; i < deltaAI; i++)
            {
                //создаем блок для статов микрота 
                GameObject _blockOfShipsModules = Instantiate(blockOfShipModulesPrefab) as GameObject;
                TMP_Dropdown[] tMP_Dropdowns = _blockOfShipsModules.GetComponentsInChildren<TMP_Dropdown>();
                SetDropdownOptionsForModuls(tMP_Dropdowns); //меняем опции на названия всех генов

                blocksAIShips.Add(_blockOfShipsModules);
                _blockOfShipsModules.GetComponentInChildren<TMP_Text>().text = "Ship " + (blocksAIShips.Count);
                RectTransform __blockRect = _blockOfShipsModules.GetComponent<RectTransform>();
                //__blockRect.SetParent(rectContentOptions, false);

                __blockRect.SetParent(DM_Options.transform, false);


                //__blockRect.position = new Vector2(numberAIShips_Header.position.x - 10.0f, numberAIShips_Header.position.y -
                //                                   blocksAIShips.Count * 50 - (blocksAIShips.Count - 1) * sizeOfBlockMicStates);
                __blockRect.anchoredPosition = blocksAIShips.Count == 1 ?
                                       new Vector2(numberAIShips_Header.anchoredPosition.x - 15.0f, numberAIShips_Header.anchoredPosition.y - numberAIShips_Header.rect.height) :
                                       new Vector2(numberAIShips_Header.anchoredPosition.x - 15.0f, blocksAIShips[blocksAIShips.Count - 2].GetComponent<RectTransform>().anchoredPosition.y - sizeOfBlockShipModules - 10.0f);
                //print("position = "+__blockRect.position);

                for (int j = 0; j < tMP_Dropdowns.Length; j++)
                {
                    tMP_Dropdowns[j].onValueChanged.AddListener(ModuleChoosen);
                    if (tMP_Dropdowns[j].template.TryGetComponent<ScrollRect>(out var scrollRect))
                    {
                        scrollRect.scrollSensitivity = 100;
                    };
                }

            }
        }

        if (deltaAI < 0)
        {
            int a = Mathf.Abs(deltaAI);
            for (int i = 0; i < a; i++)
            {
#if UNITY_EDITOR

                DestroyImmediate(blocksAIShips[blocksAIShips.Count - 1].gameObject);
#else
                Destroy(blocksAIShips[blocksAIShips.Count - 1].gameObject);

#endif
                blocksAIShips.RemoveAt(blocksAIShips.Count - 1);
            }
        }

        //изменяем размер области для контента
        rectContentOptions.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, additionToContentVecticalSize + (blocksPlayersShips.Count + blocksAIShips.Count) * (20 + sizeOfBlockShipModules));


        //сдвигаем вниз поле для введения числа микротов AI
        //numberAIShips_Header.anchoredPosition = new Vector2(defaultPositionOfNumberAIShips_Header.x, defaultPositionOfNumberAIShips_Header.y - blocksPlayersShips.Count * 130 - blocksPlayersShips.Count * sizeOfBlockMicStates);

        numberAIShips_Header.anchoredPosition = blocksPlayersShips.Count > 0 ?
            new Vector2(defaultPositionOfNumberAIShips_Header.x, blocksPlayersShips[blocksPlayersShips.Count - 1].GetComponent<RectTransform>().anchoredPosition.y - sizeOfBlockShipModules - 20) :
            defaultPositionOfNumberAIShips_Header;


        FillAllNamesOfModulesArray();
    }



    public void ApplyDM()
    {
        List<Ship> playerShips = new List<Ship>();
        List<Ship> AIShips = new List<Ship>();
        List<string> errors = new List<string>();
        bool success;
        int _defaultValueInt;
        Dictionary<int, Dictionary<int, Dictionary<int, int>>> dataToSave = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();

        //float _defaultValueFloat;

        for (int i = 0; i < blocksPlayersShips.Count; i++)
        {
            //print("blocksPlayersShips.Count = " + blocksPlayersShips.Count);
            //Ship ship = Instantiate(shipPrefab) as Ship;
            var randomPosition = new Vector2(UnityEngine.Random.Range(-2.5f, 2.5f), UnityEngine.Random.Range(-4.0f, -0.5f));
            var ship = global_Controller.CreateShip(1, 1, (Ship.Forms)i, randomPosition);
            if(!dataToSave.ContainsKey(0)) dataToSave.Add(0, new Dictionary<int, Dictionary<int, int>>());
            dataToSave[0].Add(i, new Dictionary<int, int>());

            TMP_Dropdown[] namesOfModules = blocksPlayersShips[i].GetComponentsInChildren<TMP_Dropdown>();
            ship.Modules = new MODULES.Module[namesOfModules.Length];

            for (int j = 0; j < namesOfModules.Length; j++)
            {
                //print($"namesOfModules[{j}].value = " + namesOfModules[j].value);
                //print("number of namesOfModules = " + namesOfModules.Length);

                //проверка на совпадение генов у одного микрота
                for (int k = 0; k < j - k; k++)
                {
                    if (namesOfModules[k].value != 0 && namesOfModules[k].value == namesOfModules[j].value)
                    {
                        errors.Add($"Совпадают гены у {i} микрота игрока\n");
                    }
                }

                if (namesOfModules[j].value == 0)
                {
                    dataToSave[0][i].Add(-j,0);
                    continue; // 0 - NONE (ген не выбран)
                }

                var typeOfModule = Module.GetTypeOfModule(namesOfModules[j].options[namesOfModules[j].value].text);
                Module module = ship.gameObject.AddComponent(typeOfModule) as Module;

                TMP_InputField lvlOfModule = namesOfModules[j].GetComponentInChildren<TMP_InputField>();
                _defaultValueInt = GetDefaultValueFromField(lvlOfModule.GetComponentInChildren<TMP_Text>().text, typeof(int));

                var levelOfModule = GetValueFromField(lvlOfModule.text, _defaultValueInt, out success);
                module.LevelOfModule = levelOfModule;
                ship.Modules[j] = module;
                if (!success)
                {
                    errors.Add($"{blocksPlayersShips[i].GetComponent<TMP_Text>().text}: {blocksPlayersShips[i].GetComponentsInChildren<TMP_Text>()[1].text}; ");
                    lvlOfModule.text = "";
                }
                if(!dataToSave[0][i].ContainsKey(namesOfModules[j].value))
                    dataToSave[0][i].Add(namesOfModules[j].value, levelOfModule);
            }

            ship.shipBars.SetActive(false);//убираем полоски жизней у загруженных микротов
            playerShips.Add(ship);
        }

        Vector3[] StartingPositions()
        {
            if (blocksAIShips.Count == 1)
                return new Vector3[] { new Vector3(0, 4, -1) };

            if (blocksAIShips.Count == 2)
            {
                Vector3[] res = new Vector3[2];
                res[0] = new Vector3(-4, 4, -1);
                res[1] = new Vector3(4, 4, -1);
                return res;
            }

            if (blocksAIShips.Count == 3)
                return new Vector3[]
                {
                    new Vector3(-5, 4, -1),
                    new Vector3(0, 4, -1),
                    new Vector3(5, 4, -1)
                };

            else
            {
                Vector3[] res = new Vector3[blocksAIShips.Count];
                for (int i = 0; i < res.Length; i++)
                    res[i] = new Vector3(UnityEngine.Random.Range(-4.0f, 4.0f), UnityEngine.Random.Range(-4.0f, 4.0f), -1);

                return res;
            }
        }

        for (int i = 0; i < blocksAIShips.Count; i++)
        {
            var shipAI = global_Controller.CreateShip(2, 2, (Ship.Forms)i, StartingPositions()[i]);
            if (!dataToSave.ContainsKey(1)) dataToSave.Add(1, new Dictionary<int, Dictionary<int, int>>());
            dataToSave[1].Add(i, new Dictionary<int, int>());

            TMP_Dropdown[] namesOfModules = blocksAIShips[i].GetComponentsInChildren<TMP_Dropdown>();
            shipAI.Modules = new MODULES.Module[namesOfModules.Length];

            for (int j = 0; j < namesOfModules.Length; j++)
            {
                //print($"namesOfModules[{j}].value = " + namesOfModules[j].value);
                //print("number of namesOfModules = " + namesOfModules.Length);

                //проверка на совпадение генов у одного микрота
                for (int k = 0; k < j - k; k++)
                {
                    if (namesOfModules[k].value != 0 && namesOfModules[k].value == namesOfModules[j].value)
                    {
                        errors.Add($"Совпадают гены у {i} микрота AI\n");
                    }
                }

                if (namesOfModules[j].value == 0)
                {
                    dataToSave[1][i].Add(-j,0);
                    continue; // 0 - NONE (ген не выбран)
                }

                var typeOfModule = Module.GetTypeOfModule(namesOfModules[j].options[namesOfModules[j].value].text);
                Module module = shipAI.gameObject.AddComponent(typeOfModule) as Module;

                TMP_InputField lvlOfModule = namesOfModules[j].GetComponentInChildren<TMP_InputField>();
                _defaultValueInt = GetDefaultValueFromField(lvlOfModule.GetComponentInChildren<TMP_Text>().text, typeof(int));

                var levelOfModule = GetValueFromField(lvlOfModule.text, _defaultValueInt, out success);
                module.LevelOfModule = levelOfModule;
                shipAI.Modules[j] = module;
                if (!success)
                {
                    errors.Add($"{blocksAIShips[i].GetComponent<TMP_Text>().text}: {blocksAIShips[i].GetComponentsInChildren<TMP_Text>()[1].text}; ");
                    lvlOfModule.text = "";
                }
                if (!dataToSave[1][i].ContainsKey(namesOfModules[j].value))
                    dataToSave[1][i].Add(namesOfModules[j].value, levelOfModule);

            }

            shipAI.SensivityRadiusSqr = AI.sensivityRadiusSqr;
            AIShips.Add(shipAI);
        }

        parceResultGO.SetActive(true);
        if (errors.Count == 0)
        {
            parceResult.color = new Color(0.4f, 1, 0.4f);
            parceResult.text = "Success!";

            //for new SaveDataSystem
            Player_Data.Instance.playerShips = playerShips.ToArray();
            Player_Data.Instance.enemyShips = AIShips.ToArray();
            SaveUtil.Save();


            SetUSPCountToAI();
            SetUSPCountToPlayer();

            string _dataToSave;
            if (dataToSave.Count > 0)
            {
                _dataToSave = JsonConvert.SerializeObject(dataToSave);
            }
            else _dataToSave = string.Empty;

            //print(_dataToSave);
            PlayerPrefs.SetString(SCFPP.DevelopMode.shipsData, _dataToSave);
        }
        else
        {
            parceResult.color = new Color(1, 0.4f, 0.4f);
            parceResult.text = "Error!\n";
            for (int i = 0; i < errors.Count; i++)
            {
                parceResult.text += errors[i];
            }

            foreach (Ship ship in playerShips)
#if UNITY_EDITOR
                DestroyImmediate(ship.gameObject);
#else
                Destroy(ship.gameObject);
#endif
            playerShips.Clear();

            foreach (Ship shipAI in AIShips)
#if UNITY_EDITOR
                DestroyImmediate(shipAI.gameObject);
#else
                Destroy(shipAI.gameObject);
#endif
            AIShips.Clear();
        }

        //if(playerShips[0] != null)
        //    Player_Data.Instance.shipForMaze = playerShips[0];
        Player_Data.Instance.shapeOfBattlefield = (ShapeOfBattlefield)shapeOfBattlefield.value;
        Player_Data.Instance.sizeOfBattleField = GetValueFromField(sizeOfBattlefield.text, 20, out _);
        Player_Data.Instance.capsuleLength = GetValueFromField(capsuleLength.text, 6, out _);


    }




    public void FillAllShips()
    {
        if (blocksPlayersShips.Count > 0 && blocksPlayersShips.Count + blocksAIShips.Count > 1)
        {
            TMP_InputField[] exampleInputFilds = blocksPlayersShips[0].GetComponentsInChildren<TMP_InputField>();
            TMP_Dropdown[] _exampleModules = blocksPlayersShips[0].GetComponentsInChildren<TMP_Dropdown>();

            for (int i = 1; i < blocksPlayersShips.Count; i++)
            {
                TMP_InputField[] tMP_InputField = blocksPlayersShips[i].GetComponentsInChildren<TMP_InputField>();
                for (int j = 0; j < tMP_InputField.Length; j++)
                {
                    tMP_InputField[j].text = exampleInputFilds[j].text;
                }

            }

            for (int i = 0; i < blocksAIShips.Count; i++)
            {
                TMP_InputField[] tMP_InputField = blocksAIShips[i].GetComponentsInChildren<TMP_InputField>();
                for (int j = 0; j < tMP_InputField.Length; j++)
                {
                    tMP_InputField[j].text = exampleInputFilds[j].text;
                }

            }

            for (int i = 1; i < blocksPlayersShips.Count; i++)
            {
                TMP_Dropdown[] tMP_Dropdowns = blocksPlayersShips[i].GetComponentsInChildren<TMP_Dropdown>();
                for (int j = 0; j < tMP_Dropdowns.Length; j++)
                {
                    tMP_Dropdowns[j].value = _exampleModules[j].value;
                }
            }

            for (int i = 0; i < blocksAIShips.Count; i++)
            {
                TMP_Dropdown[] tMP_Dropdowns = blocksAIShips[i].GetComponentsInChildren<TMP_Dropdown>();
                for (int j = 0; j < tMP_Dropdowns.Length; j++)
                {
                    tMP_Dropdowns[j].value = _exampleModules[j].value;
                }
            }

        }
    }

    public void FillAIShips()
    {
        if (blocksAIShips.Count > 1)
        {
            TMP_InputField[] exampleInputFilds = blocksAIShips[0].GetComponentsInChildren<TMP_InputField>();
            for (int i = 1; i < blocksAIShips.Count; i++)
            {
                TMP_InputField[] tMP_InputField = blocksAIShips[i].GetComponentsInChildren<TMP_InputField>();
                for (int j = 0; j < tMP_InputField.Length; j++)
                {
                    tMP_InputField[j].text = exampleInputFilds[j].text;
                }

            }

            TMP_Dropdown[] exampleModules = blocksAIShips[0].GetComponentsInChildren<TMP_Dropdown>();
            for (int i = 1; i < blocksAIShips.Count; i++)
            {
                TMP_Dropdown[] tMP_Dropdowns = blocksAIShips[i].GetComponentsInChildren<TMP_Dropdown>();
                for (int j = 0; j < tMP_Dropdowns.Length; j++)
                {
                    tMP_Dropdowns[j].value = exampleModules[j].value;
                }

            }

        }

    }

    public void FillAIShipsFromPlayerShips()
    {
        if (blocksPlayersShips.Count > 0)
        {
            for (int i = 0; i < blocksPlayersShips.Count; i++)
            {
                TMP_InputField[] exampleInputField = blocksPlayersShips[i].GetComponentsInChildren<TMP_InputField>();

                if (blocksAIShips.Count < i + 1)
                    return;

                TMP_InputField[] tMP_InputField = blocksAIShips[i].GetComponentsInChildren<TMP_InputField>();
                for (int j = 0; j < exampleInputField.Length; j++)
                {
                    tMP_InputField[j].text = exampleInputField[j].text;
                }

            }

            for (int i = 0; i < blocksPlayersShips.Count; i++)
            {
                TMP_Dropdown[] exampleModules = blocksPlayersShips[i].GetComponentsInChildren<TMP_Dropdown>();

                if (blocksAIShips.Count < i + 1)
                    return;

                TMP_Dropdown[] tMP_Dropdowns = blocksAIShips[i].GetComponentsInChildren<TMP_Dropdown>();
                for (int j = 0; j < exampleModules.Length; j++)
                {
                    tMP_Dropdowns[j].value = exampleModules[j].value;
                }

            }


        }

    }

    private void SetDropdownOptionsForModuls(TMP_Dropdown[] tMP_Dropdowns)
    {
        int genesCount = System.Enum.GetNames(typeof(Moduls)).Length;
        string[] namesOfModules = System.Enum.GetNames(typeof(Moduls));

        List<TMP_Dropdown.OptionData> listOfNamesOfModules = new List<TMP_Dropdown.OptionData>();
        listOfNamesOfModules.Add(new TMP_Dropdown.OptionData("NONE"));

        for (int i = 0; i < genesCount; i++)
        {
            listOfNamesOfModules.Add(new TMP_Dropdown.OptionData(namesOfModules[i]));
        }

        for (int i = 0; i < tMP_Dropdowns.Length; i++)
        {
            tMP_Dropdowns[i].options = listOfNamesOfModules;
        }
    }

    private void SetDropdownOptionsForBattlefieldShape(TMP_Dropdown tMP_Dropdown)
    {
        int shapeCount = System.Enum.GetNames(typeof(ShapeOfBattlefield)).Length;
        string[] namesOfShapes = System.Enum.GetNames(typeof(ShapeOfBattlefield));

        List<TMP_Dropdown.OptionData> listOfShapes = new List<TMP_Dropdown.OptionData>();
        //listOfNamesOfModules.Add(new TMP_Dropdown.OptionData("NONE"));

        for (int i = 0; i < shapeCount; i++)
        {
            listOfShapes.Add(new TMP_Dropdown.OptionData(namesOfShapes[i]));
        }

        tMP_Dropdown.options = listOfShapes;
        //for (int i = 0; i < tMP_Dropdown.Length; i++)
        //{
        //    tMP_Dropdown[i].options = listOfShapes;
        //}
    }



    /// <summary>
    /// Заполняет массив уровней генов микротов
    /// </summary>
    private void FillAllNamesOfModulesArray()
    {
        //print("FILL ARRAY");
        int _allModulesCount = blocksPlayersShips.Count * 5 + blocksAIShips.Count * 5;
        int currentPositionInArray = 0;//счетчик. Отслеживает текущую позицию в массиве.

        _valuesOfallNamesOfModules = new int[_allModulesCount];

        for (int i = 0; i < blocksPlayersShips.Count; i++)//перебираем блоки микротов игрока
        {
            TMP_Dropdown[] tMP_Dropdowns = blocksPlayersShips[i].GetComponentsInChildren<TMP_Dropdown>();

            for (int j = 0; j < tMP_Dropdowns.Length; j++)
            {
                _valuesOfallNamesOfModules[currentPositionInArray] = tMP_Dropdowns[j].value;
                currentPositionInArray++;
                //print("currentPositionInArray = " + currentPositionInArray);
            }

        }



        for (int i = 0; i < blocksAIShips.Count; i++)//перебираем блоки микротов АИ
        {
            TMP_Dropdown[] tMP_Dropdowns = blocksAIShips[i].GetComponentsInChildren<TMP_Dropdown>();

            for (int j = 0; j < tMP_Dropdowns.Length; j++)
            {
                _valuesOfallNamesOfModules[currentPositionInArray] = tMP_Dropdowns[j].value;
                currentPositionInArray++;
                //print("currentPositionInArray = " + currentPositionInArray);
            }
        }

    }

    private void ModuleChoosen(int value)
    {
        //print("count of inputFields = " + _valuesOfallNamesOfModules.Length + "   value = " + value);

        int currentPositionInArray = 0; //счетчик позиции в массиве
        TMP_Dropdown changedDropDown = null;

        for (int i = 0; i < blocksPlayersShips.Count; i++)
        {
            TMP_Dropdown[] tMP_Dropdowns = blocksPlayersShips[i].GetComponentsInChildren<TMP_Dropdown>();

            for (int j = 0; j < tMP_Dropdowns.Length; j++)
            {
                //print($"allNamesOfModules[{currentPositionInArray}] = " + _valuesOfallNamesOfModules[currentPositionInArray] + $"    tMP_Dropdowns[{j}].value = " + tMP_Dropdowns[j].value);
                if (_valuesOfallNamesOfModules[currentPositionInArray] != tMP_Dropdowns[j].value)
                {
                    //print("Got it!    Changed name of Module number = " + currentPositionInArray + "   value = " + value);
                    changedDropDown = tMP_Dropdowns[j];
                }
                currentPositionInArray++;
            }
        }

        for (int i = 0; i < blocksAIShips.Count; i++)
        {
            TMP_Dropdown[] tMP_Dropdowns = blocksAIShips[i].GetComponentsInChildren<TMP_Dropdown>();

            for (int j = 0; j < tMP_Dropdowns.Length; j++)
            {
                if (_valuesOfallNamesOfModules[currentPositionInArray] != tMP_Dropdowns[j].value)
                {
                    //print("Got it!    Changed name of Module number = " + currentPositionInArray + "   value = " + value);
                    changedDropDown = tMP_Dropdowns[j];
                }
                currentPositionInArray++;
            }
        }

        FillAllNamesOfModulesArray();//переписываем массив со значениями имен генов


        TMP_InputField tMP_InputField = changedDropDown.GetComponentInChildren<TMP_InputField>();
        TMP_Text tMP_Text = tMP_InputField.GetComponentInChildren<TMP_Text>();
        //print("tMP_Text.text = " + tMP_Text.text);

        if (value == 0)
        {
            tMP_Text.text = "0";
            return;
        }


        //System.Type typeOfModule = MODULS.Module.GetTypeOfModule(value - 1);
        var typeOfModule = Module.GetTypeOfModule(((Moduls)(value - 1)).ToString());
        //print("typeOfModule = " + typeOfModule);

        var method = typeOfModule.GetMethod("GetMaxLevel");
        int maxLvl = (int)method.Invoke(null, null);
        //print("maxLvl = " + maxLvl);

        tMP_Text.text = "" + maxLvl;

    }
}



