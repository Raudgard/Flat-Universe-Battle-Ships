using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Tools
{
    /// <summary>
    /// ����� � ���������� ���������������� ��������.
    /// </summary>
    public class UnityTools : MonoBehaviour
    {
        #region Singleton
        private static UnityTools instance;
        //public static UnityTools Instance
        //{
        //    get { return instance; }
        //}
        #endregion

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }


        /// <summary>
        /// ��������� ��������� ����� � ��������� � ������.
        /// </summary>
        /// <param name="action">����� ��� ����������.</param>
        /// <param name="framesDelay">����� ������ ��������.</param>
        /// <param name="executer">��� ������ ��������� ��������? ����������� executor ��� UnityTools.</param>
        public static void ExecuteWithDelay(Action action, int framesDelay, MonoBehaviour executer = null)
        {
            if (executer != null)
            {
                executer.StartCoroutine(instance.Executing(action, framesDelay));
            }
            else
            {
                instance.StartCoroutine(instance.Executing(action, framesDelay));
            }
        }

        /// <summary>
        /// ��������� ��������� ����� � ��������� � ��������.
        /// </summary>
        /// <param name="action">����� ��� ����������.</param>
        /// <param name="secondsDelay">������ ��������. </param>
        /// <param name="executer">��� ������ ��������� ��������? ����������� executor ��� UnityTools.</param>
        public static void ExecuteWithDelay(Action action, float secondsDelay, MonoBehaviour executer = null)
        {
            if (executer != null)
            {
                executer.StartCoroutine(instance.Executing(action, secondsDelay));
            }
            else
            {
                instance.StartCoroutine(instance.Executing(action, secondsDelay));
            }
        }


        private IEnumerator Executing(Action action, int framesDelay)
        {
            for (int i = 0; i < framesDelay; i++)
            {
                yield return null;
            }
            action?.Invoke();
        }

        private IEnumerator Executing(Action action, float secondsDelay)
        {
            yield return new WaitForSeconds(secondsDelay);
            action?.Invoke();
        }



    }
}