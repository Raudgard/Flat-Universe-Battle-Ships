using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using MODULES;


namespace SaveUtility
{

    /// <summary>
    /// �������� ���������� ��� ���������� � ��������. ������������� � json.
    /// </summary>
    public class SaveData
    {
        /// <summary>
        /// ���������� �������� �������� ������.
        /// </summary>
        public int countShips;

        /// <summary>
        /// ������� ������.
        /// </summary>
        public List<ShipSaved> ships = new List<ShipSaved>();




        public static SaveData GetSaveData(List<Ship> ships)
        {
            var saveData = new SaveData();
            
            saveData.countShips = ships.Count;
            for(int i = 0; i < ships.Count; i++)
            {
                saveData.ships.Add(new ShipSaved(ships[i].team, ships[i].playerNumber, ships[i].Form, ships[i].startingPosition, ships[i].Modules));
            }
            return saveData;
        }

        

    }


    
}