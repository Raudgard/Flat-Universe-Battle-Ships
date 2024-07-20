using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Tools
{
    /// <summary>
    /// Delegates iteration to search positive answer.
    /// Класс для последовательного исполнения добавленных делегатов с одинаковой сигнатурой и возвратом bool. Если один из делегатов вернул true,
    /// то результатом Invoke будет true и дальнейшие делегаты НЕ ВЫЗЫВАЮТСЯ, иначе false.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class DelegiterPositive<T1>
    {
        public List<Func<T1, bool>> Delegates { get; private set; } = new List<Func<T1, bool>>();

        public void AddListener(Func<T1, bool> func) => Delegates.Add(func);
        public void RemoveListener(Func<T1, bool> func)
        {
            if (Delegates.Contains(func))
                Delegates.Remove(func);
        }

        public bool Invoke(T1 t1)
        {
            for (int i = 0; i < Delegates.Count; i++)
            {
                if (Delegates[i](t1))
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Delegates iteration to search positive answer.
    /// Класс для последовательного исполнения добавленных делегатов с одинаковой сигнатурой и возвратом bool. Если один из делегатов вернул true,
    /// то результатом Invoke будет true и дальнейшие делегаты НЕ ВЫЗЫВАЮТСЯ, иначе false.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class DelegiterPositive<T1, T2>
    {
        public List<Func<T1, T2, bool>> Delegates { get; private set; } = new List<Func<T1, T2, bool>>();

        public void AddListener(Func<T1, T2, bool> func) => Delegates.Add(func);
        public void RemoveListener(Func<T1, T2, bool> func)
        {
            if (Delegates.Contains(func))
                Delegates.Remove(func);
        }

        public bool Invoke(T1 t1, T2 t2)
        {
            for (int i = 0; i < Delegates.Count; i++)
            {
                if (Delegates[i](t1, t2))
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Delegates iteration to search positive answer.
    /// Класс для последовательного исполнения добавленных делегатов с одинаковой сигнатурой и возвратом bool. Если один из делегатов вернул true,
    /// то результатом Invoke будет true и дальнейшие делегаты НЕ ВЫЗЫВАЮТСЯ, иначе false.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public class DelegiterPositive<T1, T2, T3>
    {
        public List<Func<T1, T2, T3, bool>> Delegates { get; private set; } = new List<Func<T1, T2, T3, bool>>();

        public void AddListener(Func<T1, T2, T3, bool> func) => Delegates.Add(func);
        public void RemoveListener(Func<T1, T2, T3, bool> func)
        {
            if (Delegates.Contains(func))
                Delegates.Remove(func);
        }

        public bool Invoke(T1 t1, T2 t2, T3 t3)
        {
            for (int i = 0; i < Delegates.Count; i++)
            {
                if (Delegates[i](t1, t2, t3))
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Delegates iteration to search positive answer.
    /// Класс для последовательного исполнения добавленных делегатов с одинаковой сигнатурой и возвратом bool. Если один из делегатов вернул true,
    /// то результатом Invoke будет true и дальнейшие делегаты НЕ ВЫЗЫВАЮТСЯ, иначе false.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    public class DelegiterPositive<T1, T2, T3, T4>
    {
        public List<Func<T1, T2, T3, T4, bool>> Delegates { get; private set; } = new List<Func<T1, T2, T3, T4, bool>>();

        public void AddListener(Func<T1, T2, T3, T4, bool> func) => Delegates.Add(func);
        public void RemoveListener(Func<T1, T2, T3, T4, bool> func)
        {
            if (Delegates.Contains(func))
                Delegates.Remove(func);
        }

        public bool Invoke(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            for (int i = 0; i < Delegates.Count; i++)
            {
                if (Delegates[i](t1, t2, t3, t4))
                    return true;
            }

            return false;
        }
    }

}