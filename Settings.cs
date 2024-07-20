using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings", order = 2)]
    public class Settings : ScriptableObject
    {
        [Header("Различные настройки")]

        /// <summary>
        /// Максимальное количество кораблей для игрока.
        /// </summary>
        [Tooltip("Максимальное количество кораблей для игрока.")]
        public int maxShipsCapacity;

        /// <summary>
        /// Уровень модуля, с которого начинает работать ультимативная способность.
        /// </summary>
        [Tooltip("Уровень модуля, с которого начинает работать ультимативная способность.")]
        public int moduleUltimateLevel;

        /// <summary>
        /// Время прикосновения пальцем к экрану до начала какой-либо обработки.
        /// </summary>
        [Tooltip("Время прикосновения пальцем к экрану до начала какой-либо обработки.")]
        [Foldout("Holding Touch Settings")] public float touchTimeBeforeReaction;

        /// <summary>
        /// Время прикосновения пальцем к экрану, неообходимое для установления реакции.
        /// </summary>
        [Tooltip("Время прикосновения пальцем к экрану, неообходимое для установления реакции.")]
        [Foldout("Holding Touch Settings")] public float touchTimeForReaction;

        /// <summary>
        /// Время между созданием USP при удержании пальца на экране.
        /// </summary>
        [Tooltip("Время между созданием USP при удержании пальца на экране.")]
        [Foldout("Holding Touch Settings")] public float timeBetweenUSPCreationWhileHolding;

        /// <summary>
        /// Радиус окружности, внутри которой случайным образом генерируются координаты для нового USP при долгом нажатии на экран.
        /// </summary>
        [Tooltip("Радиус окружности, внутри которой случайным образом генерируются координаты для нового USP при долгом нажатии на экран.")]
        [Foldout("Holding Touch Settings")] public float holdingTouchCreationRadius;





        [Space]
        [Header("Настройки визуальных эффектов корабля.")]


        #region visualEffectController


        /// <summary>
        /// Время проявления при обнаружении стелс.
        /// </summary>
        [Tooltip("Время проявления при обнаружении стелс.")]
        [Foldout("Visual Effects Controller Settings")] public float timeToBecameVisible;

        [Foldout("Visual Effects Controller Settings")] public float timeOfDamageTakenColor;
        [Foldout("Visual Effects Controller Settings")] public float timeOfUSPTakenColor;
        [Foldout("Visual Effects Controller Settings")] public float speedOfVanishing;
        /// <summary>
        /// Альфа канал для исчезнувшего стелс кораблей игрока.
        /// </summary>
        [Tooltip("Альфа канал для исчезнувшего стелс кораблей игрока.")]
        [Foldout("Visual Effects Controller Settings")] public float minAlphaChannelForStealthUserPlayer;

        #endregion


        #region UltimateChances

        [System.Serializable]
        public class UltimateChance
        {
            public MODULES.Moduls module;
            public float chance;
        }


        [Space]
        [Header("Настройки шанса срабатывания ультимейтов модулей.")]

        public List<UltimateChance> ultimateChances;

        #endregion

        


    }
}