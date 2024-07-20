using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avoiding_Obstacle : MonoBehaviour
{
    private Ship ship;
    public Coroutine avoidShipCoroutine;
    private LayerMask shipLayer;
    private Rotation_Controller rotation_Control;
    private WaitForFixedUpdate fixUpdate;

    public float secondsToAvoiding;
    public int attempsToSameTarget = 0;
    public float coeffToSizeOfShipForObstacle;
    private int mainBypassDirection;
    private int altBypassDirection;


    private const float sin30 = 0.5f, sin60 = 0.8660254038f, sin90 = 1, sin120 = 0.8660254038f, sin150 = 0.5f;
    private const float cos30 = 0.8660254038f, cos60 = 0.5f, cos90 = 0, cos120 = -0.5f, cos150 = -0.8660254038f;
    private void Awake()
    {
        ship = GetComponent<Ship>();
        shipLayer = LayerMask.GetMask("ShipLayer");
        rotation_Control = GetComponent<Rotation_Controller>();
        fixUpdate = new WaitForFixedUpdate();
        mainBypassDirection = Random.Range(0, 2) == 0 ? -1 : 1;
        altBypassDirection = -mainBypassDirection;
    }


    public IEnumerator AvoidObstacle(GameObject target)
    {
        if (target == null)
            yield break;

        Vector2 vectorToTarget = target.transform.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, vectorToTarget, Mathf.Infinity, shipLayer, - 3, 0);
        //print("hit = " + hit.transform.name + "hit.distance = "+hit.distance);
        if(hit.transform == null || !hit.transform.TryGetComponent(out Ship _shipObstacle))
        {
            //print("на пути не микрот");
            yield break;
        }

        Vector2 vectorFromObstacle = transform.position - _shipObstacle.transform.position;//находим вектор ОТ препятствия к микроту
        //Debug.DrawLine(_shipObstacle.transform.position, transform.position, Color.white, 1);

        //Если расстояние до препятствия больше, чем некоторый размер, кратный размеру корабля, то сначала направляем корабль прямо к ЦЕЛИ. (отменено пока)
        //Практика показала, что корабли частенько двигаются вперед, несмотря на то, что они уже достают до цели ИЛИ у них есть цель поближе.
        //Из-за этого их действия кажутся слишком глупыми. Возможно дело в неправильной настройке коэффициента или логики.
        //Но если полностью убрать эту механику, то становится лучше, Хотя иногда они, конечно, начинают обходной маневр слишком рано,
        //когда до цели еще слишком далеко и нужно было бы просто подойти для начала.

        //float twoSizeOfship = ship.radiusSize * coeffToSizeOfShipForObstacle;
        //if(vectorFromObstacle.sqrMagnitude > twoSizeOfship * twoSizeOfship)
        //{
        //    ship.moveDirection = vectorToTarget.normalized;
        //    rotation_Control.Rotate(vectorToTarget);

        //    //print($"{gameObject.name} say: Too far to obstacle. Moving straight to target!");

        //    yield return new WaitForSeconds(secondsToAvoiding);

        //    avoidShipCoroutine = null;
        //    yield break;
        //}

        vectorFromObstacle = vectorFromObstacle.normalized;//нормализуем его
        Vector2 rightPerpendicularVector = new Vector2(-vectorFromObstacle.y, vectorFromObstacle.x);//вектор перпендикулярный направо, если смотреть от микрота к врагу
        Vector2 leftPerpendicularVector = new Vector2(vectorFromObstacle.y, -vectorFromObstacle.x);//вектор перпендикулярный налево, если смотреть от микрота к врагу

        float distanceToObstacle = (hit.distance - ship.radiusSize) <= 0 ? 0.01f : hit.distance; //измеряем длину луча до препятствия, вычитая радиус микрота. Если получилось отрицательно значение, значит препятствие находится "в упор" и считаем расстоянием до него 0,01.
        //print("distanceToObstacle = " + distanceToObstacle);

        float coeffDependDistance = ship.radiusSize * 2 * 5 / distanceToObstacle; //коэффициент удаления по перпендикуляру, основанный на расстоянии до препятсвия и 5-и размерах микрота.
        //print("coeffDependDistance = " + coeffDependDistance);

        float radiusSize = 0; 
        if(target.TryGetComponent(out Ship _targetShip))
        {
            radiusSize = _targetShip.radiusSize;
        }
        else
        {
            //здесь будет размер другого объекта, в случае если обходим не корабль
        }


        float distance = (ship.radiusSize + radiusSize + ship.attack_range) * coeffDependDistance; //то самое расстояние вдоль перпендикуляра
        //print("distance = " + distance);

        Vector2 destination;

        //сравниваем в какую сторону (направо или налево) быстрее обход, т.е. короче путь
        int way = ComparisonBypassWays(vectorToTarget, target.transform.position);
        if (way > 0 )
        {
            destination = rightPerpendicularVector * distance; // перпендикуляр, увеличенный в зависимости от расстояния до препятствия.
        }
        else if (way < 0)
        {
            destination = leftPerpendicularVector * distance; // перпендикуляр, увеличенный в зависимости от расстояния до препятствия.
        }
        else if(attempsToSameTarget < 5) //до 5 раз пробуем найти новую цель и новые пути обхода
        {
            yield return fixUpdate;
            yield return fixUpdate;
            yield return fixUpdate;

            attempsToSameTarget++;
            avoidShipCoroutine = null;
            yield break;
        }
        else
        {
            destination = rightPerpendicularVector * distance; // если все-таки не находим новую цель, то обходим справа.
        }

        Vector2 path = -vectorFromObstacle + destination; // путь обхода препятствия.

        var vectorDirection = path.normalized;
        ship.moveDirection = vectorDirection;
        rotation_Control.Rotate(vectorDirection);

        //print($"{gameObject.name} say: going perpendicular");

        yield return new WaitForSeconds(secondsToAvoiding);

        //print("obstacle Coroutine is end");
        attempsToSameTarget = 0;
        avoidShipCoroutine = null;

    }

    ///<summary>
    ///Метод сравнения обходных путей. Сравнивает обход направо и налево. Возвращает 1, если быстрее направо, 2 - если налево, 0 - если путь обхода не найден.
    ///</summary>
    ///<param name="vectorToTarget">Вектор от микрота к цели</param>
    private int ComparisonBypassWays(Vector2 vectorToTarget, Vector2 targetPosition)
    {
        //Создаем 10 проверочных точек по окружности с центром в позиции врага и радиусом расстояния до врага.
        //Позиции откладываюся по 30 градусов. Сначала проверяется справа, потом слева, опять справа, слева и т.д.

        //float distanceToEnemy = vectorToEnemy.magnitude;
        //Vector2 targetPosition = ship.enemyCurrent.transform.position;
        Vector2 _vectorFromEnemy = -vectorToTarget;

        Vector2 vectorFromEnemyToNewPoint = OffsetByAngle(_vectorFromEnemy, 30 * mainBypassDirection);
        Vector2 positionForCheck = targetPosition + vectorFromEnemyToNewPoint;
        if(CastRayToEnemyFromCheckPosition(positionForCheck, targetPosition))
        {
            return mainBypassDirection;
        }

        vectorFromEnemyToNewPoint = OffsetByAngle(_vectorFromEnemy, 30 * altBypassDirection);
        positionForCheck = targetPosition + vectorFromEnemyToNewPoint;
        if (CastRayToEnemyFromCheckPosition(positionForCheck, targetPosition))
        {
            return altBypassDirection;
        }

        vectorFromEnemyToNewPoint = OffsetByAngle(_vectorFromEnemy, 60 * mainBypassDirection);
        positionForCheck = targetPosition + vectorFromEnemyToNewPoint; 
        if (CastRayToEnemyFromCheckPosition(positionForCheck, targetPosition))
        {
            return mainBypassDirection;
        }

        vectorFromEnemyToNewPoint = OffsetByAngle(_vectorFromEnemy, 60 * altBypassDirection);
        positionForCheck = targetPosition + vectorFromEnemyToNewPoint;
        if (CastRayToEnemyFromCheckPosition(positionForCheck, targetPosition))
        {
            return altBypassDirection;
        }

        vectorFromEnemyToNewPoint = OffsetByAngle(_vectorFromEnemy, 90 * mainBypassDirection);
        positionForCheck = targetPosition + vectorFromEnemyToNewPoint;
        if (CastRayToEnemyFromCheckPosition(positionForCheck, targetPosition))
        {
            return mainBypassDirection;
        }

        vectorFromEnemyToNewPoint = OffsetByAngle(_vectorFromEnemy, 90 * altBypassDirection);
        positionForCheck = targetPosition + vectorFromEnemyToNewPoint;
        if (CastRayToEnemyFromCheckPosition(positionForCheck, targetPosition))
        {
            return altBypassDirection;
        }

        vectorFromEnemyToNewPoint = OffsetByAngle(_vectorFromEnemy, 120 * mainBypassDirection);
        positionForCheck = targetPosition + vectorFromEnemyToNewPoint;
        if (CastRayToEnemyFromCheckPosition(positionForCheck, targetPosition))
        {
            return mainBypassDirection;
        }

        vectorFromEnemyToNewPoint = OffsetByAngle(_vectorFromEnemy, 120 * altBypassDirection);
        positionForCheck = targetPosition + vectorFromEnemyToNewPoint;
        if (CastRayToEnemyFromCheckPosition(positionForCheck, targetPosition))
        {
            return altBypassDirection;
        }

        vectorFromEnemyToNewPoint = OffsetByAngle(_vectorFromEnemy, 150 * mainBypassDirection);
        positionForCheck = targetPosition + vectorFromEnemyToNewPoint;
        if (CastRayToEnemyFromCheckPosition(positionForCheck, targetPosition))
        {
            return mainBypassDirection;
        }

        vectorFromEnemyToNewPoint = OffsetByAngle(_vectorFromEnemy, 150 * altBypassDirection);
        positionForCheck = targetPosition + vectorFromEnemyToNewPoint;
        if (CastRayToEnemyFromCheckPosition(positionForCheck, targetPosition))
        {
            return altBypassDirection;
        }

        //Vector2 shiftVector = vectorToEnemy + vectorFromEnemyToNewPoint;

        //Если со всех десяти проверочных позиций нет прямой видимости с врагом, то возвращаем 0 и будет какое-то время искать других врагов.
        return 0;
    }


    ///<summary>
    ///Возвращает вектор той же длины, исходящий из той же точки, но сдвинутый на указанный угол
    ///</summary>
    ///<param name="vector">Исходный вектор</param>
    ///<param name="angle">Угол сдвига</param>
    private Vector2 OffsetByAngle(Vector2 vector, int angle)
    {
        switch(angle)
        {
            case 30:
                return new Vector2(vector.x * cos30 - vector.y * sin30, vector.x * sin30 + vector.y * cos30);
            case -30:
                return new Vector2(vector.x * cos30 - vector.y * (-sin30), vector.x * (-sin30) + vector.y * cos30);
            case 60:
                return new Vector2(vector.x * cos60 - vector.y * sin60, vector.x * sin60 + vector.y * cos60);
            case -60:
                return new Vector2(vector.x * cos60 - vector.y * (-sin60), vector.x * (-sin60) + vector.y * cos60);
            case 90:
                return new Vector2(vector.x * cos90 - vector.y * sin90, vector.x * sin90 + vector.y * cos90);
            case -90:
                return new Vector2(vector.x * cos90 - vector.y * (-sin90), vector.x * (-sin90) + vector.y * cos90);
            case 120:
                return new Vector2(vector.x * cos120 - vector.y * sin120, vector.x * sin120 + vector.y * cos120);
            case -120:
                return new Vector2(vector.x * cos120 - vector.y * (-sin120), vector.x * (-sin120) + vector.y * cos120);
            case 150:
                return new Vector2(vector.x * cos150 - vector.y * sin150, vector.x * sin150 + vector.y * cos150);
            case -150:
                return new Vector2(vector.x * cos150 - vector.y * (-sin150), vector.x * (-sin150) + vector.y * cos150);

            default:
                return Vector2.zero;
        }
    }

    private bool CastRayToEnemyFromCheckPosition(Vector2 checkPosition, Vector2 enemyPosition)
    {
        Vector2 direction = enemyPosition - checkPosition;
        //RaycastHit2D hit = Physics2D.CircleCast(checkPosition, 0.1f, direction, Mathf.Infinity, shipLayer);       // Очень нагружает процессор!
        //RaycastHit2D hit = Physics2D.Linecast(checkPosition, enemyPosition, shipLayer);                           // Не определяет объекты с отключенным коллайдером!!!
        RaycastHit2D hit = Physics2D.Raycast(checkPosition, direction, Mathf.Infinity, shipLayer, -3, 0);           // от -3 до 0, потому что все корабли должны находится на "глубине" -1 (координата Z),
                                                                                                                    // а ClickController находится на "глубине" 5. Иначе определяет все равно его, не смотря на маску. 
        Debug.DrawLine(checkPosition, hit.point, Color.cyan, 1);

        if (hit.transform == null)
        {
            print(ship.gameObject.name + ": " + "hit.transform = null");
            return false;
        }

        if (hit.transform.TryGetComponent(out Ship _ship) /*&& ship.enemyCurrent != null*/ /*&& _ship != null*/ &&  _ship == ship.enemyCurrent)
        {
            //print("ray hit enemy");
            return true;
        }

        //print("ray hit NOT enemy");
        return false;
    }

}
