//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace MODULES
//{
//    public class Phantom : MonoBehaviour
//    {
//        private float timeOfLife;
//        public int numberOfHitsForDeath;
//        public int numberOfHitsTaked = 0;
//        //private Battle_Scene_Controller battle_Scene_Controller;
//        //private Player_Data player_Data;
//        private Global_Controller global_Controller;
//        private Ship ship;
//        private SpriteRenderer[] spriteRenderers;
//        //private SpriteRenderer[] barsRenderers;
//        private Color[] endColorsForSprites;


//        private void Awake()
//        {
//            global_Controller = Global_Controller.Instance;
//            ship = GetComponent<Ship>();
//            Destroy(GetComponent<Taking_Damage>());
//            ship.taking_Damage_component = gameObject.AddComponent<Taking_Damage_for_Phantom>();
//            if (TryGetComponent(out ShipVisualController visualEffectsController)) Destroy(visualEffectsController);
//            GetComponent<SearchingForUSP>().enabled = false;

//            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
//            //barsRenderers = GetComponentsInChildren<CanvasRenderer>();
//            endColorsForSprites = new Color[spriteRenderers.Length];
//            for(int i = 0; i < endColorsForSprites.Length; i++)
//            {
//                endColorsForSprites[i] = new Color(spriteRenderers[i].color.r, spriteRenderers[i].color.g, spriteRenderers[i].color.b, 0.5f);
//            }
//        }

//        private void OnEnable()
//        {
//            for (int i = 0; i < ship.Modules.Length; i++)
//            {
//                Destroy(ship.Modules[i]);
//                //ship.Modules[i] = null;
//            }
//            ship.Modules = new Module[0];
//        }

//        private void Start()
//        {
//            Phantom_Module phantom_Module = GetComponent<Phantom_Module>();
//            timeOfLife = phantom_Module.timeOfLife;
//            //numberOfHitsForDeath = phantom_Module.numberOfHitsForDeath;
//            Destroy(phantom_Module);

//            //ship.healthBar.fillAmount = (float)(numberOfHitsForDeath - numberOfHitsTaked) / numberOfHitsForDeath;
//            ship.HealthCurrent = ship.healthMax;

//            StartCoroutine(Appearance());
//            StartCoroutine(SelfDistructIn());

//        }


//        private IEnumerator Appearance()
//        {
//            //var endColorForCanvas = new Color(1, 1, 1, 0.5f);
//            var colorTransparent = new Color(1, 1, 1, 0);
//            var fixupdatetime = new WaitForFixedUpdate();
//            float t = 0;

//            while (t < 1)
//            {
//                //var colorForCanvas = Color.Lerp(colorTransparent, endColorForCanvas, t);
//                for(int i = 0; i < endColorsForSprites.Length; i++)
//                {
//                    spriteRenderers[i].color = Color.Lerp(colorTransparent, endColorsForSprites[i], t);
//                }
//                //for (int i = 0; i < barsRenderers.Length; i++)
//                //{
//                //    barsRenderers[i].SetColor(colorForCanvas);
//                //}
//                t += 0.033f;
//                yield return fixupdatetime;
//            }
//        }

//        private IEnumerator SelfDistructIn()
//        {
//            yield return new WaitForSeconds(timeOfLife);

//            //var startColor = new Color(1, 1, 1, 0.5f);
//            var colorTransparent = new Color(1,1,1,0);
//            var fixupdatetime = new WaitForFixedUpdate();
//            float t = 0;

//            while (t < 1)
//            {
//                //var color = Color.Lerp(startColor, colorTransparent, t);
//                for (int i = 0; i < spriteRenderers.Length; i++)
//                {
//                    spriteRenderers[i].color = Color.Lerp(endColorsForSprites[i], colorTransparent, t);
//                }
//                //for (int i = 0; i < barsRenderers.Length; i++)
//                //{
//                //    barsRenderers[i].SetColor(color);
//                //}
//                t += 0.033f;
//                yield return fixupdatetime;
//            }


//            PhantomIsDead();
//        }


//        public void PhantomIsDead()
//        {
//            global_Controller.DeleteShip(EVENT_TYPE.SHIP_DESTROYED, ship, this);
//            Destroy(gameObject);
//        }
//    }
//}