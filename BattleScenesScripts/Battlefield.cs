using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ShapeOfBattlefield
{
    Circle,
    Capsule,
    Square,
    Triangle,
    Cross,
    TwoCircle,
    Free
}

public class Battlefield
{

    private ShapeOfBattlefield ShapeOfBattlefield { get; set; }
    private float Size { get; set; }
    public float CapsuleLength { get; set; }
    public Vector3[] Points { get; private set; }
    public Vector2[] SpawnPointsFor2And4And8Players { get; private set; }
    public Vector2[] SpawnPointsFor3And6Players { get; private set; }

    private const float rootOfThreeInHalf = 0.8660254037844386f;
    private const float rootOfTwoInHalf = 0.7071067811865475f;
    private const float distanceFromBordersForSpawn = 7.0f;
    private const float widthOfBorder = 2.0f;
    private const float Zcoordinate = -1;


    public int CountOfPoints { get; set; }//количество точек окружности, координаты которых мы хотим вычислить

    /// <summary>
    /// Create battlefield with border points
    /// </summary>
    /// <param name="shapeOfBattlefield"></param>
    /// <param name="sizeOfBattlefield">Usually it's mean radius of circle</param>
    /// <param name="countOfPoints">Count of points what you need to create border. Must be a multiple of 4.</param>
    /// <param name="caplsuleLengthMultyplier">A multiplier to the size that calculates the length of the capsule</param>
    public Battlefield(ShapeOfBattlefield shapeOfBattlefield, float sizeOfBattlefield, int countOfPoints = 180, float caplsuleLengthMultyplier = 2)
    {
        ShapeOfBattlefield = shapeOfBattlefield;
        Size = sizeOfBattlefield;
        CountOfPoints = countOfPoints;
        CapsuleLength = Size * caplsuleLengthMultyplier;

        SpawnPointsFor2And4And8Players = new Vector2[8];
        SpawnPointsFor3And6Players = new Vector2[6];

        Points = CreatePoints();
    }









    private Vector3[] CreatePoints()
    {
        Vector3[] points;
        float distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis;
        switch (ShapeOfBattlefield)
        {



            case ShapeOfBattlefield.Circle:
                points = new Vector3[CountOfPoints];

                for (int i = 0; i < points.Length; i++)
                {
                    points[i] = new Vector3(Mathf.Cos(2 * Mathf.PI * i / CountOfPoints) * Size, Mathf.Sin(2 * Mathf.PI * i / CountOfPoints) * Size, Zcoordinate);
                }

                distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis = Size - distanceFromBordersForSpawn;

                SpawnPointsFor2And4And8Players[0] = new Vector2(0, -distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis);
                SpawnPointsFor2And4And8Players[1] = new Vector2(0, distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis);
                SpawnPointsFor2And4And8Players[2] = new Vector2(distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis, 0);
                SpawnPointsFor2And4And8Players[3] = new Vector2(-distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis, 0);
                SpawnPointsFor2And4And8Players[4] = new Vector2(distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis * rootOfTwoInHalf, distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis * rootOfTwoInHalf);
                SpawnPointsFor2And4And8Players[5] = new Vector2(-distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis * rootOfTwoInHalf, distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis * rootOfTwoInHalf);
                SpawnPointsFor2And4And8Players[6] = new Vector2(-distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis * rootOfTwoInHalf, -distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis * rootOfTwoInHalf);
                SpawnPointsFor2And4And8Players[7] = new Vector2(distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis * rootOfTwoInHalf, -distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis * rootOfTwoInHalf);

                SpawnPointsFor3And6Players[0] = new Vector2(0, -distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis);
                SpawnPointsFor3And6Players[1] = new Vector2(distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis * rootOfThreeInHalf, distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis * 0.5f);
                SpawnPointsFor3And6Players[2] = new Vector2(-distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis * rootOfThreeInHalf, distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis * 0.5f);
                SpawnPointsFor3And6Players[3] = new Vector2(0, distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis);
                SpawnPointsFor3And6Players[4] = new Vector2(-distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis * rootOfThreeInHalf, -distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis * 0.5f);
                SpawnPointsFor3And6Players[5] = new Vector2(distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis * rootOfThreeInHalf, -distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis * 0.5f);

                //foreach (var point in SpawnPointsFor2And4And8Players)
                //    Debug.Log(point);
                break;



            case ShapeOfBattlefield.Capsule:
                points = new Vector3[CountOfPoints + 2];

                var shiftForYcoordinate = CapsuleLength / 2;

                for (int i = 0; i < points.Length / 2; i++)
                {
                    points[i] = new Vector3(Mathf.Cos(2 * Mathf.PI * i / CountOfPoints) * Size, Mathf.Sin(2 * Mathf.PI * i / CountOfPoints ) * Size + shiftForYcoordinate, Zcoordinate); 
                }


                for(int i = points.Length / 2; i < points.Length; i++)
                {
                    points[i] = new Vector3(Mathf.Cos(2 * Mathf.PI * (i - 1) / CountOfPoints) * Size, Mathf.Sin(2 * Mathf.PI * (i - 1) / CountOfPoints) * Size - CapsuleLength + shiftForYcoordinate, Zcoordinate);
                }


                ////поднимаем всю конструкцию по Y, чтобы точка (0, 0) оказалась в центре
                //for (int i = 0; i < points.Length; i++)
                //{
                //    points[i].y += shiftForYcoordinate;
                //}


                SpawnPointsFor2And4And8Players[0] = new Vector2(0, -shiftForYcoordinate);
                SpawnPointsFor2And4And8Players[1] = new Vector2(0, shiftForYcoordinate);

                SpawnPointsFor3And6Players[0] = new Vector2(0, -shiftForYcoordinate);
                SpawnPointsFor3And6Players[1] = new Vector2(0, shiftForYcoordinate);
                SpawnPointsFor3And6Players[2] = new Vector2(0, 0);

                break;



            case ShapeOfBattlefield.Square:
                points = new Vector3[4];

                points[0] = new Vector3(Size, Size, Zcoordinate);
                points[1] = new Vector3(-Size, Size, Zcoordinate);
                points[2] = new Vector3(-Size, -Size, Zcoordinate);
                points[3] = new Vector3(Size, -Size, Zcoordinate);

                distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis = Size - distanceFromBordersForSpawn;

                SpawnPointsFor2And4And8Players[0] = new Vector2(-distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis, -distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis);
                SpawnPointsFor2And4And8Players[1] = new Vector2(distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis, distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis);
                SpawnPointsFor2And4And8Players[2] = new Vector2(-distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis, distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis);
                SpawnPointsFor2And4And8Players[3] = new Vector2(distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis, -distanceFromCenterOfCoordinatesToSpawnPointOnTheAxis);

                //float radiusOfSector = distanceFromBordersForSpawn - widthOfBorder;

                break;



            case ShapeOfBattlefield.Cross:
                points = new Vector3[CountOfPoints * 2 + 8];

                var capsuleLengthInHalf = CapsuleLength / 2;
                var countPointsForSemicircle = CountOfPoints / 2;
                var countPointsForCircle = countPointsForSemicircle * 2;

                //первая точка (верхний правый угол)
                points[0] = new Vector3(Size, Size, Zcoordinate);

                //верхний полукруг
                for (int i = 1; i < countPointsForSemicircle + 1; i++)
                {
                    points[i] = new Vector3(Mathf.Cos(2 * Mathf.PI * (i - 1) / countPointsForCircle) * Size, Mathf.Sin(2 * Mathf.PI * (i - 1) / countPointsForCircle) * Size + capsuleLengthInHalf, Zcoordinate);
                }

                //завершение верхнего полукруга
                points[countPointsForSemicircle + 1] = new Vector3(-Size, capsuleLengthInHalf, Zcoordinate);
                //верхняя левая точка
                points[countPointsForSemicircle + 2] = new Vector3(-Size, Size, Zcoordinate);
                var lastOfLeftSemicirclePoint = countPointsForSemicircle + 3 + countPointsForSemicircle;

                //левый полукруг
                for (int i = countPointsForSemicircle + 3, k = countPointsForCircle / 4; i < lastOfLeftSemicirclePoint; i++, k++)
                {
                    points[i] = new Vector3(Mathf.Cos(2 * Mathf.PI * k / countPointsForCircle) * Size - capsuleLengthInHalf, Mathf.Sin(2 * Mathf.PI * k / countPointsForCircle) * Size, Zcoordinate);
                }

                //завершение левого полукруга
                points[lastOfLeftSemicirclePoint] = new Vector3(-capsuleLengthInHalf, -Size, Zcoordinate);
                //нижняя левая точка
                points[lastOfLeftSemicirclePoint + 1] = new Vector3(-Size, -Size, Zcoordinate);
                var lastOfBottomSemicirclePoint = lastOfLeftSemicirclePoint + 2 + countPointsForSemicircle;

                //нижний полукруг
                for (int i = lastOfLeftSemicirclePoint + 2, k = countPointsForSemicircle; i < lastOfBottomSemicirclePoint; i++, k++)
                {
                    points[i] = new Vector3(Mathf.Cos(2 * Mathf.PI * k / countPointsForCircle) * Size, Mathf.Sin(2 * Mathf.PI * k / countPointsForCircle) * Size - capsuleLengthInHalf, Zcoordinate);
                }

                //завершение нижнего полукруга
                points[lastOfBottomSemicirclePoint] = new Vector3(Size, -capsuleLengthInHalf, Zcoordinate);
                //нижняя правая точка
                points[lastOfBottomSemicirclePoint + 1] = new Vector3(Size, -Size, Zcoordinate);
                var lastOfRightSemicirclePoint = lastOfBottomSemicirclePoint + 2 + countPointsForSemicircle;
                //var beginPointForRightSemicircle = countPointsForCircle * 3 / 4;


                //правый полукруг
                for (int i = lastOfBottomSemicirclePoint + 2, k = countPointsForCircle * 3 / 4; i < lastOfRightSemicirclePoint; i++, k++)
                {
                    points[i] = new Vector3(Mathf.Cos(2 * Mathf.PI * k / countPointsForCircle) * Size + capsuleLengthInHalf, Mathf.Sin(2 * Mathf.PI * k / countPointsForCircle) * Size, Zcoordinate);
                }

                //завершение правого полукруга
                points[lastOfRightSemicirclePoint] = new Vector3(capsuleLengthInHalf, Size, Zcoordinate);
                //points[points.Length - 1] = new Vector3(Size * 1.5f, Size, Zcoordinate);

                SpawnPointsFor2And4And8Players[0] = new Vector2(0, -capsuleLengthInHalf);
                SpawnPointsFor2And4And8Players[1] = new Vector2(0, capsuleLengthInHalf);
                SpawnPointsFor2And4And8Players[2] = new Vector2(-capsuleLengthInHalf, 0);
                SpawnPointsFor2And4And8Players[3] = new Vector2(capsuleLengthInHalf, 0);
                SpawnPointsFor2And4And8Players[4] = new Vector2(0, 0);


                //points = new Vector3[12];
                //points[0] = new Vector3(Size, Size, Zcoordinate);
                //points[1] = new Vector3(Size, CapsuleLength / 2, Zcoordinate);
                //points[2] = new Vector3(-Size, CapsuleLength / 2, Zcoordinate);
                //points[3] = new Vector3(-Size, Size, Zcoordinate);
                //points[4] = new Vector3(-CapsuleLength / 2, Size, Zcoordinate);
                //points[5] = new Vector3(-CapsuleLength / 2, -Size, Zcoordinate);
                //points[6] = new Vector3(-Size, -Size, Zcoordinate);
                //points[7] = new Vector3(-Size, -CapsuleLength / 2, Zcoordinate);
                //points[8] = new Vector3(Size, -CapsuleLength / 2, Zcoordinate);
                //points[9] = new Vector3(Size, -Size, Zcoordinate);
                //points[10] = new Vector3(CapsuleLength / 2, -Size, Zcoordinate);
                //points[11] = new Vector3(CapsuleLength / 2, Size, Zcoordinate);


                break;



            case ShapeOfBattlefield.Triangle:
                points = new Vector3[3];

                points[0] = new Vector3(-rootOfThreeInHalf * Size, -0.5f * Size, Zcoordinate);
                points[1] = new Vector3(rootOfThreeInHalf * Size, -0.5f * Size, Zcoordinate);
                points[2] = new Vector3(0, Size, Zcoordinate);

                var k0 = points[0].y / points[0].x;
                var k1 = points[1].y / points[1].x;
                //var k2 = float.IsInfinity(points[2].y / points[2].x) ? 0 : (points[2].y / points[2].x);

                var x0 = Mathf.Sqrt(2 * distanceFromBordersForSpawn * 2 * distanceFromBordersForSpawn / (1 + k0 * k0)) + points[0].x;
                var x1 = -Mathf.Sqrt(2 * distanceFromBordersForSpawn * 2 * distanceFromBordersForSpawn / (1 + k1 * k1)) + points[1].x;
                //var x2 = Mathf.Sqrt(2 * distanceFromBordersForSpawn * 2 * distanceFromBordersForSpawn / (1 + k2 * k2)) + points[2].x;

                var y0 = k0 * x0;
                var y1 = k1 * x1;
                //var y2 = k2 * x2;

                SpawnPointsFor3And6Players[0] = new Vector2(x0, y0);
                SpawnPointsFor3And6Players[1] = new Vector2(x1, y1);
                SpawnPointsFor3And6Players[2] = new Vector2(0, points[2].y - 2 * distanceFromBordersForSpawn);
                SpawnPointsFor3And6Players[3] = new Vector2(0, 0);


                break;


            default:
                points = new Vector3[180];
                break;
        }
        return points;

    }

}
