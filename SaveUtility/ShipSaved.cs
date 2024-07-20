using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MODULES;
using System.Linq;

namespace SaveUtility
{
    /// <summary>
    /// Представляет собой данные для сохранения одного корабля.
    /// </summary>
    public class ShipSaved
    {
        public int team;
        public int playerNumber;
        public Ship.Forms form;
        public (float X, float Y) startingPosition;
        public List<ModuleSaved> modules;

        public ShipSaved(int team, int playerNumber, Ship.Forms form, Vector2 startingPosition, params Module[] modules)
        {
            this.team = team;
            this.playerNumber = playerNumber;
            this.form = form;
            this.startingPosition = (startingPosition.x, startingPosition.y);
            this.modules = ModuleSaved.GetModulesSaved(modules).ToList();
        }

        public ShipSaved()
        {
            team = 0;
            playerNumber = 0;
            form = Ship.Forms.Alpha;
            modules = new List<ModuleSaved>();
            startingPosition = (0, 0);
        }
    }
}
