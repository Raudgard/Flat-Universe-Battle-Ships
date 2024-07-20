using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MODULES
{
    public class Evasion_Module : Module
    {
        private Coroutine evasionCoroutine = null;

        //private Taking_Damage taking_Damage_component;

        private void Awake()
        {
            moduleType = Moduls.EVASION_MODULE;
            ship = GetComponent<Ship>();
            //ship.takeHitComponent.IsEvaded = IsEvade;
        }

        protected override void Start()
        {
            base.Start();

            ship.takeHitComponent.IsEvaded += IsEvaded;
        }



        /// <summary>
        /// Вероятность уворота в процентах.
        /// </summary>
        public new static readonly float[] ModuleData =
        {
            0,
            7f, 7.07f,  7.14f,  7.21f,  7.28f,  7.36f,  7.43f,  7.5f,   7.58f,  7.66f,
            7.73f,  7.81f,  7.89f,  7.97f,  8.05f,  8.13f,  8.21f,  8.29f,  8.37f,  8.46f,
            8.54f,  8.63f,  8.71f,  8.8f,   8.89f,  8.98f,  9.07f,  9.16f,  9.25f,  9.34f,
            9.43f,  9.53f,  9.62f,  9.72f,  9.82f,  9.92f,  10.02f, 10.12f, 10.22f, 10.32f,
            10.42f, 10.53f, 10.63f, 10.74f, 10.85f, 10.95f, 11.06f, 11.17f, 11.29f, 11.4f,
            11.51f, 11.63f, 11.74f, 11.86f, 11.98f, 12.1f,  12.22f, 12.34f, 12.47f, 12.59f,
            12.72f, 12.84f, 12.97f, 13.1f,  13.23f, 13.37f, 13.5f,  13.63f, 13.77f, 13.91f,
            14.05f, 14.19f, 14.33f, 14.47f, 14.62f, 14.76f, 14.91f, 15.06f, 15.21f, 15.36f,
            15.52f, 15.67f, 15.83f, 15.99f, 16.15f, 16.31f, 16.47f, 16.64f, 16.8f,  16.97f,
            17.14f, 17.31f, 17.48f, 17.66f, 17.84f, 18.01f, 18.19f, 18.38f, 18.56f, 18.75f,
            19.31f, 19.89f, 20.48f, 21.1f,  21.73f, 22.38f, 23.06f, 23.75f, 24.46f, 25.19f,
        };  


        public static readonly float[] TimeOfEvasion =
        {
            0,
            0.1f,   0.102f, 0.103f, 0.105f, 0.106f, 0.108f, 0.109f, 0.111f, 0.113f, 0.114f,
            0.116f, 0.118f, 0.12f,  0.121f, 0.123f, 0.125f, 0.127f, 0.129f, 0.131f, 0.133f,
            0.135f, 0.137f, 0.139f, 0.141f, 0.143f, 0.145f, 0.147f, 0.149f, 0.152f, 0.154f,
            0.156f, 0.159f, 0.161f, 0.163f, 0.166f, 0.168f, 0.171f, 0.173f, 0.176f, 0.179f,
            0.181f, 0.184f, 0.187f, 0.19f,  0.193f, 0.195f, 0.198f, 0.201f, 0.204f, 0.207f,
            0.211f, 0.214f, 0.217f, 0.22f,  0.223f, 0.227f, 0.23f,  0.234f, 0.237f, 0.241f,
            0.244f, 0.248f, 0.252f, 0.255f, 0.259f, 0.263f, 0.267f, 0.271f, 0.275f, 0.279f,
            0.284f, 0.288f, 0.292f, 0.296f, 0.301f, 0.305f, 0.31f,  0.315f, 0.319f, 0.324f,
            0.329f, 0.334f, 0.339f, 0.344f, 0.349f, 0.354f, 0.36f,  0.365f, 0.371f, 0.376f,
            0.382f, 0.388f, 0.393f, 0.399f, 0.405f, 0.411f, 0.418f, 0.424f, 0.43f,  0.437f,
            0.45f,  0.463f, 0.477f, 0.491f, 0.506f, 0.521f, 0.537f, 0.553f, 0.57f,  0.587f
        };

        public override int LevelOfModule
        {
            get
            {
                return levelOfModule;
            }
            set
            {
                if (value < 1)
                    levelOfModule = 1;
                else if (value > ModuleData.Length - 1)
                    levelOfModule = ModuleData.Length - 1;
                else
                    levelOfModule = value;
            }
        }

        public static int GetMaxLevel() => ModuleData.Length - 1;

        /// <summary>
        /// Корабль увернулся?
        /// </summary>
        /// <returns></returns>
        private bool IsEvaded()
        {
            if (ship.State != Ship.States.REPRODUCTION &&
                ship.State != Ship.States.STUNNED &&
                GameEngineAssistant.GetProbability(ModuleData[LevelOfModule]) &&
                evasionCoroutine == null)
            {
                //Debug.Log($"Evasion Module. TakeHitComponent  return true");
                StartCoroutine(GoToThirdDimention());
                return true;
            }

            //Debug.Log($"Evasion Module. TakeHitComponent  return false");
            return false;
        }


        /// <summary>
        /// Сработал уворот?
        /// </summary>
        /// <returns></returns>
        private bool IsEvade()
        {
            if (ship.State != Ship.States.REPRODUCTION &&
                ship.State != Ship.States.STUNNED &&
                GameEngineAssistant.GetProbability(ModuleData[LevelOfModule]) &&
                evasionCoroutine == null)
            {
                StartCoroutine(GoToThirdDimention());
                return true;
            }

            //ship.takeHitComponent.Take_Damage(damage, direction,  impactPoint, action);
            return false;

        }


        /// <summary>
        /// Уходит в 3 измерение. На практике отключаем коллайдер, чтобы увернуться от пули и включаем полупрозрачность.
        /// </summary>
        /// <returns></returns>
        private IEnumerator GoToThirdDimention()
        {
            ship.mainCollider.enabled = false;
            ship.InThirdDimention = true;
            ship.shipVisualController.EvasionStart();

            float timeOfEvation = UltimateImpactAction() ? TimeOfEvasion[LevelOfModule] * 2 : TimeOfEvasion[LevelOfModule];

            yield return new WaitForSeconds(timeOfEvation);

            ship.shipVisualController.EvasionEnd();
            ship.mainCollider.enabled = true;
            ship.InThirdDimention = false;
            evasionCoroutine = null;
        }
    }
}