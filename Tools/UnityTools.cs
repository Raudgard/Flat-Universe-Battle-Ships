using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Tools
{
    /// <summary>
    /// Класс с различными вспомогательными методами.
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
        /// Выполняет указанный метод с задержкой в кадрах.
        /// </summary>
        /// <param name="action">Метод для исполнения.</param>
        /// <param name="framesDelay">Число кадров задержки.</param>
        /// <param name="executer">Кто должен выполнять корутину? Назначенный executor или UnityTools.</param>
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
        /// Выполняет указанный метод с задержкой в секундах.
        /// </summary>
        /// <param name="action">Метод для исполнения.</param>
        /// <param name="secondsDelay">Секунд задержки. </param>
        /// <param name="executer">Кто должен выполнять корутину? Назначенный executor или UnityTools.</param>
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