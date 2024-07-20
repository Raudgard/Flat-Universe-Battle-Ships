using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Rotation_Controller : MonoBehaviour
{
    [SerializeField] private Ship ship;
    [SerializeField] private Transform bars;
    public Coroutine rotatingCoroutine;
    public float rotationSpeed;

    /// <summary>
    /// Значение вращения по оси Z в данный момент. Равно 0, пока корабль не вращается.
    /// </summary>
    public float currentRotationZValue = 0;

    /// <summary>
    /// Поворачивает корабль носом в сторону движения при этом оставляя полосу ХП строго сверху
    /// </summary>
    /// <param name="vectorTo"></param>
    public void Rotate(Vector2 vectorTo)
    {
        var targetAngle = Vector2.SignedAngle(Vector2.up, vectorTo);

        var difference = transform.eulerAngles.z - targetAngle;
        //print("difference = " + difference);
        if (difference > 360) difference -= 360;
        else if (difference > 180) difference -= 360;
        bool rotateOnRight = difference < 0;
        //if (difference < 0) rotateOnRight = true;
        //else rotateOnRight = false;
        //print("current angle = " + transform.eulerAngles.z + "   targetAngle = " + targetAngle + "  difference = " + difference);

        if (rotatingCoroutine != null)
        {
            StopCoroutine(rotatingCoroutine);
        }

        rotatingCoroutine = StartCoroutine(Rotating(difference, targetAngle, rotateOnRight));

    }

    private IEnumerator Rotating(float angle, float targetAngle, bool rotateOnRight)
    {
        var counter = (int)Mathf.Abs(angle) / rotationSpeed;
        //print("counter = " + counter);

        var speed = rotateOnRight ? rotationSpeed : -rotationSpeed;
        for(int i = 0; i < counter; i++)
        {
            ship.transform.Rotate(Vector3.forward, speed);
            bars/*.transform*/.Rotate(Vector3.forward, -speed);
            currentRotationZValue = speed;
            yield return new WaitForFixedUpdate();
        }

        transform.eulerAngles = new Vector3(0, 0, targetAngle);
        bars/*.transform*/.localEulerAngles = new Vector3(0, 0, -targetAngle);

        currentRotationZValue = 0;
        rotatingCoroutine = null;
    }

}
