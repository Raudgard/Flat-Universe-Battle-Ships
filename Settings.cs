using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Settings", order = 2)]
    public class Settings : ScriptableObject
    {
        [Header("��������� ���������")]

        /// <summary>
        /// ������������ ���������� �������� ��� ������.
        /// </summary>
        [Tooltip("������������ ���������� �������� ��� ������.")]
        public int maxShipsCapacity;

        /// <summary>
        /// ������� ������, � �������� �������� �������� ������������� �����������.
        /// </summary>
        [Tooltip("������� ������, � �������� �������� �������� ������������� �����������.")]
        public int moduleUltimateLevel;

        /// <summary>
        /// ����� ������������� ������� � ������ �� ������ �����-���� ���������.
        /// </summary>
        [Tooltip("����� ������������� ������� � ������ �� ������ �����-���� ���������.")]
        [Foldout("Holding Touch Settings")] public float touchTimeBeforeReaction;

        /// <summary>
        /// ����� ������������� ������� � ������, ������������ ��� ������������ �������.
        /// </summary>
        [Tooltip("����� ������������� ������� � ������, ������������ ��� ������������ �������.")]
        [Foldout("Holding Touch Settings")] public float touchTimeForReaction;

        /// <summary>
        /// ����� ����� ��������� USP ��� ��������� ������ �� ������.
        /// </summary>
        [Tooltip("����� ����� ��������� USP ��� ��������� ������ �� ������.")]
        [Foldout("Holding Touch Settings")] public float timeBetweenUSPCreationWhileHolding;

        /// <summary>
        /// ������ ����������, ������ ������� ��������� ������� ������������ ���������� ��� ������ USP ��� ������ ������� �� �����.
        /// </summary>
        [Tooltip("������ ����������, ������ ������� ��������� ������� ������������ ���������� ��� ������ USP ��� ������ ������� �� �����.")]
        [Foldout("Holding Touch Settings")] public float holdingTouchCreationRadius;





        [Space]
        [Header("��������� ���������� �������� �������.")]


        #region visualEffectController


        /// <summary>
        /// ����� ���������� ��� ����������� �����.
        /// </summary>
        [Tooltip("����� ���������� ��� ����������� �����.")]
        [Foldout("Visual Effects Controller Settings")] public float timeToBecameVisible;

        [Foldout("Visual Effects Controller Settings")] public float timeOfDamageTakenColor;
        [Foldout("Visual Effects Controller Settings")] public float timeOfUSPTakenColor;
        [Foldout("Visual Effects Controller Settings")] public float speedOfVanishing;
        /// <summary>
        /// ����� ����� ��� ������������ ����� �������� ������.
        /// </summary>
        [Tooltip("����� ����� ��� ������������ ����� �������� ������.")]
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
        [Header("��������� ����� ������������ ����������� �������.")]

        public List<UltimateChance> ultimateChances;

        #endregion

        


    }
}