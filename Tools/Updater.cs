using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace Tools
{
    /// <summary>
    /// ������, ����������� Update � FixUpdate ��� ���� ������������������ ��������.
    /// ���������������� ������ ���������� ��� �������� ������� � RegisterNeedUpdateObject, � ��� ����������� � UnregisterNeedUpdateObject.
    /// </summary>
    public class Updater : MonoBehaviour, ISerializationCallbackReceiver
    {
        #region Singleton
        private static Updater instance;
        public static Updater Instance => instance;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            if (instance == null)
                instance = this;
        }

        #endregion

        private List<INeedUpdate> needUpdateList = new List<INeedUpdate>();
        private List<INeedFixUpdate> needFixUpdateList = new List<INeedFixUpdate>();

        private void Awake()
        {
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }


        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            //Debug.Log("SceneManager_activeSceneChanged");
            needUpdateList.Clear();
            needFixUpdateList.Clear();
        }


        void Update()
        {
            for (int i = 0; i < needUpdateList.Count; i++)
            {
                needUpdateList[i].UpdateMe();
            }

            //if (Time.frameCount % 30 == 0)
            //{
            //    Debug.Log($"Total memory before GC: {GC.GetTotalMemory(false):N0}");
            //    GC.Collect(0, GCCollectionMode.Optimized, false, false);
            //    Debug.Log($"Total memory after GC: {GC.GetTotalMemory(true):N0}");
            //}
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < needFixUpdateList.Count; i++)
            {
                needFixUpdateList[i].FixUpdateMe();
            }
        }

        /// <summary>
        /// ������������ ������ � ������ ����������� ��� � ����. ��������� ���� ����� ��� �������� ������� (��������, � Awake() � Monobehaviour).
        /// </summary>
        /// <param name="needUpdateObject"></param>
        public void RegisterNeedUpdateObject(INeedUpdate needUpdateObject)
        {
            if (!needUpdateList.Contains(needUpdateObject))
            {
                needUpdateList.Add(needUpdateObject);
            }
        }

        /// <summary>
        /// ������������ ������ � ������ ����������� ��� � Fixed timestep. ��������� ���� ����� ��� �������� ������� (��������, � Awake() � Monobehaviour).
        /// </summary>
        /// <param name="needFixUpdateObject"></param>
        public void RegisterNeedUpdateObject(INeedFixUpdate needFixUpdateObject)
        {
            if (!needFixUpdateList.Contains(needFixUpdateObject))
            {
                needFixUpdateList.Add(needFixUpdateObject);
            }
        }

        /// <summary>
        /// ������� ������ �� ������ �����������. ���������� ����������� ��������� ���� ����� ��� ����������� �������.
        /// </summary>
        /// <param name="needUpdateObject"></param>
        public void UnregisterNeedUpdateObject(INeedUpdate needUpdateObject)
        {
            if (needUpdateList.Contains(needUpdateObject))
            {
                needUpdateList.Remove(needUpdateObject);
            }
        }

        /// <summary>
        /// ������� ������ �� ������ �����������. ���������� ����������� ��������� ���� ����� ��� ����������� �������.
        /// </summary>
        /// <param name="needFixUpdateObject"></param>
        public void UnregisterNeedUpdateObject(INeedFixUpdate needFixUpdateObject)
        {
            if (needFixUpdateList.Contains(needFixUpdateObject))
            {
                needFixUpdateList.Remove(needFixUpdateObject);
            }
        }


    }
}