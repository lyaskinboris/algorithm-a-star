using exercise_1.Algorith;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace exercise_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Rectangle[,] rField = new Rectangle[0, 0];
        public int[,] field = new int[0, 0];
        public Point start, finish;
        private int sizeOfField;
        private int item;
        private int[] tempStart = new int[2], tempFinish = new int[2];
        private List<Point> result;
        private int heuristic;
        private bool flagResult;

        public MainWindow()
        {
            InitializeComponent();
            heuristic = 1;
        }

        //начальные значения
        private void Initialization()
        {
            start.X = 0; start.Y = 0;
            finish.X = 1; finish.Y = 1;
            item = 0;
            flagResult = false;
            if (result != null)
                foreach (var clearResult in result)
                    rField[(int)clearResult.X, (int)clearResult.Y].Fill = Brushes.White;
        }

        //создание поля
        private void button_Click(object sender, RoutedEventArgs e)
        {
            Initialization();
            if (SizeField.Text != "" && SizeField.Text.Length == SizeField.Text.Where(c => char.IsDigit(c)).Count())
            {
                if (Convert.ToInt32(SizeField.Text) <= 30)
                {
                    sizeOfField = Convert.ToInt32(SizeField.Text);
                    if (sizeOfField >= 2)
                    {
                        uniGrid.Children.Clear();
                        rField = new Rectangle[sizeOfField, sizeOfField];

                        field = new int[sizeOfField, sizeOfField];
                        for (int i = 0; i < sizeOfField; ++i)
                            for (int j = 0; j < sizeOfField; ++j)
                            {
                                int RectangleCoordinatesX = i;
                                int RectangleCoordinatesY = j;
                                rField[i, j] = new Rectangle();
                                rField[i, j].Fill = Brushes.White;
                                rField[i, j].Stroke = Brushes.Black;
                                if ((int)start.X == i && (int)start.Y == j)
                                    StartCellColor((int)start.X, (int)start.Y);
                                if ((int)finish.X == i && (int)finish.Y == j)
                                    FinishCellColor((int)finish.X, (int)finish.Y);
                                rField[i, j].MouseLeftButtonDown += (sender2, e2) => Rectangle_PreviewMouseLeft(sender2, e2, RectangleCoordinatesX, RectangleCoordinatesY);
                                uniGrid.Children.Add(rField[i, j]);
                            }
                    }
                    else
                        MessageBox.Show("Поле должно содержать больше 1на1");
                }
                else
                    MessageBox.Show("Размер поля должен быть меньше чем 30 на 30");
            }
            else
                MessageBox.Show("неккоректный размер поля");
        }
        
        //указание цвета клетки для старта
        void StartCellColor(int RX, int RY)
        {
            rField[RX, RY].Fill = Brushes.Yellow;
            tempStart[0] = RX;
            tempStart[1] = RY;
            start = new Point(RX, RY);
        }

        //указание цвета клетки для финиша
        void FinishCellColor(int RX, int RY)
        {
            rField[RX, RY].Fill = Brushes.Pink;
            tempFinish[0] = RX;
            tempFinish[1] = RY;
            finish = new Point(RX, RY);
        }

        //рисуем старт, финиш, стенки
        private void Rectangle_PreviewMouseLeft(object sender, MouseButtonEventArgs e, int RectangleCoordinatesX, int RectangleCoordinatesY)
        {
            if (!flagResult)
            {
                switch (item)
                {
                    case 1:
                        rField[tempStart[0], tempStart[1]].Fill = Brushes.White; //старт
                        StartCellColor(RectangleCoordinatesX, RectangleCoordinatesY);
                        break;
                    case 2:
                        rField[RectangleCoordinatesX, RectangleCoordinatesY].Fill = Brushes.Brown; //стена
                        field[RectangleCoordinatesX, RectangleCoordinatesY] = 1;
                        break;
                    case 3:
                        rField[tempFinish[0], tempFinish[1]].Fill = Brushes.White; //финиш
                        FinishCellColor(RectangleCoordinatesX, RectangleCoordinatesY);
                        break;
                    default:
                        break;
                }
            }
            else
                MessageBox.Show("Очистите поле");
        }

        //рисуем результат
        private void Result_Click(object sender, RoutedEventArgs e)
        {
            if (!flagResult)
            {
                if (SizeField.Text != "" && SizeField.Text.Length == SizeField.Text.Where(c => char.IsDigit(c)).Count() && Convert.ToInt32(SizeField.Text) < 31)
                {
                    if (FirstRadio.IsChecked == true)
                        heuristic = 1;
                    else
                        heuristic = 2;
                    result = PathFinder.FindPath(start, finish, field, heuristic);
                    if (result != null)
                    {
                        foreach (var drawResult in result)
                            if (!((drawResult.X == start.X && drawResult.Y == start.Y) || (drawResult.X == finish.X && drawResult.Y == finish.Y)))
                                rField[(int)drawResult.X, (int)drawResult.Y].Fill = Brushes.Green;
                        flagResult = true;
                    }
                    else
                    {
                        Initialization();
                        MessageBox.Show("Пути от старта в финишу нет");
                    }
                }
            }
            else
                MessageBox.Show("Очистите поле.");

        }

        //очищаем поле
        private void ClearField_Click(object sender, RoutedEventArgs e)
        {
            if (SizeField.Text != "" && SizeField.Text.Length == SizeField.Text.Where(c => char.IsDigit(c)).Count() && Convert.ToInt32(SizeField.Text) < 31)
            {
                Initialization();
                for (int i = 0; i < sizeOfField; ++i)
                    for (int j = 0; j < sizeOfField; ++j)
                        if ((field[i, j] == 1))
                        {
                            rField[i, j].Fill = Brushes.White;
                            field[i, j] = 0;
                        }
                StartCellColor((int)start.X, (int)start.Y);
                FinishCellColor((int)finish.X, (int)finish.Y);
            }
        }

        // элементы: стенка, старт, финиш
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            item = (int)Types.start;
        }

        private void First_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Wall_Click(object sender, RoutedEventArgs e)
        {
            item = (int)Types.wall;
        }
        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            item = (int)Types.finish;
        }

    }
}
