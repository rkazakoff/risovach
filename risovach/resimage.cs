using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace risovach
{
    public class resimage
    {
        static public int height_res; //высота уменьшенного итогового изображения
        static public int width_res; //ширина уменьшенного итогового изображения
        static public int core = Environment.ProcessorCount - 1; //оставляем одно ядро пустым для стабильной работы системы
        static public int partx; //часть изображения по x для разделения по потокам
        static public int partgo; //какой номер потока запускается
        static public bool got; //триггер выполнения

        public static void res_thread() //функция запускается в новом потоке и меняет разрешение изображения
        {
            double xk = (double)Form1.shir / width_res;
            double yk = (double)Form1.vys / height_res;
            Bitmap bmp = new Bitmap(width_res, height_res);
            Bitmap bmp2 = new Bitmap(Form1.image_orig);
            int bmp2Width = bmp2.Width;
            int bmp2Height = bmp2.Height;
            int n = partgo;
            got = true;
            int x1;
            int x2;
            if (n + 1 == core)
            {
                x1 = (core - 1) * partx;
                x2 = width_res;
            }
            else
            {
                x1 = n * partx;
                x2 = (n + 1) * partx;
            }
            int xgo = 0; int ygo = 0;
            for (int x = x1; x < x2; x++)
            {
                for (int y = 0; y < height_res; y++)
                {
                    xgo = Convert.ToInt32(x * xk);
                    bool checksh = false;
                    while (checksh == false)
                    {
                        if (xgo > bmp2Width)
                        {
                            xgo = xgo - 1;
                        }
                        else
                        {
                            checksh = true;
                        }
                    }
                    checksh = false;
                    ygo = Convert.ToInt32(y * yk);
                    while (checksh == false)
                    {
                        if (ygo > bmp2Height)
                        {
                            ygo = ygo - 1;
                        }
                        else
                        {
                            checksh = true;
                        }
                    }
                    bmp.SetPixel(x, y, bmp2.GetPixel(xgo, ygo));
                }
            }
            bool trigm = false;
            while (trigm == false)
            {
                if (n > 0)
                {
                    if (Form1.t[n - 1] == true)
                    { trigm = true; }
                }
                else
                {
                    trigm = true;
                }
            }
            for (int x = x1; x < x2; x++)
            {
                for (int y = 0; y < height_res; y++)
                {
                    Form1.resized_image.SetPixel(x, y, bmp.GetPixel(x, y));
                }
            }
            Form1.t[n] = true;
            bmp.Dispose();
            bmp2.Dispose();          
        }
    }
}
