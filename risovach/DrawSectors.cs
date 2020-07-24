using System;
using System.Drawing;

namespace risovach
{
    public class DrawSectors
    {
        public static Bitmap MakeLine(Bitmap source_image, Color linego) //рисуем контуры раскраски; 1 - раскрашенное изображение, 2 - цвет линий
        {
            int height = source_image.Height;
            int width = source_image.Width;
            Bitmap lines_picture = new Bitmap(width, height);
            for (int y = 0; y < height - 1; y++) //обрабатываю изображение для уменьшения кол-ва цветов
            {
                for (int x = 0; x < width - 1; x++)
                {
                    Color pixel1 = source_image.GetPixel(x, y);
                    Color pixel2 = source_image.GetPixel(x + 1, y);
                    Color pixel3 = source_image.GetPixel(x, y + 1);
                    Color pixel4 = source_image.GetPixel(x + 1, y + 1);
                    if (pixel1 == pixel2 && pixel1 == pixel3 && pixel1 == pixel4 && pixel1 != Color.FromArgb(255, 0, 0, 0) ||
                        pixel1 != pixel2 && pixel2 == Color.FromArgb(255, 0, 0, 0) && pixel3 == Color.FromArgb(255, 0, 0, 0) && pixel4 == Color.FromArgb(255, 0, 0, 0) ||
                        pixel1 == pixel2 && pixel2 != pixel3 && pixel3 == Color.FromArgb(255, 0, 0, 0) && pixel4 == Color.FromArgb(255, 0, 0, 0) ||
                        pixel1 != pixel2 && pixel2 == Color.FromArgb(255, 0, 0, 0) && pixel3 == pixel1 && pixel4 == Color.FromArgb(255, 0, 0, 0) ||
                        pixel1 == pixel2 && pixel2 == pixel3 && pixel3 != pixel4 && pixel4 == Color.FromArgb(255, 0, 0, 0) ||
                        pixel1 == pixel4 && pixel2 == pixel3 && pixel3 == Color.FromArgb(255, 0, 0, 0) && pixel4 != pixel3)
                    {
                        lines_picture.SetPixel(x, y, Color.FromArgb(255, 255, 255, 255)); //назначаем белый цвет пикселя
                        if (y == height - 2 && pixel3 != Color.FromArgb(255, 0, 0, 0))
                        {
                            lines_picture.SetPixel(x, y + 1, Color.FromArgb(255, 255, 255, 255));
                        }
                        if (x == width - 2 && pixel2 != Color.FromArgb(255, 0, 0, 0))
                        {
                            lines_picture.SetPixel(x + 1, y, Color.FromArgb(255, 255, 255, 255));
                        }
                        if (y == height - 2 && x == width - 2 && pixel4 != Color.FromArgb(255, 0, 0, 0))
                        {
                            lines_picture.SetPixel(x + 1, y + 1, Color.FromArgb(255, 255, 255, 255));
                        }
                    }
                    if (pixel1 != pixel2 && pixel1 != Color.FromArgb(255, 0, 0, 0) && pixel2 != Color.FromArgb(255, 0, 0, 0))
                    {
                        lines_picture.SetPixel(x, y, Color.FromArgb(255, linego));
                        lines_picture.SetPixel(x + 1, y, Color.FromArgb(255, linego));
                    }
                    if (pixel1 != pixel3 && pixel1 != Color.FromArgb(255, 0, 0, 0) && pixel3 != Color.FromArgb(255, 0, 0, 0))
                    {
                        lines_picture.SetPixel(x, y, Color.FromArgb(255, linego));
                        lines_picture.SetPixel(x, y + 1, Color.FromArgb(255, linego));
                    }
                    if (pixel1 != pixel4 && pixel1 != Color.FromArgb(255, 0, 0, 0) && pixel4 != Color.FromArgb(255, 0, 0, 0))
                    {
                        lines_picture.SetPixel(x, y, Color.FromArgb(255, linego));
                        lines_picture.SetPixel(x + 1, y + 1, Color.FromArgb(255, linego));
                    }
                }
            }
            return lines_picture;
        }

        public static Bitmap filter_text(Bitmap lines_image, Bitmap drawed_picture, Color line_color, int font_size) //подписываем номера цветов на раскраске; 1 - раскраска для подписи, 2 - раскрашенное изображение, 3 - номер цвета для подписи, 4 - размер шрифта
        {
            Bitmap image_with_numbers = lines_image;
            int countgo = 6;
            int height = lines_image.Height;
            int width = lines_image.Width;
            bool[,] matrix = new bool[width, height];
            int minusx = (font_size / 3) + 5;
            int minusy = (font_size / 4) + 7;
            int dlinax = 5 * (font_size / 7);
            //progressBar2.Maximum = he - 1;

            for (int y = 0; y < height; y++) //изображение
            {
                for (int x = 0; x < width; x++) //изображение
                {
                    matrix[x, y] = false;  
                }
            }
            for (int y = 0; y < height; y++) //изображение
            {             
                for (int x = 0; x < width; x++) //изображение
                {
                    Color pixel = lines_image.GetPixel(x, y);
                    if (matrix[x, y] == false && pixel == Color.FromArgb(255, 255, 255, 255))
                    {
                        int fpointlong = 0; int spointlong = 0; //Вправо и вниз
                        int fpointlong2 = 0; int spointlong2 = 0; // Влево и вверх
                        int xx = x; int yy = y;
                        int bufx = xx; int bufy = yy;
                        bool trigger = false;
                        for (int i = 0; i < countgo; i++)
                        {
                            if (trigger == true)
                            {
                                break;
                            }
                            bool stop = false;
                            int testlong = 0;
                            while (stop == false)    //считаем белые пиксели вправо
                            {
                                xx = xx + 1;
                                testlong = testlong + 1;
                                if (yy >= height - 1 || yy < 0 || xx >= width - 1 || xx < 0)
                                {
                                    xx = xx - 1;
                                    stop = true;

                                    if (fpointlong < testlong - 1)
                                    {
                                        fpointlong = testlong - 1;
                                    }
                                }
                                else
                                {
                                    if (matrix[xx, yy] == true)
                                    {
                                        trigger = true;
                                        break;
                                    }
                                    else
                                    {
                                        Color pixelcheck = lines_image.GetPixel(xx, yy);
                                        // thisbitmap.SetPixel(xx, yy, Color.FromArgb(255, 0, 255, 0)); //////////////////////
                                        if (pixelcheck == line_color)
                                        {
                                            xx = xx - 1;
                                            stop = true;
                                            if (fpointlong < testlong - 1)
                                            {
                                                fpointlong = testlong - 1;

                                            }
                                        }
                                    }
                                }

                            }
                            if (trigger == true)
                            {
                                break;
                            }
                            stop = false;
                            xx = bufx; testlong = 0;
                            while (stop == false) //считаем белые пиксели влево
                            {
                                xx = xx - 1;
                                testlong = testlong + 1;
                                if (xx >= width - 1 || xx < 1 || yy >= height - 1 || yy < 0)
                                {
                                    xx = xx + 1;
                                    stop = true;
                                    if (fpointlong2 < testlong - 1)
                                    {
                                        fpointlong2 = testlong - 1;
                                    }
                                }
                                else
                                {
                                    if (matrix[xx, yy] == true)
                                    {
                                        trigger = true;
                                        break;
                                    }
                                    else
                                    {
                                        Color pixelcheck = lines_image.GetPixel(xx, yy);
                                        if (pixelcheck == line_color)
                                        {
                                            xx = xx + 1;
                                            stop = true;
                                            if (fpointlong2 < testlong - 1)
                                            {
                                                fpointlong2 = testlong - 1;

                                            }
                                        }
                                    }
                                }
                            }
                            if (trigger == true)
                            {
                                break;
                            }
                            stop = false;
                            fpointlong = ((fpointlong - fpointlong2) / 2) + 1;
                            xx = bufx + fpointlong;
                            if (xx >= width)
                            { xx = width - 1; }
                            bufx = xx;
                            testlong = 0;
                            while (stop == false)    //считаем белые пиксели вниз
                            {
                                yy = yy + 1;
                                testlong = testlong + 1;
                                if (yy >= height - 1 || yy < 0 || xx >= width - 1 || xx < 1)
                                {
                                    stop = true;
                                    yy = yy - 1;
                                    if (spointlong < testlong - 1)
                                    {
                                        spointlong = testlong - 1;
                                    }
                                }
                                else
                                {
                                    if (matrix[xx, yy] == true)
                                    {
                                        trigger = true;
                                        break;
                                    }
                                    else
                                    {
                                        Color pixelcheck = lines_image.GetPixel(xx, yy);
                                        //thisbitmap.SetPixel(xx, yy, Color.FromArgb(255, 0, 255, 0)); ////////////////
                                        if (pixelcheck == line_color)
                                        {
                                            yy = yy - 1;
                                            stop = true;
                                            if (spointlong < testlong - 1)
                                            {
                                                spointlong = testlong - 1;

                                            }
                                        }
                                    }
                                }

                            }
                            if (trigger == true)
                            {
                                break;
                            }
                            stop = false;
                            yy = bufy; testlong = 0;
                            while (stop == false) //считаем белые пиксели вверх
                            {
                                yy = yy - 1;
                                testlong = testlong - 1;
                                if (yy >= height - 1 || yy < 1 || xx >= width - 1 || xx < 1)
                                {
                                    yy = yy + 1;
                                    stop = true;
                                }
                                else
                                {
                                    if (matrix[xx, yy] == true)
                                    {
                                        trigger = true;
                                        break;
                                    }
                                    else
                                    {
                                        Color pixelcheck = lines_image.GetPixel(xx, yy);
                                        if (pixelcheck == line_color)
                                        {
                                            yy = yy + 1;
                                            stop = true;
                                            if (spointlong2 < testlong - 1)
                                            {
                                                spointlong2 = testlong - 1;

                                            }
                                        }
                                    }
                                }
                            }
                            if (trigger == true)
                            {
                                break;
                            }
                            stop = false;
                            spointlong = ((spointlong - spointlong2) / 2) + 1;
                            yy = bufy + spointlong;
                            if (yy >= height)
                            { yy = height - 1; }
                            bufy = yy;
                        }
                        bool gol = false;
                        if (trigger == false)
                        {
                            bool trigger2 = false; //если false, то напечатаем номер цвета
                            int x3 = bufx - (minusx + dlinax + 3); int y3 = bufy - ((minusy / 2) + 3); // 3 - примерная ширина символа, 7 - примерная высота символа. проверяем чтобы число цвета влезло в поле
                            for (int i = x3; i < bufx + (dlinax * 2) + 3; i++) // рассчитываем на 3 цифры, поэтому dlinax*3 (dlina + 2*dlina)
                            {
                                for (int z = y3; z < bufy + ((minusy / 2) + font_size); z++) //рассчитываем на высоту символа ; нужно проверить высоту символов!
                                {
                                    if (i >= width || z >= height || i < 0 || z < 0)
                                    {
                                        trigger2 = true;
                                        break;
                                    }
                                    if (lines_image.GetPixel(i, z) == line_color)
                                    {
                                        trigger2 = true;
                                        break;
                                    }
                                }
                                if (trigger2 == true)
                                {
                                    break;
                                }
                            }
                            if (trigger2 == false)
                            {
                                Color paint = drawed_picture.GetPixel(bufx, bufy); //берем цвет из раскрашенного рисунка
                                int ind = 0;
                                for (int i = 0; i < Form1.Palitra[Form1.usespalitra].Count; i++) //ищем цвет из палитры
                                {
                                    if (paint.A == Form1.Palitra[Form1.usespalitra][i].A && paint.R == Form1.Palitra[Form1.usespalitra][i].R && paint.G == Form1.Palitra[Form1.usespalitra][i].G && paint.B == Form1.Palitra[Form1.usespalitra][i].B)
                                    {
                                        ind = i;
                                        break;
                                    }

                                }
                                Brush brushline = new SolidBrush(line_color);
                                gol = true;
                                Font myFont = new Font("Arial", font_size);
                                Graphics g = Graphics.FromImage(image_with_numbers);
                                g.DrawString(Convert.ToString(ind + 1), myFont, brushline, bufx - (minusx + dlinax), bufy - (minusy / 2)); //пишем цифры
                                                                                                                                           //thisbitmap.SetPixel(bufx - minusx, bufy - (minusy / 2), Color.FromArgb(255, 0, 0, 255)); //тест для проверки места создания подписи
                            }
                        }
                        bool st = false;
                        if (gol == true)
                        {
                            gol = false;
                            int x2 = bufx; int y2 = bufy;
                            while (st == false) //заполняем матрицу запрещенных пикселей для повторного использования
                            {
                                bool st2 = false;
                                y2 = bufy;
                                Color pixelcheck;
                                if (x2 >= 0 && x2 < width && y2 >= 0 && y2 < height)
                                {
                                    pixelcheck = lines_image.GetPixel(x2, y2);
                                }
                                else
                                { break; }
                                if (pixelcheck == line_color)
                                {
                                    st = true;
                                }
                                else
                                {
                                    matrix[x2, y2] = true;
                                    while (st2 == false)
                                    {
                                        y2 = y2 + 1;
                                        if (x2 >= 0 && x2 < width && y2 >= 0 && y2 < height)
                                        {
                                            pixelcheck = lines_image.GetPixel(x2, y2);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        if (pixelcheck == line_color)
                                        {
                                            st2 = true;
                                        }
                                        else
                                        {
                                            matrix[x2, y2] = true;
                                        }
                                    }
                                    y2 = bufy;
                                    st2 = false;
                                    while (st2 == false)
                                    {
                                        y2 = y2 - 1;
                                        if (x2 >= 0 && x2 < width && y2 >= 0 && y2 < height)
                                        {
                                            pixelcheck = lines_image.GetPixel(x2, y2);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        if (pixelcheck == line_color)
                                        {
                                            st2 = true;
                                        }
                                        else
                                        {
                                            matrix[x2, y2] = true;
                                        }
                                    }
                                }
                                y2 = bufy;
                                x2 = x2 + 1;
                                if (x2 >= 0 && x2 < width && y2 >= 0 && y2 < height)
                                {
                                    pixelcheck = lines_image.GetPixel(x2, y2);
                                    if (pixelcheck == line_color)
                                    {
                                        st = false;
                                    }
                                }
                                else
                                {
                                    st = true;
                                }
                            }
                            st = false;
                            x2 = bufx;
                            y2 = bufy;
                            while (st == false) //заполняем матрицу запрещенных пикселей для повторного использования
                            {
                                bool st2 = false;
                                y2 = bufy;
                                Color pixelcheck;
                                if (x2 >= 0 && x2 < width && y2 >= 0 && y2 < height)
                                {
                                    pixelcheck = lines_image.GetPixel(x2, y2);
                                }
                                else
                                { break; }
                                if (pixelcheck == line_color)
                                {
                                    st = true;
                                }
                                else
                                {
                                    matrix[x2, y2] = true;
                                    while (st2 == false)
                                    {
                                        y2 = y2 + 1;
                                        if (x2 >= 0 && x2 < width && y2 >= 0 && y2 < height)
                                        {
                                            pixelcheck = lines_image.GetPixel(x2, y2);
                                        }
                                        else
                                        {
                                            //y2 = y2 - 1;
                                            break;
                                        }
                                        if (pixelcheck == line_color)
                                        {
                                            st2 = true;
                                        }
                                        else
                                        {
                                            matrix[x2, y2] = true;
                                        }
                                    }
                                    y2 = bufy;
                                    st2 = false;
                                    while (st2 == false)
                                    {
                                        y2 = y2 - 1;
                                        if (x2 >= 0 && x2 < width && y2 >= 0 && y2 < height)
                                        {
                                            pixelcheck = lines_image.GetPixel(x2, y2);
                                        }
                                        else
                                        {
                                            y2 = y2 + 1;
                                            break;
                                        }
                                        if (pixelcheck == line_color)
                                        {
                                            st2 = true;
                                        }
                                        else
                                        {
                                            matrix[x2, y2] = true;
                                        }
                                    }
                                }
                                x2 = x2 - 1;
                                if (x2 >= 0 && x2 < width && y2 >= 0 && y2 < height)
                                {
                                    pixelcheck = lines_image.GetPixel(x2, y2);
                                    if (pixelcheck == line_color)
                                    {
                                        st = false;
                                    }
                                }
                                else
                                {
                                    st = true;
                                }
                            }
                        }
                    }
                }


            }
            return image_with_numbers;
        }
    }
}
