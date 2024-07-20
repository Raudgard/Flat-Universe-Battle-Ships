using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Texts
{
    public enum Character
    {
        MAIN_HERO,
        GOLERNA,
        EMPEROR,
        CAPTAINS_MATE,
        ENEMY_OFFICER,
        UNKNOWN
    }
    //public class Character
    //{
    //    public string Name { get; set; }
    //    public Character(string name)
    //    {
    //        Name = name;
    //    }


    //}


    public static class Characters
    {
        public static string Main_hero = "Main character";
        public static string Gol_erna = "Gol'erna";
        public static string Emperor = "Emperor";
        public static string Captains_mate = "Captain's mate";
        public static string Enemy_officer = "Enemy officer";
        public static string Unknown = "Unknown";

        public static Character GetCharacters(string name)
        {
            return name switch
            {
                "Main character" => Character.MAIN_HERO,
                "Gol'erna" => Character.GOLERNA,
                "Emperor" => Character.EMPEROR,
                "Captain's mate" => Character.CAPTAINS_MATE,
                "Enemy officer" => Character.ENEMY_OFFICER,
                "Unknown" => Character.UNKNOWN,
                _ => throw new System.Exception()
            };
        }
    }
}
