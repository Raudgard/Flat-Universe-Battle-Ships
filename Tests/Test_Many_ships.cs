using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MODULES;
using SaveUtility;

public class Test_Many_ships : MonoBehaviour
{
#if DEBUG
    private Global_Controller global_controller;

    public bool createSamePlayer;
    public Vector2 spawnCoordinates;
    public float radiusOfSpawnZone;
    public List<ModuleSaved> modules;
    public int count = 0;
    public int team = 1;
    public int playerNumber;

    void Start()
    {
        global_controller = Global_Controller.Instance;
    }

    private void Update()
    {
        if(count > 0)
        {
            CreateShip();
            count--;
        }
    }




    private void CreateShip()
    {
        Ship ship;
        Vector2 place = spawnCoordinates + Random.insideUnitCircle;

        if (createSamePlayer && global_controller.ships[playerNumber].Count > 0 && global_controller.ships[playerNumber][0] != null)
        {
            ship = global_controller.CreateShip(team, playerNumber, Ship.Forms.Alpha, place, ModuleSaved.GetModulesSaved(global_controller.ships[playerNumber][0].Modules));
        }
        else if(modules.Count > 0)
        {
            var modules = new ModuleSaved[this.modules.Count];
            for(int i = 0; i < modules.Length; i++)
            {
                modules[i] = new ModuleSaved() { typeOfModule = Module.GetTypeOfModule(this.modules[i].module.ToString()), level = this.modules[i].level };
            }

            ship = global_controller.CreateShip(team, playerNumber, Ship.Forms.Alpha, place, modules);
        }
        else
        {
            ship = global_controller.CreateShip(team, playerNumber, Ship.Forms.Alpha, place,
                new SaveUtility.ModuleSaved() { typeOfModule = typeof(Attack_Module), level = 110 },
                    new SaveUtility.ModuleSaved() { typeOfModule = typeof(Health_Module), level = 110 },
                    new SaveUtility.ModuleSaved() { typeOfModule = typeof(Movement_Speed_Module), level = 110 },
                    new SaveUtility.ModuleSaved() { typeOfModule = typeof(Armor_Module), level = 110 });
        }

        var randomPlace = Random.insideUnitCircle * radiusOfSpawnZone;
        ship.transform.position = new Vector3(place.x + randomPlace.x, place.y + randomPlace.y, - 1);
        global_controller.AddShip(ship);
        
    }
#endif
}
