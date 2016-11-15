using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace exercise_1.Algorith
{
    class PathFinder
    {
        public static List<Point> FindPath(Point start, Point finish, int[,] field, int heuristic, ref Rectangle[,] rFiled)
        {


            List<PathNode> OpenList = new List<PathNode>(); //те клетки, которые просмотрели
            List<PathNode> CloseList = new List<PathNode>(); //те клетки, которые нужно просмотреть
            int count = 0;
            PathNode startNode = new PathNode()
            {
                Position = start,
                Parent = null,
                GLenghtPath = 0,
                HLenght = GetHLenghtPath(start, finish, heuristic)
            };
            OpenList.Add(startNode);

            while (OpenList.Count > 0) // перебираем все узлы, которые есть в open, то есть всю карты
            {
                //System.Threading.Thread.Sleep(100);
                var currentNode = OpenList.OrderBy(node => node.FLenghtPath).First(); //берем минимальный узел из Open
                                                                                      //с минимальным значением F (sort -> first item)
                if (currentNode.Position == finish) // проверяем, дошли до конца ли
                    return GetResultPath(currentNode); //возвращаем путь от старта к финишу
                OpenList.Remove(currentNode); //удаляем текущий узел из Open, тк он нам в нем больше не нужен, мы его и так смотрим
                CloseList.Add(currentNode);  //добавляем узел в Close, тк это то, что просмотрели
                foreach (var neighbour in UnClosedNeigdours(currentNode, finish, field, heuristic)) //проверяем всех соседей текущего узла                                                                                        
                                                                                         //которые не являются частью нашего массива closed
                {
                    if (CloseList.Count(node => node.Position == neighbour.Position) > 0) //пропускаем элементы closed
                        continue;
                    var tempNode = OpenList.FirstOrDefault(node => node.Position == neighbour.Position); //берем соседа

                    if (tempNode == null) //если соседа нет в Open, добавляем его туда
                    {
                        if (neighbour.Position != finish)
                            rFiled[(int)neighbour.Position.X, (int)neighbour.Position.Y].Fill = Brushes.Aqua;

                        OpenList.Add(neighbour);
                    }
                    else
                    {
                        if (tempNode.GLenghtPath > neighbour.GLenghtPath) //проверяем на длину пути. если пришли более коротким путем,
                                                                          //то меняем текущий на этот коротокий
                        {
                            tempNode.Parent = currentNode;
                            tempNode.GLenghtPath = neighbour.FLenghtPath;
                        }
                    }
                }


                //  rFiled[(int)neighbour.Position.X, (int)neighbour.Position.Y].Fill = count;
            }

            return null; //если вдруг нет пути к финишу
        }
        #region эвристическая функция
        public static int GetHLenghtPath(Point start, Point finish, int heuristic)//считаем эвристическую функцию по пифагору
        {
            if (heuristic == 1)
            {
                return (int)Math.Sqrt(
                    Math.Pow((start.X - finish.X), 2) + Math.Pow((start.Y - finish.Y), 2)
                    );
            }
            else
                return (int)Math.Abs((start.X - finish.X)) + (int)Math.Abs((start.Y - finish.Y));

        }
        #endregion

        #region соседи
        //непосещенные соседи. соседа 4 (тк по диагонали нельяз ходить)
        private static List<PathNode> UnClosedNeigdours(PathNode pathNode, Point finish, int[,] field, int heuristic)
        {
            var result = new List<PathNode>();

            Point[] neighbourPoints = new Point[8];


            neighbourPoints[0] = new Point(pathNode.Position.X - 1, pathNode.Position.Y - 1);
            neighbourPoints[1] = new Point(pathNode.Position.X - 1, pathNode.Position.Y);
            neighbourPoints[2] = new Point(pathNode.Position.X - 1, pathNode.Position.Y + 1);
            neighbourPoints[3] = new Point(pathNode.Position.X, pathNode.Position.Y + 1);
            neighbourPoints[4] = new Point(pathNode.Position.X, pathNode.Position.Y - 1);
            neighbourPoints[5] = new Point(pathNode.Position.X + 1, pathNode.Position.Y - 1);
            neighbourPoints[6] = new Point(pathNode.Position.X + 1, pathNode.Position.Y);
            neighbourPoints[7] = new Point(pathNode.Position.X + 1, pathNode.Position.Y + 1);

            //заполняем список соседей на карте
            foreach (var point in neighbourPoints)
            {
                if (point.X < 0 || point.X >= field.GetLength(0)) //граница карты по Y
                    continue;
                if (point.Y < 0 || point.Y >= field.GetLength(1)) // граница карты по X
                    continue;
                

                int xWall = (int)point.X, yWall = (int)point.Y;

                if ((field[xWall, yWall] != 0)) // проверка на стенки
                    continue;



                if (xWall < (Math.Sqrt(field.Length) - 1) && yWall < (Math.Sqrt(field.Length) - 1))
                    if ((field[xWall + 1, yWall] == 1 || field[xWall, yWall + 1] == 1) && (point == neighbourPoints[0])) // верх-право. 
                        continue;
                if (xWall > 0 && yWall < (Math.Sqrt(field.Length) - 1))
                    if ((field[xWall - 1, yWall] == 1 || field[xWall, yWall + 1] == 1) && (point == neighbourPoints[5] )) // право-низ
                        continue;
                if (xWall > 0 && yWall > 0)
                    if ((field[xWall - 1, yWall] == 1 || field[xWall, yWall - 1] == 1) && (point == neighbourPoints[7] )) // низ - лево
                        continue;
                if (xWall < (Math.Sqrt(field.Length) - 1) && yWall > 0)
                    if ((field[xWall + 1, yWall] == 1 || field[xWall, yWall - 1] == 1) && (point == neighbourPoints[2])) // лево - верх
                        continue;



                var neighbourNode = new PathNode()
                {
                    Position = point,
                    Parent = pathNode,
                    GLenghtPath = pathNode.GLenghtPath + 1,
                    HLenght = GetHLenghtPath(point, finish, heuristic)
                };
                result.Add(neighbourNode);
            }


            return result;
        }
        #endregion

        #region результаты пути от Старта к Финишу. работает по принципу однонаправленного списка
        private static List<Point> GetResultPath(PathNode pathNode)
        {
            var result = new List<Point>();
            var current = pathNode;
            while (current != null)
            {
                result.Add(current.Position);
                current = current.Parent;
            }
            result.Reverse(); //обратный порядок
            return result;
        }
        #endregion
    }
}
