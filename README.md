# Factory
Небольшой проект демонстрирующий работу конвейера, двигая и изменяя элементы. 
## Table on contents
* [General info](#general_info)
* [Final result](#final_result)
* [Features](#features)
* [Technologies](#technologies)
* [Setup](#setup)
---
## General info
Это обычный проект написанный с помощью C# и Google Forms, которые предоставляют нам удобный и понятный интерфейс, кнопки, и многие другие объекты. Так же я хотел всунуть сюда асинхронность, однако эта функция не работает корректно :) 

---
## Final

https://github.com/Andezion/Factory/assets/115638748/972d850b-8185-4db4-b35c-e69402435b76

---
## Features
Основная функция программы отвечающая за поворот прямоугольника-хватателя должна была быть реализована как асинхронная для задержки во время её исполнения, однако, к сожалению, она не работает корректно. Однако другие задачи функция исполняет корректно. 

```
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
```
С течением времени - меняется угол поворота нашего прямоугольника. Каждую итерацию мы изменяем и пересчитываем новый угол и новые крайние точки нашего прямоугольника (движущаяся часть). Благодаря встроенным функция мы вращаем прямоугольник отталкиваясь от новыъ крайних точек. Так же с помощью простой логики - мы определяем должен ли появляться у прямоугольника сопутствующий квадрат. Удобность функции заключается в том, что мы можем добавить любой объект, и с лёгкостью прописать его движение. 

Так же получилось с легкостью реализовать движение конвейера и квадратов на нём всего-лишь в одном цикле.

```
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
```

Благодаря тому, что линии и квадраты у нас реализованы как массив - то в определённый момент мы просто показываем наши линии, попутно перемещая их вверх или вниз, благодаря чему создаётся впечатление движущегося конвейера. Положение квадратов считается в зависимости от расположения линий и показывается по такому же принципу как и линии. 

Возвращаясь к логике появления квадратов. Мы двигаемся от одного крайнего угла к другому, по ходу этого меняя некоторые булевский переменные.

```
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
```

Благодаря такому ходу мы легко меняем направление и пересчитываем скорость в зависимости от направления. Так же тут прописана переменная для переноса квадрата.

---
## Technologies
Проект сделан и использованием:
* C# и Visual Studio 2022
* Google Forms
* Linus Torvalds tears 
---
## Setup
Можете просто установить последний релиз, если вы используете Линукс - тоже установите последний релиз. Если у вас Мак ОС - не устанавливайте последний релиз, пожалуйста. 
