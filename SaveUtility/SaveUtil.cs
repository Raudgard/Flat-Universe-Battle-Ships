using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Linq;
using MODULES;

namespace SaveUtility
{
    /// <summary>
    /// —одержит, сохран€ет и загружает все игровые данные.
    /// </summary>
    public class SaveUtil
    {
        public static SaveData dataOfPlayer;
        public static SaveData dataOfAI;

        //public static List<Ship> playerShips;
        //public static List<Ship> enemyShips;
        private static string file_name = "data0.dat";
        private static string enemy_ships_file_name = "data1.dat";







        /// <summary>
        /// Serialize all data and save to file.
        /// </summary>
        /// <returns></returns>
        public static bool Save()
        {
            try
            {
                
                dataOfPlayer = SaveData.GetSaveData(Player_Data.Instance.playerShips.ToList());

                var shipsFile = JsonConvert.SerializeObject(dataOfPlayer);
                File.WriteAllText(Path.Combine(Application.persistentDataPath, file_name), shipsFile);

                if (Player_Data.Instance.enemyShips != null)
                {
                    var enemyShips = Player_Data.Instance.enemyShips;
                    if (enemyShips.Length > 0) dataOfAI = SaveData.GetSaveData(enemyShips.ToList());
                    else dataOfAI = SaveData.GetSaveData(new List<Ship>());

                    var enemysShipsFile = JsonConvert.SerializeObject(dataOfAI);
                    File.WriteAllText(Path.Combine(Application.persistentDataPath, enemy_ships_file_name), enemysShipsFile);
                }

            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "   " + e.StackTrace);
                return false;
            }
            return true;
        }


        public static bool Load()
        {
            var file0_Path = Path.Combine(Application.persistentDataPath, file_name);
            var file1_Path = Path.Combine(Application.persistentDataPath, enemy_ships_file_name);

            string ships_file;
            string enemy_ships_file;

            try
            {
                if (File.Exists(file0_Path) && File.Exists(file1_Path))
                {
                    ships_file = File.ReadAllText(file0_Path);
                    enemy_ships_file = File.ReadAllText(file1_Path);

                }
                else
                {
                    Debug.Log("Files do not exists! Player play first time!");
                    var ships = new List<Ship>();
                    var _global_controller = Global_Controller.Instance;
                    ships.Add(_global_controller.CreateShip(1, 1, Ship.Forms.Alpha, Vector2.zero,
                        ModuleSaved.GetModuleSaved(typeof(Attack_Module), 1, 0),
                        ModuleSaved.GetModuleSaved(typeof(Health_Module), 1, 0),
                        ModuleSaved.GetModuleSaved(typeof(Movement_Speed_Module), 1, 0)));
                    dataOfPlayer = SaveData.GetSaveData(ships);

                    return true;
                }

                dataOfPlayer = JsonConvert.DeserializeObject<SaveData>(ships_file);
                dataOfAI = JsonConvert.DeserializeObject<SaveData>(enemy_ships_file);

                //Debug.Log("ships count: " + dataOfPlayer.countShips);
                //Debug.Log("enemys ships count: " + dataOfAI.countShips);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "   " + e.StackTrace);
                return false;
            }

            return true;
        }

    }
}