using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround_Lights : MonoBehaviour
{
    public Vector3 _scale;
    //public Vector3 currentPosition;
    public float speed, speedMin, speedMax;
    public Color color;
    public Vector2 moveDirection;

    public float xOffsetMin, xOffsetMax;
    public float yOffsetMin, yOffsetMax;

    public float _colorChangeSpeed;
    public float _colorOffset;
    public float alphaChangingSpeed;
    public float alphaMax;
    public float alphaMin;

    public BattleSceneController battle_Scene_Controller;
    
    void Start()
    {
        battle_Scene_Controller = BattleSceneController.Instance;
        if(battle_Scene_Controller != null)
        {
            //transform.position = new Vector3(Random.Range(battle_Scene_Controller.leftBorderOfBattleField - 2, battle_Scene_Controller.rightBorderOfBattleField + 2),
            //                                 Random.Range(battle_Scene_Controller.bottomBorderOfBattleField - 2, battle_Scene_Controller.topBorderOfBattleField + 2),
            //                                 19);
            transform.position = new Vector3(Random.Range(-15, 15), Random.Range(-15, 15), 19);
        }
        else
        {
            transform.position = new Vector3(Random.Range(-7, 7), Random.Range(-11, 11), 19);
        }

        transform.localScale = _scale = new Vector3(Random.Range(4f, 5f), Random.Range(4f, 5f), 1);

        //H - цвет (от 0 до 1), S - насыщенность, V - яркость, А - прозрачность (альфа-канал)
        color = GetComponent<SpriteRenderer>().color = Random.ColorHSV(0, 1, 1, 1, 1, 1, 0, 0);

        if (battle_Scene_Controller != null)
            StartCoroutine(CheckPosition(true));
        else
            StartCoroutine(CheckPosition(false));

        StartCoroutine(ChangingColor());
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    private IEnumerator CheckPosition(bool _battle_scene)
    {
        float X = 7, Y = 11;

        if (_battle_scene)
        {
            while (true)
            {
                if (transform.position.x > 15)
                {
                    X = Random.Range(-xOffsetMin, -xOffsetMax);
                }
                else if (transform.position.x < -15)
                {
                    X = Random.Range(xOffsetMin, xOffsetMax);
                }

                if (transform.position.y > 15)
                {
                    Y = Random.Range(-yOffsetMin, -yOffsetMax);
                }
                else if (transform.position.y < -15)
                {
                    Y = Random.Range(yOffsetMin, yOffsetMax);
                }

                //print("X = " + X + "   Y = " + Y);
                moveDirection = new Vector2(X + Random.Range(0, 2), Y + Random.Range(0, 2));
                moveDirection = moveDirection.normalized;

                speed = Random.Range(speedMin, speedMax);

                yield return new WaitForSeconds(2);
            }
        }
        else
        {
            while (true)
            {
                if (transform.position.x > 13)
                {
                    X = Random.Range(-xOffsetMin, -xOffsetMax);
                }
                else if (transform.position.x < -13)
                {
                    X = Random.Range(xOffsetMin, xOffsetMax);
                }

                if (transform.position.y > 17)
                {
                    Y = Random.Range(-yOffsetMin, -yOffsetMax);
                }
                else if (transform.position.y < -17)
                {
                    Y = Random.Range(yOffsetMin, yOffsetMax);
                }

                //print("X = " + X + "   Y = " + Y);
                moveDirection = new Vector2(X + Random.Range(0, 2), Y + Random.Range(0, 2));
                moveDirection = moveDirection.normalized;

                speed = Random.Range(speedMin, speedMax);

                yield return new WaitForSeconds(2);
            }
        }
    }

    private IEnumerator ChangingColor()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float a = spriteRenderer.color.a; //Random.Range(0.05f, 0.2f);
        float coeff = 1;
        while(true)
        {
            Color.RGBToHSV(color, out float H, out float S, out float V);

            color = Color.HSVToRGB(H + _colorOffset, S, V, true);

            a += alphaChangingSpeed * coeff;
            if (a < alphaMin || a > alphaMax)
                coeff = -coeff;

            spriteRenderer.color = new Color(color.r, color.g, color.b, a);

            yield return new WaitForSeconds(_colorChangeSpeed);
        }
    }
}
