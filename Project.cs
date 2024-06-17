using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Factory
{
    public partial class Project : Form
    {
        private Pen p;

        private Timer timer;
        private Timer side_timer;
        private Timer delay_timer;

        private Graphics g;

        private int conveyorX;
        private int conveyorY;
        private int conveyorWidth = 100;
        private int conveyorHeight = 449;

        private List<int> linePositions;
        private int lineSpacing = 50;
        private int lineSpeed = 4;

        private List<int> side_linePositions;
        private int side_lineSpacing = 30;
        private int side_lineSpeed = 4;

        private List<int> side_side_linePositions;
        private int side_side_lineSpacing = 30;
        private int side_side_lineSpeed = 4;

        private int squareSize = 20;

        private float angle1 = 0;
        private float angle2 = (float)Math.PI / 4;
        private float speed = 0.3f;
        private float length = 130;

        private bool temp_for_left = true;
        private bool temp_for_right = true;

        private int blueSquareCount;
        private int greenSquareCount;
        private bool blueSquareCounted = false;
        private bool greenSquareCounted = false;

        private bool isDelayElapsed = false;
        private int currentSideIndex = 0; // Счетчик текущего треугольника для левого бокового конвейера
        private int currentSideSideIndex = 0;

        private bool leftRectangleSquareState = true;  // Состояние для левого прямоугольника
        private bool rightRectangleSquareState = false; // Состояние для правого прямоугольника

        int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
        int i1 = 0, j1 = 0, i2 = 0, j2 = 0;


        public Project()
        {
            InitializeComponent();

            p = new Pen(Color.Black);

            timer = new Timer();
            timer.Interval = 300;
            timer.Tick += Timer_Tick;

            side_timer = new Timer();
            side_timer.Interval = 300;
            side_timer.Tick += Side_Timer_Tick;

            delay_timer = new Timer(); // Инициализация таймера для задержки
            delay_timer.Interval = 3000; // Установите интервал на 3 секунды
            delay_timer.Tick += Delay_Timer_Tick;

            conveyorX = ClientSize.Width / 2 - conveyorWidth / 2;
            conveyorY = ClientSize.Height / 2 - conveyorHeight / 2;

            linePositions = new List<int>();
            for (int i = 0; i < conveyorHeight; i += lineSpacing)
            {
                linePositions.Add(i);
            }

            side_linePositions = new List<int>();
            for (int i = 0; i < 170; i += side_lineSpacing)
            {
                side_linePositions.Add(i);
            }

            side_side_linePositions = new List<int>();
            for (int i = 0; i < 170; i += side_side_lineSpacing)
            {
                side_side_linePositions.Add(i);
            }
        }
        private void Start_Click(object sender, EventArgs e)
        {
            Start.Visible = false;
            Exit.Visible = false;
            Restart.Visible = true;

            g = CreateGraphics();
            DrawConveyorAndLines();

            delay_timer.Start();

            timer.Start();
            side_timer.Start();
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void Project_Load(object sender, EventArgs e)
        {
            Restart.Visible = false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < linePositions.Count; i++)
            {
                linePositions[i] += lineSpeed;
                if (linePositions[i] > conveyorHeight)
                {
                    linePositions[i] = 0;
                }
            }

            if (temp_for_left)
            {
                angle1 -= speed;
                if (angle1 <= -3 * Math.PI / 4)
                {
                    temp_for_left = false;
                    leftRectangleSquareState = false; // Сбрасываем квадрат на 135 градусов
                }
            }
            else
            {
                angle1 += speed;
                if (angle1 >= 0)
                {
                    temp_for_left = true;
                    leftRectangleSquareState = true; // Появление квадрата в начальной позиции
                }
            }

            if (temp_for_right)
            {
                angle2 += speed;
                if (angle2 >= Math.PI)
                {
                    temp_for_right = false;
                    rightRectangleSquareState = true; // Появление квадрата на 180 градусов
                }
            }
            else
            {
                angle2 -= speed;
                if (angle2 <= Math.PI / 4)
                {
                    temp_for_right = true;
                    rightRectangleSquareState = false; // Сбрасываем квадрат на 45 градусов
                }
            }

            DrawConveyorAndLines();
        }


        private void Side_Timer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < side_linePositions.Count; i++)
            {
                side_linePositions[i] += side_lineSpeed;
                if (side_linePositions[i] > 170)
                {
                    side_linePositions[i] = 0;
                }
            }

            for (int i = 0; i < side_side_linePositions.Count; i++)
            {
                side_side_linePositions[i] += side_side_lineSpeed;
                if (side_side_linePositions[i] > 170)
                {
                    side_side_linePositions[i] = 0;
                }
            }

            DrawConveyorAndLines();
        }

        private void Delay_Timer_Tick(object sender, EventArgs e)
        {
            isDelayElapsed = true;

            if (currentSideIndex < side_linePositions.Count)
            {
                currentSideIndex++; // Увеличиваем индекс текущего треугольника для левого бокового конвейера
            }

            if (currentSideSideIndex < side_side_linePositions.Count)
            {
                currentSideSideIndex++; // Увеличиваем индекс текущего треугольника для правого бокового конвейера
            }
        }

        

        private void DrawConveyorAndLines()
        {
            g.Clear(Color.Aquamarine);

            // Рисование конвейера и линий
            g.DrawRectangle(p, conveyorX, conveyorY, conveyorWidth, conveyorHeight);
            g.FillRectangle(Brushes.Gray, conveyorX + 1, conveyorY + 1, conveyorWidth - 1, conveyorHeight - 1);

            int smallConveyorX = 20;
            int smallConveyorY = 190;
            int smallConveyorWidth = 30;
            int smallConveyorHeight = 170;

            g.DrawRectangle(p, smallConveyorX, smallConveyorY, smallConveyorWidth, smallConveyorHeight);
            g.FillRectangle(Brushes.Gray, smallConveyorX + 1, smallConveyorY + 1, smallConveyorWidth - 1, smallConveyorHeight - 1);

            int small_newX = 536;
            int small_newY = 190;

            g.DrawRectangle(p, small_newX, small_newY, smallConveyorWidth, smallConveyorHeight);
            g.FillRectangle(Brushes.Gray, small_newX + 1, small_newY + 1, smallConveyorWidth - 1, smallConveyorHeight - 1);

            g.DrawRectangle(p, 100, 220.5f, 50, 50);
            g.FillRectangle(Brushes.Bisque, 101, 221f, 49, 49);

            g.DrawRectangle(p, 436, 220.5f, 50, 50);
            g.FillRectangle(Brushes.Bisque, 437, 221f, 49, 49);

            g.DrawRectangle(p, 10, 120, 50, 70);
            g.FillRectangle(Brushes.Crimson, 11, 121, 49, 69);

            g.DrawRectangle(p, 526, 120, 50, 70);
            g.FillRectangle(Brushes.Crimson, 527, 121, 49, 69);

            g.DrawEllipse(p, 130, 89.5f, 50, 50);
            g.FillEllipse(Brushes.Blue, 130.5f, 90f, 49, 49);

            g.DrawEllipse(p, 406, 89.5f, 50, 50);
            g.FillEllipse(Brushes.Green, 406.5f, 90f, 49, 49);

            float leftbalkaX = 125f;
            float leftbalkaY = 246.5f;
            float leftendX = leftbalkaX + (float)(length * Math.Cos(angle1));
            float leftendY = leftbalkaY + (float)(length * Math.Sin(angle1));
            float leftWidth = 10;

            DrawRotatedRectangle(g, Brushes.Black, leftbalkaX, leftbalkaY, leftendX, leftendY, leftWidth, leftRectangleSquareState, false);

            float rigthbalkaX = 461f;
            float rigthbalkaY = 246.5f;
            float rigthendX = rigthbalkaX + (float)(length * Math.Cos(angle2));
            float rigthendY = rigthbalkaY - (float)(length * Math.Sin(angle2));
            float rightWidth = 10;

            DrawRotatedRectangle(g, Brushes.Black, rigthbalkaX, rigthbalkaY, rigthendX, rigthendY, rightWidth, false, rightRectangleSquareState);

            g.DrawEllipse(p, 112.5f, 233f, 25, 25);
            g.FillEllipse(Brushes.Coral, 113, 234f, 24, 24);

            g.DrawEllipse(p, 448.5f, 233f, 25, 25);
            g.FillEllipse(Brushes.Coral, 449, 234f, 24, 24);

            for (int i = 0; i < linePositions.Count; i++)
            {
                int pos = linePositions[i];
                int nextPos = (i + 1 < linePositions.Count) ? linePositions[i + 1] : conveyorHeight;

                g.DrawLine(p, conveyorX, conveyorY + pos, conveyorX + conveyorWidth, conveyorY + pos);

                if (conveyorY + pos + lineSpacing / 2 - squareSize / 2 < 250)
                {
                    g.FillRectangle(Brushes.Red, conveyorX + conveyorWidth / 2 - squareSize / 2, conveyorY + pos + lineSpacing / 2 - squareSize / 2, squareSize, squareSize);
                }
            }

            for (int i = 0; i < side_linePositions.Count; i++)
            {
                int pos = side_linePositions[i];
                int nextPos = (i + 1 < side_linePositions.Count) ? side_linePositions[i + 1] : conveyorHeight;

                g.DrawLine(p, smallConveyorX, smallConveyorY + pos, smallConveyorX + smallConveyorWidth, smallConveyorY + pos);

                x1 = smallConveyorX + smallConveyorWidth / 2 - squareSize / 4;
                y1 = smallConveyorY + pos + side_lineSpacing / 2 - squareSize / 4;
                x2 = squareSize / 2;
                y2 = squareSize / 2;
            }
            if (isDelayElapsed)
            {
                g.FillRectangle(Brushes.MidnightBlue, x1, y1, x2, y2);
                if (y1 > small_newY + smallConveyorHeight && !blueSquareCounted)
                {
                    blueSquareCount++;
                    blueSquareCounted = true;  // Устанавливаем флаг
                }
                else if (y1 <= small_newY + smallConveyorHeight)
                {
                    blueSquareCounted = false;  // Сбрасываем флаг
                }
            }


            for (int i = 0; i < side_side_linePositions.Count; i++)
            {
                int pos = side_side_linePositions[i];
                int nextPos = (i + 1 < side_side_linePositions.Count) ? side_side_linePositions[i + 1] : conveyorHeight;

                g.DrawLine(p, small_newX, small_newY + pos, small_newX + smallConveyorWidth, small_newY + pos);

                i1 = small_newX + smallConveyorWidth / 2 - squareSize / 4;
                j1 = small_newY + pos + side_side_lineSpacing / 2 - squareSize / 4;
                i2 = squareSize / 2;
                j2 = squareSize / 2;
            }
            if (isDelayElapsed)
            {
                g.FillRectangle(Brushes.Green, i1, j1, i2, j2);
                if (j1 > small_newY + smallConveyorHeight && !greenSquareCounted)
                {
                    greenSquareCount++;
                    greenSquareCounted = true;  // Устанавливаем флаг
                }
                else if (j1 <= small_newY + smallConveyorHeight)
                {
                    greenSquareCounted = false;  // Сбрасываем флаг
                }
            }

            g.DrawRectangle(p, 20, 360, 100, 50);
            g.FillRectangle(Brushes.Violet, 21, 361, 99, 49);
            g.DrawString(blueSquareCount.ToString(), new Font("Arial", 16), Brushes.Black, new PointF(50, 370));

            g.DrawRectangle(p, 476, 360, 100, 50);
            g.FillRectangle(Brushes.Violet, 477, 361, 99, 49);
            g.DrawString(greenSquareCount.ToString(), new Font("Arial", 16), Brushes.Black, new PointF(516, 370));
        }


        private void Restart_Click(object sender, EventArgs e)
        {
            timer.Stop();
            side_timer.Stop();

            // Скрываем все элементы управления на форме
            foreach (Control control in Controls)
            {
                control.Visible = false;
            }

            y1 = 190;
            j1 = 190;

            angle1 = 0;
            angle2 = (float)Math.PI / 4;

            // Делаем видимыми только кнопки "Start" и "Exit"
            Start.Visible = true;
            Exit.Visible = true;
            isDelayElapsed = false;

            blueSquareCount = 0;
            greenSquareCount = 0;

            // Очищаем графику формы
            g.Clear(Color.Aquamarine);
        }


        private async void DrawRotatedRectangle(Graphics g, Brush brush, float x1, float y1, float x2, float y2, float width, bool drawRedSquare, bool drawBlueSquare)
        {
            double angle = Math.Atan2(y2 - y1, x2 - x1); // Угол наклона прямоугольника
            float length = (float)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1)); // Длина прямоугольника

            float endX = x1 + length * (float)Math.Cos(angle);
            float endY = y1 + length * (float)Math.Sin(angle);

            g.TranslateTransform(x1, y1); // Перемещение к начальной точке
            g.RotateTransform((float)(angle * 180 / Math.PI)); // Поворот координатной системы

            // Рисование прямоугольника
            g.DrawRectangle(p, -1, -width / 2 - 1, length + 1.5f, width + 1.5f);
            g.FillRectangle(brush, 0, -width / 2, length, width);

            // Логика появления квадратов
            if (drawRedSquare)
            {
                if(endX == 160)
                {
                    await Task.Delay(1000);
                }
                if(endX <= 160)
                {
                    g.FillRectangle(Brushes.Blue, length - 20 / 2 + 8, -20 / 2 - 2, 20 + 5, 20 + 5);
                }
                else
                {
                    g.FillRectangle(Brushes.Red, length - 20 / 2 + 8, -20 / 2 - 2, 20 + 5, 20 + 5);
                }
            }

            if (drawBlueSquare) 
            {
                if(endX == 409)
                {
                    await Task.Delay(1000);
                }
                if(endX >= 409)
                {
                    g.FillRectangle(Brushes.Green, length - 20 / 2 + 8, -20 / 2 - 2, 20 + 5, 20 + 5);
                }
                else
                {
                    g.FillRectangle(Brushes.Red, length - 20 / 2 + 8, -20 / 2 - 2, 20 + 5, 20 + 5);
                }
            }

            // Рисование квадратов на концах прямоугольников
            float squareSize = 15;
            g.DrawRectangle(p, length - squareSize / 2 + 7, -squareSize / 2 - 1, squareSize + 1.5f, squareSize + 1.5f);
            g.FillRectangle(Brushes.Bisque, length - squareSize / 2 + 8, -squareSize / 2, squareSize, squareSize);

            g.ResetTransform(); // Сброс трансформаций
        }

    }
}