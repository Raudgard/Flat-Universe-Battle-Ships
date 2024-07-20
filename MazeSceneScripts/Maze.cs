using System;
using System.Collections.Generic;
using System.Threading;
using System.Drawing;


public class Maze
{
    public byte[,] maze;
    //public Point[] points;
    private (ushort, ushort) coordinatesOfLongetsWay = (0, 0);

    //public static Point[] pointsForGrafic;
    public static ushort mazeHeight;
    public static ushort mazeWidth;
    public static uint counterOfPoints; //счетчик созданных/проверенных точек

    public Maze(ushort x, ushort y)
    {
        maze = MazeCreate(x, y);
    }

    public int Width
    {
        get { return maze.GetLength(1); }
    }

    public int Height
    {
        get { return maze.GetLength(0); }
    }

    public (ushort, ushort) StartCoordinates { get; set; }
    public (ushort, ushort) CoordinatesOfLongestWay { get => (coordinatesOfLongetsWay.Item1, coordinatesOfLongetsWay.Item2); set => (coordinatesOfLongetsWay.Item1, coordinatesOfLongetsWay.Item2) = value; }
    public uint LengthOfLongestWay { get; set; }

    private byte[,] MazeCreate(ushort x, ushort y)//x,y - количество проходов по горизонтали и вертикали
    {
        byte[,] maze = StackAlgorithm(x, y);
        //points = pointsForGrafic;
        return maze;
    }


    public (ushort, ushort) RandomStartCoordinates()
    {
        Random random = new Random();
        ushort Xrandom = (ushort)random.Next(1, maze.GetLength(0) - 2);
        ushort Yrandom = (ushort)random.Next(1, maze.GetLength(1) - 2);
        if (Xrandom % 2 == 0) Xrandom++;//случайная нечетная координата по Х
        if (Yrandom % 2 == 0) Yrandom++;//случайная нечетная координата по Y

        byte choice = (byte)random.Next(1, 5);
        if (choice == 1) return (Xrandom, 1);
        else if (choice == 2) return (1, Yrandom);
        else if (choice == 3) return (Xrandom, (ushort)(maze.GetLength(1) - 2));
        else return ((ushort)(maze.GetLength(0) - 2), Yrandom);
    }

    public void GetCoordinatesOfLongestWay()
    {
        byte[,] copyMaze = new byte[maze.GetLength(0), maze.GetLength(1)];
        for (ushort i = 0; i < maze.GetLength(0); i++)
            for (ushort j = 1; j < maze.GetLength(1); j++)
                copyMaze[i, j] = maze[i, j];

        Stack<(ushort, ushort)> currentPosition = new Stack<(ushort, ushort)>();//стек текущей позиции
        List<(ushort, ushort, uint)> deadlocks = new List<(ushort, ushort, uint)>();//стек тупиков
        uint stepCounter = 0;//счетчик шагов от стартовой точки

        try { currentPosition.Push(StartCoordinates); }
        catch
        { throw new NullReferenceException("Сначала нужно создать стартовые координаты!"); }//если стартовых координат еще нет

        counterOfPoints = 0;
        copyMaze[currentPosition.Peek().Item1, currentPosition.Peek().Item2] = 2;//стартовая позиция посещена
        counterOfPoints++;

        //Thread percentage = new Thread(AlgorithmsForCreatingMaze.ShowReadyPercentage);
        //percentage.Start();

        do
        {
            //Console.WriteLine($"Current position is ({currentPosition.Peek().Item1}, {currentPosition.Peek().Item2})");
            copyMaze[currentPosition.Peek().Item1, currentPosition.Peek().Item2] = 2;//текущая позиция посещена
            counterOfPoints++;

            if (copyMaze[currentPosition.Peek().Item1 - 1, currentPosition.Peek().Item2] == 1)
            {
                copyMaze[currentPosition.Peek().Item1 - 1, currentPosition.Peek().Item2] = 2;//промежуток помечаем посещенным
                currentPosition.Push(((ushort)(currentPosition.Peek().Item1 - 2), currentPosition.Peek().Item2));//добавляем следующую точку в стек текущей позиции
                stepCounter++;
            }
            else if (copyMaze[currentPosition.Peek().Item1 + 1, currentPosition.Peek().Item2] == 1)
            {
                copyMaze[currentPosition.Peek().Item1 + 1, currentPosition.Peek().Item2] = 2;//промежуток помечаем посещенным
                currentPosition.Push(((ushort)(currentPosition.Peek().Item1 + 2), currentPosition.Peek().Item2));//добавляем следующую точку в стек текущей позиции
                stepCounter++;
            }
            else if (copyMaze[currentPosition.Peek().Item1, currentPosition.Peek().Item2 - 1] == 1)
            {
                copyMaze[currentPosition.Peek().Item1, currentPosition.Peek().Item2 - 1] = 2;//промежуток помечаем посещенным
                currentPosition.Push((currentPosition.Peek().Item1, (ushort)(currentPosition.Peek().Item2 - 2)));//добавляем следующую точку в стек текущей позиции
                stepCounter++;
            }
            else if (copyMaze[currentPosition.Peek().Item1, currentPosition.Peek().Item2 + 1] == 1)
            {
                copyMaze[currentPosition.Peek().Item1, currentPosition.Peek().Item2 + 1] = 2;//промежуток помечаем посещенным
                currentPosition.Push((currentPosition.Peek().Item1, (ushort)(currentPosition.Peek().Item2 + 2)));//добавляем следующую точку в стек текущей позиции
                stepCounter++;
            }

            else
            {
                deadlocks.Add((currentPosition.Peek().Item1, currentPosition.Peek().Item2, stepCounter));//если ни в одном направлении нет прохода, значит тупик. Добавляем в стек тупиков.                    
                counterOfPoints--;

                while (currentPosition.Count == 0 ? false :
                      (copyMaze[currentPosition.Peek().Item1 - 1, currentPosition.Peek().Item2] != 1 &&
                       copyMaze[currentPosition.Peek().Item1 + 1, currentPosition.Peek().Item2] != 1 &&
                       copyMaze[currentPosition.Peek().Item1, currentPosition.Peek().Item2 - 1] != 1 &&
                       copyMaze[currentPosition.Peek().Item1, currentPosition.Peek().Item2 + 1] != 1))
                {
                    currentPosition.Pop();
                    stepCounter--;
                }//идем обратно: выдергиваем позиции из текущего положения до тех пор пока в окружении не появится непосещенная точка. И отнимаем шаги пока идем назад.
            }
        }
        while (currentPosition.Count != 0);

        LengthOfLongestWay = deadlocks[0].Item3;
        CoordinatesOfLongestWay = (deadlocks[0].Item1, deadlocks[0].Item2);//кортеж, содержащий результат - координаты точки, дальнейшей от стартовой. До проверки всех тупиков присваиваются координаты первого тупика.

        for (ushort i = 1; i < deadlocks.Count && deadlocks[i].Item3 != 0; i++)
        {
            if (deadlocks[i].Item3 > LengthOfLongestWay)
            {
                LengthOfLongestWay = deadlocks[i].Item3;
                CoordinatesOfLongestWay = (deadlocks[i].Item1, deadlocks[i].Item2);
            }
        }//ищем длиннейший путь среди тупиков. Присваиваем его координаты кортежу результата.

        //Console.WriteLine();
        //for(int i=0;i<deadlocks.Count;i++)
        //    Console.WriteLine(deadlocks[i].Item1+"  "+deadlocks[i].Item2+"  "+deadlocks[i].Item3);
        //Console.WriteLine();

        //string msg = "Все точки проверены!\t\t\t";
        //for (int i = 1; i < copyMaze.GetLength(0); i += 2)
        //    for (int j = 1; j < copyMaze.GetLength(1); j += 2)
        //        if (copyMaze[i, j] != 2) msg = "НЕ все точки проверены!\t\t\t";
        //Console.WriteLine(msg);

    }

    private byte[,] StackAlgorithm(ushort x, ushort y)
    {
        mazeHeight = x;
        mazeWidth = y;
        counterOfPoints = 0;

        //Thread percentage = new Thread(AlgorithmsForCreatingMaze.ShowReadyPercentage);
        //percentage.Start();

        Random random = new Random();
        byte[,] maze = new byte[x * 2 + 1, y * 2 + 1];
        for (ushort i = 0; i < maze.GetLength(0); i++)
            for (ushort j = 0; j < maze.GetLength(1); j++)
                maze[i, j] = 0;//заполнили массив нулями
        //Console.WriteLine("The matrix for the maze created.\t\t\t\nCreating maze. . .");
        //Console.BufferHeight = 5000;
        //Console.WindowWidth = 120;

        //(ushort, ushort) currentPoint = ()

        ushort Xcoordinate;
        ushort Ycoordinate;//координаты текущей клетки

        void firstCoordinates()
        {
            Xcoordinate = (ushort)random.Next(1, maze.GetLength(0) - 1);
            Ycoordinate = (ushort)random.Next(1, maze.GetLength(1) - 1);
            if (Xcoordinate % 2 == 0) Xcoordinate++;//случайная координата по Х
            if (Ycoordinate % 2 == 0) Ycoordinate++;//случайная координата по Y
        }//функция поиска первичных (случайных) координат

        firstCoordinates();
        //Console.WriteLine("Первичные координаты - " + Xcoordinate + ", " + Ycoordinate);

        Stack<(ushort, ushort)> createdPoints = new Stack<(ushort, ushort)>();
        (ushort, ushort) currentPoint = (Xcoordinate, Ycoordinate);//первая точка стала текущей точкой
        createdPoints.Push(currentPoint);//первая точка добавлена в стек
        counterOfPoints++;
        byte unvisitedNeighbors;//количество непосещенных соседей у точки
        List<byte> directions = new List<byte>(4);

        List<(int, int)> pointsForGraficTemp = new List<(int, int)>();

        do
        {
            maze[Xcoordinate, Ycoordinate] = 1;// точка стала посещенной
            pointsForGraficTemp.Add((Ycoordinate, Xcoordinate));
            currentPoint = (Xcoordinate, Ycoordinate);//текущая точка
                                                      //Console.WriteLine($"Current Point is ({Xcoordinate}, {Ycoordinate})");

            unvisitedNeighbors = 0;
            directions.Clear();

            if ((Xcoordinate - 1 != 0) && maze[Xcoordinate - 2, Ycoordinate] == 0) { unvisitedNeighbors++; directions.Add(1); }
            if ((Xcoordinate + 1 != maze.GetLength(0) - 1) && (maze[Xcoordinate + 2, Ycoordinate] == 0)) { unvisitedNeighbors++; directions.Add(2); }
            if ((Ycoordinate - 1 != 0) && maze[Xcoordinate, Ycoordinate - 2] == 0) { unvisitedNeighbors++; directions.Add(3); }
            if ((Ycoordinate + 1 != maze.GetLength(1) - 1) && maze[Xcoordinate, Ycoordinate + 2] == 0) { unvisitedNeighbors++; directions.Add(4); }

            if (unvisitedNeighbors == 0)
            {
                currentPoint = createdPoints.Pop();
                Xcoordinate = currentPoint.Item1;
                Ycoordinate = currentPoint.Item2;
            }
            else
            {
                createdPoints.Push(currentPoint);//текущая точка добавляется в стек
                unvisitedNeighbors = directions[random.Next(directions.Count)];

                if (unvisitedNeighbors == 1)
                {
                    maze[Xcoordinate - 1, Ycoordinate] = 1;//убрали стену
                    Xcoordinate -= 2;
                    counterOfPoints++;
                }
                else if (unvisitedNeighbors == 2)
                {
                    maze[Xcoordinate + 1, Ycoordinate] = 1;
                    Xcoordinate += 2;
                    counterOfPoints++;
                }
                else if (unvisitedNeighbors == 3)
                {
                    maze[Xcoordinate, Ycoordinate - 1] = 1;
                    Ycoordinate -= 2;
                    counterOfPoints++;
                }
                else if (unvisitedNeighbors == 4)
                {
                    maze[Xcoordinate, Ycoordinate + 1] = 1;
                    Ycoordinate += 2;
                    counterOfPoints++;
                }
            }
        }
        while (createdPoints.Count != 0);

        //pointsForGrafic = pointsForGraficTemp.ToArray();
        return maze;
    }

}
