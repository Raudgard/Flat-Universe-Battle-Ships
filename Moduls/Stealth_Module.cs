using UnityEngine;
using System.Collections;
using NaughtyAttributes;
using System.Linq;
using System.Collections.Generic;
using System;

namespace MODULES
{
    public class Stealth_Module : Module
    {
        private Coroutine becomeInvisibleCoroutine = null;
        private Collider2D mainCollider;
        List<Collider2D> touchedColliders = new List<Collider2D>(4);


        /// <summary>
        /// Соприкасается с вражеским коллайдером.
        /// </summary>
        private bool IsContactToEnemy { get; set; }

        /// <summary>
        /// В процессе проявления.
        /// </summary>
        private bool IsAppearing { get; set; }

        /// <summary>
        /// Происходит, когда корабль становится невидимым.
        /// </summary>
        public event Action<Ship, Stealth_Module> BecameInvisible;

        /// <summary>
        /// Происходит, когда корабль становится видимым.
        /// </summary>
        public event Action<Ship, Stealth_Module> BecameVisible;

        /// <summary>
        /// Уже в данный момент подсвечивается ультом Vision Module.
        /// </summary>
        public bool IsAlreadyHighlighting { get; set; } = false;
        
        
        private void Awake()
        {
            moduleType = Moduls.STEALTH_MODULE;
        }

        protected override void Start()
        {
            base.Start();

            if (TryGetComponent(out Attack_Module attack_Module))
            {
                attack_Module.onProjectileCreated += OnProjectileCreated;
            }

            if (TryGetComponent(out Medicus_Module medicus_Module))
            {
                medicus_Module.onProjectileCreated += OnProjectileCreated;
            }

            mainCollider = ship.mainCollider;

            if (ship.IsOriginal)
            {
                OnBecameVisibleShip();
            }
            else if (!ship.IsInvisible)
            {
                becomeInvisibleCoroutine = StartCoroutine(BecomeInvisible());
            }

            ship.shipVisualController.StealthModuleInitialize();
        }

        


        public new static readonly float[] ModuleData =
        {
            9999999f,
            7f,     6.93f,  6.86f,  6.79f,  6.73f,  6.66f,  6.59f,  6.53f,  6.46f,  6.4f,
            6.34f,  6.27f,  6.21f,  6.15f,  6.09f,  6.03f,  5.97f,  5.91f,  5.85f,  5.79f,
            5.74f,  5.68f,  5.62f,  5.57f,  5.51f,  5.46f,  5.4f,   5.35f,  5.3f,   5.25f,
            5.19f,  5.14f,  5.09f,  5.04f,  4.99f,  4.94f,  4.89f,  4.84f,  4.8f,   4.75f,
            4.7f,   4.66f,  4.61f,  4.56f,  4.52f,  4.47f,  4.43f,  4.39f,  4.34f,  4.3f,
            4.26f,  4.21f,  4.17f,  4.13f,  4.09f,  4.05f,  4.01f,  3.97f,  3.93f,  3.89f,
            3.85f,  3.81f,  3.78f,  3.74f,  3.7f,   3.67f,  3.63f,  3.59f,  3.56f,  3.52f,
            3.49f,  3.45f,  3.42f,  3.39f,  3.35f,  3.32f,  3.29f,  3.25f,  3.22f,  3.19f,
            3.16f,  3.13f,  3.1f,   3.06f,  3.03f,  3f,     2.97f,  2.95f,  2.92f,  2.89f,
            2.86f,  2.83f,  2.8f,   2.77f,  2.75f,  2.72f,  2.69f,  2.67f,  2.64f,  2.61f,
            2.54f,  2.46f,  2.39f,  2.32f,  2.25f,  2.19f,  2.13f,  2.06f,  2f,     1.94f,

        };  //в секундах

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




        private void OnProjectileCreated(ProjectileСontainer projectileСontainer, Module module)
        {
            OnDetected();
        }

        private void Update()
        {
            //Debug.Log($"mainCollider: {mainCollider}, contacts count: {mainCollider.GetContacts(touchedColliders)},  touchedColliders.Length: {touchedColliders.Count()}");
            
            if (mainCollider.GetContacts(touchedColliders) > 0 && 
                touchedColliders.Any(c => c.TryGetComponent(out Ship hittedShip) && 
                hittedShip.team != ship.team))
            {
                IsContactToEnemy = true;
                OnDetected();
            }
            else
            {
                IsContactToEnemy = false;
            }
        }




        /// <summary>
        /// Если обнаружен.
        /// </summary>
        public void OnDetected()
        {
            //Debug.Log($"ship {ship.shipName} detected!");
            ship.IsInvisible = false;
            BecameVisible?.Invoke(ship, this);
            IsAppearing = true;
            ship.shipVisualController.BecameVisible(OnBecameVisibleShip);
        }

        /// <summary>
        /// После окончания процесса становления корабля видимым.
        /// </summary>
        private void OnBecameVisibleShip()
        {
            //Debug.Log($"ship {ship.shipName} is visible!");
            IsAppearing = false;

            if (becomeInvisibleCoroutine == null)
                becomeInvisibleCoroutine = StartCoroutine(BecomeInvisible());
        }



        private IEnumerator BecomeInvisible()
        {
            while(IsContactToEnemy ||
                ship.State == Ship.States.FIGHT ||
                ship.State == Ship.States.HEALING ||
                ship.State == Ship.States.STUNNED ||
                IsAppearing)
            {
                yield return null;
            }

            float timeToBecameInvisible = UltimateImpactAction() ? 0.001f : ModuleData[levelOfModule];
            ship.shipVisualController.BecameInvisible(timeToBecameInvisible, SuccessfulInvisible);
            becomeInvisibleCoroutine = null;
        }

        /// <summary>
        /// После окончания процесса становления корабля невидимым.
        /// </summary>
        private void SuccessfulInvisible()
        {
            //Debug.Log($"ship {ship.shipName} is invisible!");
            ship.IsInvisible = true;
            BecameInvisible?.Invoke(ship, this);
        }







        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, ship.mainCollider.radius * transform.localScale.x);

        }

    }
}
