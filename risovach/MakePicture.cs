using System;
using System.Collections.Generic;
using System.Drawing;


namespace risovach
{
    public class MakePicture
    {

        public struct Colors_count //структура для подсчета количества цветов
        {
            public byte A, R, G, B;
            public int n;
            public Colors_count(byte Alpha, byte Red, byte Green, byte Blue, int Count)
            {
                this.A = Alpha;
                this.R = Red;
                this.G = Green;
                this.B = Blue;
                this.n = Count;

            }
        }

        static public int core = Environment.ProcessorCount - 1; //оставляем одно ядро пустым для стабильной работы системы
        static public int partx; //часть изображения по x для разделения по потокам
        static public int partgo; //какой номер потока запускается
        static public bool got; //триггер выполнения
        static public int count; //количество итераций фильтра

        private static double color_delta(int x, byte Alpha, byte Red, Byte Green, Byte Blue) //расстояние до цвета
        {
            double deltaP = Math.Sqrt(Math.Pow((Form1.CurrentPalitra[x].A - Alpha), 2) + Math.Pow((Form1.CurrentPalitra[x].R - Red), 2) + Math.Pow((Form1.CurrentPalitra[x].G - Green), 2) + Math.Pow((Form1.CurrentPalitra[x].B - Blue), 2));
            return deltaP;
        }
        public static void makepict_thread() //поток для создания разукрашенного изображения
        {
            Bitmap received_image = new Bitmap(Form1.resized_image);
            Bitmap image_out = new Bitmap(Form1.resized_image);
            int n = partgo;
            got = true;
            int x1;
            int x2;
            int widht = received_image.Width;
            int height = received_image.Height;
            if (n + 1 == core)
            {
                x1 = (core - 1) * partx;
                x2 = widht;
            }
            else
            {
                x1 = n * partx;
                x2 = (n + 1) * partx;
            }
            for (int y = 0; y < height; y++) //обрабатываю изображение для уменьшения кол-ва цветов
            {
                for (int x = x1; x < x2; x++)
                {
                    Color pixel = received_image.GetPixel(x, y);
                    byte a = pixel.A;
                    byte r = pixel.R;
                    byte g = pixel.G;
                    byte b = pixel.B;
                    double delta = color_delta(0, a, r, g, b); //берем первый цвет из палитры, чтобы было с чем потом сравнить в цикле
                    double bufferdelta;
                    int num = 0;
                    for (int k = 1; k < Form1.CurrentPalitra.Count; k++) //если в палитре только один цвет, то цикл начаться не должен
                    {
                        bufferdelta = color_delta(k, a, r, g, b);
                        if (bufferdelta < delta) //Если дельта меньше, то этот цвет более подходит
                        {
                            delta = color_delta(k, a, r, g, b);
                            num = k; //запоминаем номер цвета из палитры
                        }
                    }
                    image_out.SetPixel(x, y, Color.FromArgb(Form1.CurrentPalitra[num].A, Form1.CurrentPalitra[num].R, Form1.CurrentPalitra[num].G, Form1.CurrentPalitra[num].B));
                }
            }
            Color checkpixel;
            Color checkpixelimage;
            bool trigm = false;
            while (trigm == false && Form1.trig_thread_end == false)
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

            for (int y = 0; y < height; y++) //изображение
            {
                for (int x = x1; x < x2; x++) //изображение
                {
                    checkpixel = image_out.GetPixel(x, y);
                    for (int k = 0; k < Form1.CurrentPalitra.Count; k++) //если в палитре только один цвет, то цикл начаться не должен; возможно ошибка!
                    {
                        checkpixelimage = Color.FromArgb(Form1.CurrentPalitra[k].A, Form1.CurrentPalitra[k].R, Form1.CurrentPalitra[k].G, Form1.CurrentPalitra[k].B);
                        if (checkpixel == checkpixelimage)
                        {
                            Form1.realizecolor[Form1.cur][k] = true;
                            break;
                        }
                    }
                }
            }
            for (int y = 0; y < height; y++) //изображение
            {
                for (int x = x1; x < x2; x++) //изображение
                {
                    Form1.drawed_picture.SetPixel(x, y, image_out.GetPixel(x, y));
                }
            }
            received_image.Dispose();
            image_out.Dispose();
            Form1.t[n] = true;
        }

        public static void filter_cube() //квадратный фильтр, если Form1.mode = 0, то используется 2х2, если =1, то 4х4, если другое, то берется значение из ползунка и умножается на два
        {           
            Bitmap image_out = new Bitmap(Form1.drawed_picture);
            Bitmap image_cache = new Bitmap(Form1.drawed_picture);
            List<byte> Alpha = new List<byte>();
            List<byte> Red = new List<byte>();
            List<byte> Green = new List<byte>();
            List<byte> Blue = new List<byte>();
            int count;
            if (Form1.mode == 0)
            {
                count = 2;
            }
            else
            {
                if (Form1.mode == 1)
                {
                    count = 4;
                }
                else
                {
                    //count = 0;
                    count = Form1.trackbar_cache_value * 2;
                }
            }
            int height = image_cache.Height;
            int widht = image_cache.Width;
            int n = partgo; //записываем номер этого потока
            got = true; //уведомляем, что записали номер этого потока
            int x1;
            int x2;
            if (n + 1 == core)
            {
                x1 = (core - 1) * partx;
                x2 = widht;
            }
            else
            {
                x1 = n * partx;
                x2 = (n + 1) * partx;
            }
            List<Colors_count> color_array = new List<Colors_count>();
            for (int y = 0; y < height; y++) //изображение
            {
                for (int x = x1; x < x2; x++) //изображение
                {
                    int eee = Alpha.Count;
                    for (int tt = 0; tt < eee; tt++)
                    {
                        Alpha.RemoveAt(0);
                        Red.RemoveAt(0);
                        Blue.RemoveAt(0);
                        Green.RemoveAt(0);
                    }
                    for (int z = y - count / 2; z < y + count / 2; z++) //фильтр
                    {
                        for (int l = x - count / 2; l < x + count / 2; l++) //фильтр
                        {
                            if (z >= 0 && l >= 0 && z < height && l < widht)
                            {
                                Color pixel = image_cache.GetPixel(l, z);
                                Alpha.Add(pixel.A);
                                Red.Add(pixel.R);
                                Green.Add(pixel.G);
                                Blue.Add(pixel.B);

                            }

                        }
                    }
                    int jjj = color_array.Count;
                    for (int tt = 0; tt < jjj; tt++)
                    {
                        color_array.RemoveAt(0);
                    }
                    color_array.Add(new Colors_count(Alpha[0], Red[0], Green[0], Blue[0], 0));
                    for (int o = 0; o < Alpha.Count; o++)
                    {
                        bool metka = false;
                        for (int q = 0; q < color_array.Count; q++)
                        {
                            if (color_array[q].A == Alpha[o] && color_array[q].R == Red[o] && color_array[q].G == Green[o] && color_array[q].B == Blue[o])
                            {
                                metka = true;
                                int numbb = color_array[q].n + 1;
                                color_array.RemoveAt(q);
                                color_array.Add(new Colors_count(Alpha[o], Red[o], Green[o], Blue[o], numbb));
                                break;
                            }

                        }
                        if (metka == false)
                        {
                            color_array.Add(new Colors_count(Alpha[o], Red[o], Green[o], Blue[o], 0));
                        }
                    }
                    int skolko = color_array[0].n;
                    int nomer = 0;
                    for (int o = 1; o < color_array.Count; o++) //ищем пиксель, который встречается в квадрате чаще всего
                    {
                        if (skolko < color_array[o].n)
                        {
                            skolko = color_array[o].n;
                            nomer = o;
                        }
                    }
                    image_out.SetPixel(x, y, Color.FromArgb(color_array[nomer].A, color_array[nomer].R, color_array[nomer].G, color_array[nomer].B));
                }
            }
            bool trigm = false;
            if (n == core - 1)
            {
                Form1.trig_thread_end = true;
            }
            while (trigm == false || Form1.trig_thread_end == false)
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
            for (int y = 0; y < height; y++) //изображение
            {
                for (int x = x1; x < x2; x++) //изображение
                {
                    Form1.drawed_picture.SetPixel(x, y, image_out.GetPixel(x, y));
                }
            }
            Form1.t[n] = true;
            image_cache.Dispose();
            image_out.Dispose();
        }
    }
}
