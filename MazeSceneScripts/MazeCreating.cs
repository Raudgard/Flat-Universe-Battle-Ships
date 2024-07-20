using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class MazeCreating : MonoBehaviour
{
    public GameObject rectangularBlock;
    public GameObject mazeGameObject;

    private Maze maze;
    public ushort heightOfNewMaze, widthOfNewMaze;
    private (int X , int Y)  START_POSITION, FINISH_POSITION;

    public GameObject whiteField;
    public GameObject startZone;
    public GameObject finishZone;
    public Color startZoneColor1, startZoneColor2;
    public Color finishZoneColor1, finishZoneColor2;
    public float degree; //ступень изменения цвета зон при мерцании
    public float degreeFactor; // множитель для ускорения изменения цвета зон при мерцании

    public float sizeFactor = 1;

    private void Start()
    {
        //StartCoroutine(CheckForLoadSceneAndResizeCamera());
        CreateMaze();
        CreateStartAndFinish();

    }

    


    private void CreateMaze()
    {
        maze = new Maze(widthOfNewMaze, heightOfNewMaze);
        maze.StartCoordinates = maze.RandomStartCoordinates();
        START_POSITION = (maze.StartCoordinates.Item1, maze.StartCoordinates.Item2);
        maze.GetCoordinatesOfLongestWay();
        FINISH_POSITION = (maze.CoordinatesOfLongestWay.Item1, maze.CoordinatesOfLongestWay.Item2);

        GameObject block;

        for (int i = 0; i < maze.Height; i += 2)
        {
            for (int j = 0; j < maze.Width; j += 2)
            {
                if (i < maze.Height - 2 && maze.maze[i, j] == 0 && maze.maze[i + 1, j] == 0)
                {
                    block = Instantiate(rectangularBlock) as GameObject;
                    block.transform.localScale *= sizeFactor;
                    block.transform.position = new Vector3((i + 1) * sizeFactor, j * sizeFactor, -5);
                    block.transform.parent = mazeGameObject.transform;
                }

                if (j < maze.Width - 2 && maze.maze[i, j] == 0 && maze.maze[i, j + 1] == 0)
                {
                    block = Instantiate(rectangularBlock) as GameObject;
                    block.transform.localScale *= sizeFactor;
                    block.transform.position = new Vector3(i * sizeFactor, (j + 1) * sizeFactor, -5);
                    block.transform.Rotate(new Vector3(0, 0, 90));
                    block.transform.parent = mazeGameObject.transform;
                }
            }

        }

    }

    private void CreateStartAndFinish()
    {
        startZone = Instantiate(whiteField) as GameObject;
        startZone.GetComponent<SpriteRenderer>().color = startZoneColor1;
        startZone.transform.position = new Vector3(START_POSITION.X, START_POSITION.Y, 1) * sizeFactor;
        startZone.transform.parent = mazeGameObject.transform;
        startZone.name = "Start Zone";

        finishZone = Instantiate(whiteField) as GameObject;
        finishZone.GetComponent<SpriteRenderer>().color = finishZoneColor1;
        finishZone.transform.position = new Vector3(FINISH_POSITION.X, FINISH_POSITION.Y, 1) * sizeFactor;
        finishZone.name = "Finish Zone";
        BoxCollider2D boxCollider2D = finishZone.AddComponent<BoxCollider2D>();
        boxCollider2D.isTrigger = true;
        FinishZoneScript finishZoneScript = finishZone.AddComponent<FinishZoneScript>();
        finishZoneScript.entranceSide = GetEntranceSide();
        finishZone.transform.parent = mazeGameObject.transform;

        StartCoroutine(Flicker());
    }

    /// <summary>
    /// Мерцание стартовой и финишной зон
    /// </summary>
    /// <returns></returns>
    private IEnumerator Flicker()
    {
        UnityEngine.Color _startZoneColor = startZone.GetComponent<SpriteRenderer>().color;
        UnityEngine.Color _finishZoneColor = finishZone.GetComponent<SpriteRenderer>().color;

        while (true)
        {
            while (_startZoneColor != startZoneColor2 && _finishZoneColor != finishZoneColor2)
            {
                _startZoneColor = new UnityEngine.Color(_startZoneColor.r - degree * degreeFactor, _startZoneColor.g, _startZoneColor.b, _startZoneColor.a + degree * degreeFactor);
                if (_startZoneColor.r < startZoneColor2.r)
                    _startZoneColor.r = startZoneColor2.r;
                if (_startZoneColor.a > startZoneColor2.a)
                    _startZoneColor.a = startZoneColor2.a;
                startZone.GetComponent<SpriteRenderer>().color = _startZoneColor;

                _finishZoneColor = new UnityEngine.Color(_finishZoneColor.r, _startZoneColor.g + degree * degreeFactor, _startZoneColor.b, _finishZoneColor.a + degree * degreeFactor);
                if (_finishZoneColor.g > finishZoneColor2.g)
                    _finishZoneColor.g = finishZoneColor2.g;
                if (_finishZoneColor.a > finishZoneColor2.a)
                    _finishZoneColor.a = finishZoneColor2.a;
                finishZone.GetComponent<SpriteRenderer>().color = _finishZoneColor;

                yield return null;
            }

            while (_startZoneColor != startZoneColor1 && _finishZoneColor != finishZoneColor1)
            {
                _startZoneColor = new UnityEngine.Color(_startZoneColor.r + degree, _startZoneColor.g, _startZoneColor.b, _startZoneColor.a - degree);
                if (_startZoneColor.r > startZoneColor1.r)
                    _startZoneColor.r = startZoneColor1.r;
                if (_startZoneColor.a < startZoneColor1.a)
                    _startZoneColor.a = startZoneColor1.a;
                startZone.GetComponent<SpriteRenderer>().color = _startZoneColor;

                _finishZoneColor = new UnityEngine.Color(_finishZoneColor.r, _startZoneColor.g - degree * degreeFactor, _startZoneColor.b, _finishZoneColor.a - degree * degreeFactor);
                if (_finishZoneColor.g < finishZoneColor1.g)
                    _finishZoneColor.g = finishZoneColor1.g;
                if (_finishZoneColor.a < finishZoneColor1.a)
                    _finishZoneColor.a = finishZoneColor1.a;
                finishZone.GetComponent<SpriteRenderer>().color = _finishZoneColor;

                yield return null;
            }

        }

    }


    private int GetEntranceSide()
    {
        print("top = " + maze.maze[FINISH_POSITION.X, FINISH_POSITION.Y + 1]);
        print("right = " + maze.maze[FINISH_POSITION.X + 1, FINISH_POSITION.Y]);
        print("bottom = " + maze.maze[FINISH_POSITION.X, FINISH_POSITION.Y - 1]);
        print("left = " + maze.maze[FINISH_POSITION.X - 1, FINISH_POSITION.Y]);

        if (maze.maze[FINISH_POSITION.X, FINISH_POSITION.Y + 1] == 1) return 1;
        if (maze.maze[FINISH_POSITION.X + 1, FINISH_POSITION.Y] == 1) return 2;
        if (maze.maze[FINISH_POSITION.X, FINISH_POSITION.Y - 1] == 1) return 3;
        if (maze.maze[FINISH_POSITION.X - 1, FINISH_POSITION.Y] == 1) return 4;

        return 0;
    }



    private IEnumerator CheckForLoadSceneAndResizeCamera()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        while (activeScene != SceneManager.GetSceneByName("Maze"))
        {
            activeScene = SceneManager.GetActiveScene();
            yield return null;
        }
        Player_Data.ResizeCamera();
    }
    
}
