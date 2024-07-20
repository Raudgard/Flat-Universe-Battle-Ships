//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using MODULES;

///// <summary>
///// Компонент Taking_Damage для фантомов.
///// </summary>
//public class Taking_Damage_for_Phantom : Taking_Damage
//{
//    private Ship ship;
//    private Global_Controller global_Controller;
//    private Phantom phantom_component;


//    private void Awake()
//    {
//        ship = GetComponent<Ship>();
//        global_Controller = Global_Controller.Instance;
//        phantom_component = GetComponent<Phantom>();
//        Take_Damage = TakingDamage;
//    }

//    protected override void TakingDamage(int damage, Vector2 direction, Vector3 impactPoint)
//    {
//        phantom_component.numberOfHitsTaked++;
//        global_Controller.StartCoroutine(global_Controller.VisualizationOfDamage(1, ship.healthMax, transform.position, direction, References.Instance.colors.digits_damage_color));


//        //ship.healthBar.fillAmount = (float)(phantom_component.numberOfHitsForDeath - phantom_component.numberOfHitsTaked) / phantom_component.numberOfHitsForDeath;
//        ship.HealthCurrent -= 1;

//        if (phantom_component.numberOfHitsTaked == phantom_component.numberOfHitsForDeath)
//            phantom_component.PhantomIsDead();
//    }



    

//}
