using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MODULES
{
    public class Evasion_trigger_controller : MonoBehaviour
    {
        private float additiveSizeToColliderExtent = 0.5f;
        //public CircleCollider2D mainCollider;
        public CircleCollider2D triggerCollider;
        private Evasion_Module evasion_Module;
        public Ship ship;
        //private VisualEffectsController visualEffectsController;

        private Coroutine evasionCoroutine = null;
        //private Collider2D projectile = null;

        private void Start()
        {
            evasion_Module = GetComponentInParent<Evasion_Module>();
            //visualEffectsController = GetComponentInParent<VisualEffectsController>();
            EventManager.Instance.AddListener(EVENT_TYPE.SHIPS_SIZE_CHANGED, ShipChangeSize);

            if (TryGetComponent(out CircleCollider2D _))
            {
                CalculateRadiusForTriggerCollider();
                return;
            }

            triggerCollider = gameObject.AddComponent<CircleCollider2D>();
            triggerCollider.isTrigger = true;
            CalculateRadiusForTriggerCollider();
        }



        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Projectile projectile) &&
                projectile.shipWhoFired != ship &&
                ship.State != Ship.States.REPRODUCTION &&
                ship.State != Ship.States.STUNNED)
            {
                if (GameEngineAssistant.GetProbability(Evasion_Module.ModuleData[evasion_Module.LevelOfModule]))
                {
                    //this.projectile = collision;
                    if (evasionCoroutine == null) StartCoroutine(GoToThirdDimention());
                }
            }
        }


        /// <summary>
        /// Уходит в 3 измерение. На практике отключаем коллайдер, чтобы увернуться от пули и включаем полупрозрачность.
        /// </summary>
        /// <returns></returns>
        private IEnumerator GoToThirdDimention()
        {
            ship.mainCollider.enabled = false;
            //canReturn = false;
            //var fixupdatetime = new WaitForFixedUpdate();
            ship.shipVisualController.EvasionStart();

            //while (!canReturn) yield return fixupdatetime;

            yield return new WaitForSeconds(Evasion_Module.TimeOfEvasion[evasion_Module.LevelOfModule]);

            ship.shipVisualController.EvasionEnd();
            ship.mainCollider.enabled = true;
            evasionCoroutine = null;
        }


        /// <summary>
        /// Обрабатывает событие изменения размеров корабля (например, от Giant_Module)
        /// </summary>
        /// <param name="eVENT_TYPE"></param>
        /// <param name="component"></param>
        /// <param name="obj"></param>
        private void ShipChangeSize(EVENT_TYPE eVENT_TYPE, Component component, object obj)
        {
            if (component.gameObject == ship.gameObject)
            {
                CalculateRadiusForTriggerCollider();
            }
        }

        /// <summary>
        /// Изменяет радиус коллайдера таким образом, чтобы его размер был больше размера mainCollider на постоянную величину.
        /// </summary>
        private void CalculateRadiusForTriggerCollider()
        {
            triggerCollider.radius = ship.mainCollider.radius + transform.localScale.x / ship.transform.localScale.x * additiveSizeToColliderExtent;
        }
    }
}