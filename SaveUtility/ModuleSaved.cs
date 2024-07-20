using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MODULES;
using System;

namespace SaveUtility
{
    [Serializable]
    /// <summary>
    /// Класс для сериализации и сохранения, содержащий информацию о модуле корабля.
    /// </summary>
    public class ModuleSaved
    {
#if DEBUG
        public Moduls module; //Ввел только для удобства использования скрипта TestManyShips
#endif
        public Type typeOfModule;
        public int level;
        public int energy;

        public static ModuleSaved GetModuleSaved(Type typeOfModule, int level, int energy)
        {
            var moduleSaved = new ModuleSaved();

            moduleSaved.typeOfModule = typeOfModule;
            moduleSaved.level = level;
            moduleSaved.energy = energy;
            return moduleSaved;
        }

        public static ModuleSaved GetModuleSaved(Module module)
        {
            var moduleSaved = new ModuleSaved();
            moduleSaved.typeOfModule = module.GetType();
            moduleSaved.level = module.LevelOfModule;
            moduleSaved.energy = module.energy;
            return moduleSaved;
        }

        public static ModuleSaved[] GetModulesSaved(Module[] modules)
        {
            ModuleSaved[] modulesSaved = new ModuleSaved[modules.Length];
            for(int i = 0; i < modules.Length; i++)
            {
                if (modules[i] != null)
                {
                    modulesSaved[i] = GetModuleSaved(modules[i]);
                }
            }
            return modulesSaved;
        }


    }
}
