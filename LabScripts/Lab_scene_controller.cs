using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MODULES;


public class Lab_scene_controller : MonoBehaviour
{
    private Ship[] ships;

    public Sprite armorModuleSprite;
    public Sprite attackModuleSprite;
    public Sprite attackSpeedModuleSprite;
    public Sprite HealthModuleSprite;
    public Sprite movementSpeedModuleSprite;

    public Module geneIconPrefab;

    void Start()
    {
        Player_Data.ResizeCamera();

        ships = FindObjectsOfType<Ship>();

        for (int i = 0; i < ships.Length; i++)
        {
            if (ships[i] != null)
            {
                //ships[i].State = Ship.States.IN_HANGAR;
                MovingWhenIdle movingWhenIdle = ships[i].GetComponent<MovingWhenIdle>();
                movingWhenIdle.enabled = false;

                ships[i].transform.position = new Vector3(-4, i * 3 - 6, -1);

                for (int j = 0; j < ships[i].Modules.Length; j++)
                {
                    if (ships[i].Modules[j] != null)
                    {
                        Module geneIcon = Instantiate(geneIconPrefab) as Module;
                        geneIcon.transform.position = new Vector3(ships[i].transform.position.x + 1.0f * (j + 1), ships[i].transform.position.y, -1);
                        geneIcon.GetComponent<SpriteRenderer>().sprite = GetSpriteOfModule(ships[i].Modules[j].moduleType);
                        geneIcon.GetComponent<Module>().moduleType = ships[i].Modules[j].moduleType;
                        geneIcon.GetComponent<Module>().LevelOfModule = ships[i].Modules[j].LevelOfModule;
                        print("nameOfModule = " + ships[i].Modules[j].moduleType.ToString() + "   LevelOfModule = " + ships[i].Modules[j].LevelOfModule);
                        geneIcon.name = ships[i].Modules[j].ToString();

                        TextMeshPro[] TMProForLevelOfModule = geneIcon.GetComponentsInChildren<TextMeshPro>();
                        foreach (TextMeshPro textMeshPro in TMProForLevelOfModule)
                        {
                            if (textMeshPro != null)
                                textMeshPro.text = ships[i].Modules[j].LevelOfModule.ToString();
                        }//отображение уровня гена в дочернем объекте TMPro

                    }
                }
            }
        }


    }

    
    private Sprite GetSpriteOfModule(Moduls gene)
    {
        switch (gene)
        {
            case Moduls.ARMOR_MODULE:
                return armorModuleSprite;
            case Moduls.ATTACK_MODULE:
                return attackModuleSprite;
            case Moduls.ATTACK_SPEED_MODULE:
                return attackSpeedModuleSprite;
            case Moduls.MOVEMENT_SPEED_MODULE:
                return movementSpeedModuleSprite;
            case Moduls.HEALTH_MODULE:
                return HealthModuleSprite;

            default:
                return armorModuleSprite;
        }
    }
}
