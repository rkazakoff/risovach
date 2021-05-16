using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace risovach
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            Program.f1 = this;
            InitializeComponent();
            getsettings();
            int wl = 40; int hl = 20; //ширина и высота образца
            Bitmap line_example1 = new Bitmap(wl, hl);
            Bitmap line_example2 = new Bitmap(wl, hl);
            Bitmap line_example3 = new Bitmap(wl, hl);
            for (int i = 0; i < wl; i++)
            {
                for (int y = 0; y < hl; y++)
                {
                    line_example1.SetPixel(i, y, Color.FromArgb(255, 255, 255, 255));
                    line_example2.SetPixel(i, y, Color.FromArgb(255, 255, 255, 255));
                    line_example3.SetPixel(i, y, Color.FromArgb(255, 255, 255, 255));
                }
            }
            for (int x = 0; x < wl; x++) //для первого образца
            {
                line_example1.SetPixel(x, hl / 2, line1);
                line_example1.SetPixel(x, (hl / 2) + 1, line1);
            }
            pictureBoxline1.Image = line_example1;

            for (int x = 0; x < wl; x++) //для второго образца
            {
                line_example2.SetPixel(x, hl / 2, line2);
                line_example2.SetPixel(x, (hl / 2) + 1, line2);
            }
            pictureBoxline2.Image = line_example2;

            for (int x = 0; x < wl; x++) //для третьего образца
            {
                line_example3.SetPixel(x, hl / 2, line3);
                line_example3.SetPixel(x, (hl / 2) + 1, line3);
            }
            pictureBoxline3.Image = line_example3;

            string[] second = Directory.GetFiles(@"./Colors"); // путь к папке

            int sk = 0;
            bool launchr = false;
            while (sk < second.Length)
            {

                trackbar_filter_max.Text = Convert.ToString(trackbar_filter_cube.Maximum * 2);
                filter_count1.Text = Convert.ToString(trackbar_filter_cube.Value * 2);
                label_count.Text = Convert.ToString(trackBar_count.Value);
                OpenFileDialog open_dialog2 = new OpenFileDialog();
                open_dialog2.Filter = "JSON Files(*.JSON;*.TXT)|*.JSON;*.TXT|All files (*.*)|*.*"; //формат загружаемого файла
                                                                                                   //Colorsave Colorload = new Colorsave();
                string Colorloadd;
                string colorll;   
                try
                {
                    Palitra.Add(new List<Colors>());
                    realizecolor.Add(new List<bool>());
                    TextReader tr = new StreamReader(second[ii]);
                    // read a line of text
                    colorll = tr.ReadLine();
                    Colorloadd = colorll;
                    JArray ColorArray = JArray.Parse(Colorloadd);
                    IList<Colorsave> ColorLoadd = ColorArray.Select(p => new Colorsave
                    {
                        Name = (string)p["Name"],
                        Number = (int)p["Number"],
                        Alpha = (byte)p["Alpha"],
                        Red = (byte)p["Red"],
                        Blue = (byte)p["Blue"],
                        Green = (byte)p["Green"]
                    }).ToList();

                    int kol = Palitra[sk].Count;
                    for (int r = 0; r < kol; r++)
                    {
                        Palitra[ii].RemoveAt(0);
                    }
                    for (int r = 0; r < ColorLoadd.Count; r++)
                    {
                        addcolor(ii, ColorLoadd[r].Name, ColorLoadd[r].Alpha, ColorLoadd[r].Red, ColorLoadd[r].Green, ColorLoadd[r].Blue);
                    }
                    listBox_colors.Items.Add(second[sk]);
                    ii++;
                    launchr = true;
                    refreshlist();
                }
                catch
                {
                    DialogResult rezult = MessageBox.Show("Не удалось загрузить палитру " + second[sk],
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                sk++;
            }
            cur = 0;
            listBox_colors.SelectedIndex = 0;
            if (launchr == true)
            {
                refreshcurrentpalitra();
            }
        }
        Color line1 = Color.FromArgb(255, 0, 0, 0);
        Color line2 = Color.FromArgb(255, 160, 160, 160);
        Color line3 = Color.FromArgb(255, 220, 220, 220);


        int time_interval = 1; //интервал задержки в мс для проверки выполнения потоков

        public static int usespalitra;
        public static List<List<bool>> realizecolor = new List<List<bool>>();
        public static int cur = 0; //выбранная палитра
        int ii = 0; //сколько палитр
        public bool filt_on = true; // будет ли работать фильтр
        public static int widht = 0; //размер
        public static int height = 0; //размер
        public struct Colors //структура для палитры
        {
            public byte A, R, G, B;
            public string N;
            public Colors(string Name, byte Alpha, byte Red, byte Green, byte Blue)
            {
                this.A = Alpha;
                this.R = Red;
                this.G = Green;
                this.B = Blue;
                this.N = Name;
            }
        }
        struct Colors_count //структура для подсчета количества цветов
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
        public static List<Colors> CurrentPalitra = new List<Colors>(); //создаем палитру, которая будет использоваться на данный момент работать с Palitra как с массивом
        public static List<List<Colors>> Palitra = new List<List<Colors>>(); //коллекция палитр

        void refreshcurrentpalitra()
        {
            clear_current_palitra();
            for (int i = 0; i < Palitra[cur].Count; i++)
            {
                CurrentPalitra.Add(new Colors(Palitra[cur][i].N, Palitra[cur][i].A, Palitra[cur][i].R, Palitra[cur][i].G, Palitra[cur][i].B));
            }
            refreshlist();
        }
        void getsettings()
        {
            if (Properties.Settings.Default.numberset == false)
            {
                checkBoxnumbers.CheckState = CheckState.Unchecked;
            }
            else
            {
                checkBoxnumbers.CheckState = CheckState.Checked;
            }
            if (Properties.Settings.Default.colornameset == false)
            {
                palitraname_check.CheckState = CheckState.Unchecked;
            }
            else
            {
                palitraname_check.CheckState = CheckState.Checked;
            }
            if (Properties.Settings.Default.numbercolorset == false)
            {
                say_count_colors.CheckState = CheckState.Unchecked;
            }
            else
            {
                say_count_colors.CheckState = CheckState.Checked;
            }
            
            if (Properties.Settings.Default.onlyusecolorset == false)
            {
                only_uses_color_check.CheckState = CheckState.Unchecked;
            }
            else
            {
                only_uses_color_check.CheckState = CheckState.Checked;
            }
            textsize.Text = Convert.ToString(Properties.Settings.Default.sizenumber);
        }
        void savesettings()
        {
            if ( checkBoxnumbers.CheckState == CheckState.Unchecked)
            {
                Properties.Settings.Default.numberset = false;
            }
            else
            {
                Properties.Settings.Default.numberset = true;
            }
            if (palitraname_check.CheckState == CheckState.Unchecked)
            {
                Properties.Settings.Default.colornameset = false;
            }
            else
            {
                Properties.Settings.Default.colornameset = true;
            }
            if (say_count_colors.CheckState == CheckState.Unchecked)
            {
                Properties.Settings.Default.numbercolorset = false;
            }
            else
            {
                Properties.Settings.Default.numbercolorset = true;
            }
            if (only_uses_color_check.CheckState == CheckState.Unchecked)
            {
                Properties.Settings.Default.onlyusecolorset = false;
            }
            else
            {
                Properties.Settings.Default.onlyusecolorset = true;
            }
            try
            {
                
                Properties.Settings.Default.sizenumber = Convert.ToInt32(textsize.Text);
            }
            catch
            {
                MessageBox.Show("Ошибка при сохранении размера символов. Проверьте правильность ввода");
            }

            Properties.Settings.Default.Save();
        }


            void refreshlist() //обновляем список палитры
        {
            textBox_Palitra.Text = "";
            string buffertext = " ";
            for (int i = 0; i < CurrentPalitra.Count; i++) //для каждого цвета палитры
            {
                int number = i + 1;
                buffertext += "№:" + number + " ";
                if (CurrentPalitra[i].N == "" || CurrentPalitra[i].N == " " || CurrentPalitra[i].N == null)
                {
                    buffertext += "A:"; //Выводим альфу
                    buffertext += Convert.ToString(CurrentPalitra[i].A);
                    buffertext += ", ";
                    buffertext += "R:"; //Красный
                    buffertext += Convert.ToString(CurrentPalitra[i].R);
                    buffertext += ", ";
                    buffertext += "G:"; //Зеленый
                    buffertext += Convert.ToString(CurrentPalitra[i].G);
                    buffertext += ", ";
                    buffertext += "B:"; //Синий
                    buffertext += Convert.ToString(CurrentPalitra[i].B);
                    buffertext += "\n ";
                }
                else
                {
                    buffertext += CurrentPalitra[i].N;
                    buffertext += "\n ";
                }

            }
            if (CurrentPalitra.Count > 0)
            {
                ColorSaveToolStripMenuItem.Enabled = true;
            }
            else
            {
                ColorSaveToolStripMenuItem.Enabled = false;
            }
            textBox_Palitra.Text = buffertext;
        }

        void addcolor(int n, string Name, byte Alpha, byte Red, byte Green, byte Blue) //функция добавления цвета в палитру
        {
            bool have_here = false;
            for (int i = 0; i < Palitra[n].Count; i++)
            {
                if (Palitra[n][i].A == Alpha && Palitra[n][i].R == Red && Palitra[n][i].G == Green && Palitra[n][i].B == Blue)
                {
                    have_here = true;
                }
            }
            if (Red == Convert.ToByte(255) && Green == Convert.ToByte(255) && Blue == Convert.ToByte(255)
                    || Red == Convert.ToByte(0) && Green == Convert.ToByte(0) && Blue == Convert.ToByte(0))
            {
                have_here = true;
            }
            if (have_here == false)
            {
                Palitra[n].Add(new Colors(Name, Alpha, Red, Green, Blue));
                realizecolor[n].Add(false);
                //refreshlist();
            }
        }

        bool trigfilt = false;
        public bool trigmakepaint = false;

        public void dis_all() //не работает?
        {

            Action action1337 = () => this.ControlBox = false; //выключить интерфейс Палитра
            if (this.InvokeRequired) this.Invoke(action1337);
            else this.ControlBox = false;

            Action action1 = () => Colors_edit.Enabled = false; //выключить интерфейс Палитра
            if (Colors_edit.InvokeRequired) Colors_edit.Invoke(action1);
            else Colors_edit.Enabled = false;

            Action action2 = () => Param_paint.Enabled = false; //выключить интерфейс Параметры раскраски
            if (Param_paint.InvokeRequired) Param_paint.Invoke(action2);
            else Param_paint.Enabled = false;

            Action action3 = () => button_do.Enabled = false; //выключить кнопку преобразовать
            if (button_do.InvokeRequired) button_do.Invoke(action3);
            else button_do.Enabled = false;

            if (checkBox_filter.Enabled == true)
            {

                trigfilt = true;
                Action action4 = () => checkBox_filter.Enabled = false; //выключить галочку Исп. фильтр
                if (checkBox_filter.InvokeRequired) checkBox_filter.Invoke(action4);
                else checkBox_filter.Enabled = false;

            }
            else
            {
                trigfilt = false;
            }

            if (button_do2.Enabled == true)
            {
                trigmakepaint = true;
                Action action5 = () => button_do2.Enabled = false; //выключить кнопку Раскрасить
                if (button_do2.InvokeRequired) button_do2.Invoke(action5);
                else button_do2.Enabled = false;
            }
            else
            {
                trigmakepaint = false;
            }

            Action action6 = () => tabControl1.Enabled = false; //выключить вкладки с изображениями
            if (tabControl1.InvokeRequired) tabControl1.Invoke(action6);
            else tabControl1.Enabled = false;

            Action action7 = () => menuStrip1.Enabled = false; //выключить меню
            if (menuStrip1.InvokeRequired) menuStrip1.Invoke(action7);
            else menuStrip1.Enabled = false;

            Action action8 = () => pictureBox_resized_image.Visible = false; //выключить 1 изображение
            if (pictureBox_resized_image.InvokeRequired) pictureBox_resized_image.Invoke(action8);
            else pictureBox_resized_image.Visible = false;

            Action action9 = () => pictureBox_Drawn_image.Visible = false; //выключить 2 изображение
            if (pictureBox_Drawn_image.InvokeRequired) pictureBox_Drawn_image.Invoke(action9);
            else pictureBox_Drawn_image.Visible = false;

            Action action10 = () => pictureBox_with_line.Visible = false; //выключить 3 изображение
            if (pictureBox_with_line.InvokeRequired) pictureBox_with_line.Invoke(action10);
            else pictureBox_with_line.Visible = false;

            Action action100 = () => pictureBox_orig.Visible = false; //выключить оригинальное изображение
            if (pictureBox_orig.InvokeRequired) pictureBox_orig.Invoke(action100);
            else pictureBox_orig.Visible = false;

            Action action11 = () => filter.Enabled = false; //выключить настройки фильтров
            if (filter.InvokeRequired) filter.Invoke(action11);
            else filter.Enabled = false;

            Action action12 = () => groupBox_resize.Enabled = false; //выключить настройки фильтров
            if (groupBox_resize.InvokeRequired) groupBox_resize.Invoke(action12);
            else groupBox_resize.Enabled = false;
        }
        public void enab_all()
        {
            Action action1 = () => Colors_edit.Enabled = true; //включить интерфейс Палитра
            if (Colors_edit.InvokeRequired) Colors_edit.Invoke(action1);
            else Colors_edit.Enabled = true;

            Action action2 = () => Param_paint.Enabled = true; //включить интерфейс Параметры раскраски
            if (Param_paint.InvokeRequired) Param_paint.Invoke(action2);
            else Param_paint.Enabled = true;

            Action action3 = () => button_do.Enabled = true; //включить кнопку преобразовать
            if (button_do.InvokeRequired) button_do.Invoke(action3);
            else button_do.Enabled = true;

            if (filt_on == true)
            {
                Action action4 = () => checkBox_filter.Enabled = true; //включить галочку Исп. фильтр
                if (checkBox_filter.InvokeRequired) checkBox_filter.Invoke(action4);
                else checkBox_filter.Enabled = true;
            }
            if (trigmakepaint == true)
            {
                Action action5 = () => button_do2.Enabled = true; //включить кнопку Раскрасить
                if (button_do2.InvokeRequired) button_do2.Invoke(action5);
                else button_do2.Enabled = true;
            }

            Action action6 = () => tabControl1.Enabled = true; //включить вкладки с изображениями
            if (tabControl1.InvokeRequired) tabControl1.Invoke(action6);
            else tabControl1.Enabled = true;

            Action action7 = () => menuStrip1.Enabled = true; //включить меню
            if (menuStrip1.InvokeRequired) menuStrip1.Invoke(action7);
            else menuStrip1.Enabled = true;

            Action action8 = () => pictureBox_resized_image.Visible = true; //включить 1 изображение
            if (pictureBox_resized_image.InvokeRequired) pictureBox_resized_image.Invoke(action8);
            else pictureBox_resized_image.Visible = true;

            Action action9 = () => pictureBox_Drawn_image.Visible = true; //включить 2 изображение
            if (pictureBox_Drawn_image.InvokeRequired) pictureBox_Drawn_image.Invoke(action9);
            else pictureBox_Drawn_image.Visible = true;

            Action action10 = () => pictureBox_with_line.Visible = true; //включить 3 изображение
            if (pictureBox_with_line.InvokeRequired) pictureBox_with_line.Invoke(action10);
            else pictureBox_with_line.Visible = true;

            Action action100 = () => pictureBox_orig.Visible = true; //включить 3 изображение
            if (pictureBox_orig.InvokeRequired) pictureBox_orig.Invoke(action100);
            else pictureBox_orig.Visible = true;

            Action action11 = () => filter.Enabled = true; //включить настройки фильтров
            if (filter.InvokeRequired) filter.Invoke(action11);
            else filter.Enabled = true;

            Action action12 = () => groupBox_resize.Enabled = true; //включить настройки фильтров
            if (groupBox_resize.InvokeRequired) groupBox_resize.Invoke(action12);
            else groupBox_resize.Enabled = true;

            Action action1337 = () => this.ControlBox = true; //выключить интерфейс Палитра
            if (this.InvokeRequired) this.Invoke(action1337);
            else this.ControlBox = true;
        }
        int ct1 = 0, ct2 = 0, ct3 = 0, ct4 = 0, ct5 = 0, ct6 = 0, ct7 = 0, ct8 = 0, ct9 = 0, ct10 = 0, ct11 = 0;
        public void progresstelupd(int numberthr, int currentpr)
        {
            numberthr++;
            switch (numberthr)
            {
                case 1:
                    ct1 = currentpr;
                    break;
                case 2:
                    ct2 = currentpr;
                    break;
                case 3:
                    ct3 = currentpr;
                    break;
                case 4:
                    ct4 = currentpr;
                    break;
                case 5:
                    ct5 = currentpr;
                    break;
                case 6:
                    ct6 = currentpr;
                    break;
                case 7:
                    ct7 = currentpr;
                    break;
                case 8:
                    ct8 = currentpr;
                    break;
                case 9:
                    ct9 = currentpr;
                    break;
                case 10:
                    ct10 = currentpr;
                    break;
                case 11:
                    ct11 = currentpr;
                    break;
                default:
                    break;
            }
        }        

        ////////////////////////////////////////Создаем раскрашенное изображение - начало кода
        static public Bitmap image_orig; //оригинал изображения

        public static int mode = 2; 


        static public Bitmap resized_image; //изображение с уменьшенным разрешением
        static public Bitmap drawed_picture; //картинка, нарисованная по палитре
        static public Bitmap lines_picture; //итоговая раскраска       

        Bitmap imageres2;

        public static bool trig_thread_end; //триггер определения окончания работы последнего потока
        void do_button1_func()
        {
            dis_all(); // отключаем интерфейс

            Action action621 = () => label_status.Text = "Создается рисунок...";
            if (label_status.InvokeRequired) label_status.Invoke(action621);
            else label_status.Text = "Создается рисунок...";

            drawed_picture = new Bitmap(resized_image);
            int counttb = 0;
            Action action1 = () => counttb = trackBar_count.Value;
            if (trackBar_count.InvokeRequired) trackBar_count.Invoke(action1);
            else counttb = trackBar_count.Value;
            height = resized_image.Height;
            widht = resized_image.Width;
            imageres2 = new Bitmap(widht, height); 
            usespalitra = cur;
            Action action11 = () => progressBar1.Maximum = counttb + 1;
            if (progressBar1.InvokeRequired) progressBar1.Invoke(action11);
            else progressBar1.Maximum = counttb + 1;
            for (int t = 0; t < realizecolor[cur].Count; t++)
            {
                realizecolor[cur][t] = false;
            }
            if (CurrentPalitra.Count != 0) //В палитре есть цвета?
            {
                for (int x = 0; x < 12; x++)
                {
                    t[x] = false;
                }
                trig_thread_end = false;
                if (core > 11) //оставляем один поток свободным для стабильной работы системы
                {
                    core = 11;
                }
                if (core < 2) //тут уже ничего не поможет, нагрузим хотя бы все потоки процессора
                {
                    core = 2;
                }
                //core = 2;
                MakePicture.partx = widht / core;
                partx = MakePicture.partx;
                MakePicture.got = false;
                MakePicture.partgo = 0;
                Thread b1 = new Thread(MakePicture.makepict_thread);
                b1.Start();
                if (core > 1)
                {
                    while (MakePicture.got == false)
                    {
                        Thread.Sleep(time_interval);
                    }
                    MakePicture.got = false;
                    MakePicture.partgo = 1;  //потом убрать комментарии
                    Thread b2 = new Thread(MakePicture.makepict_thread);
                    b2.Start();
                }
                if (core > 2)
                {
                    while (MakePicture.got == false)
                    {
                        Thread.Sleep(time_interval);
                    }
                    MakePicture.partgo = 2;
                    MakePicture.got = false;
                    Thread b3 = new Thread(MakePicture.makepict_thread);
                    b3.Start();
                }

                if (core > 3)
                {
                    while (MakePicture.got == false)
                    {
                        Thread.Sleep(time_interval);
                    }
                    MakePicture.partgo = 3;
                    MakePicture.got = false;
                    Thread b4 = new Thread(MakePicture.makepict_thread);
                    b4.Start();
                }

                if (core > 4)
                {
                    while (MakePicture.got == false)
                    {
                        Thread.Sleep(time_interval);
                    }
                    MakePicture.partgo = 4;
                    MakePicture.got = false;
                    Thread b5 = new Thread(MakePicture.makepict_thread);
                    b5.Start();
                }

                if (core > 5)
                {
                    while (MakePicture.got == false)
                    {
                        Thread.Sleep(time_interval);
                    }
                    MakePicture.partgo = 5;
                    MakePicture.got = false;
                    Thread b6 = new Thread(MakePicture.makepict_thread);
                    b6.Start();
                }

                if (core > 6)
                {
                    while (MakePicture.got == false)
                    {
                        Thread.Sleep(time_interval);
                    }
                    MakePicture.partgo = 6;
                    MakePicture.got = false;
                    Thread b7 = new Thread(MakePicture.makepict_thread);
                    b7.Start();
                }

                if (core > 7)
                {
                    while (MakePicture.got == false)
                    {
                        Thread.Sleep(time_interval);
                    }
                    MakePicture.partgo = 7;
                    MakePicture.got = false;
                    Thread b8 = new Thread(MakePicture.makepict_thread);
                    b8.Start();
                }

                if (core > 8)
                {
                    while (MakePicture.got == false)
                    {
                        Thread.Sleep(time_interval);
                    }
                    MakePicture.partgo = 8;
                    MakePicture.got = false;
                    Thread b9 = new Thread(MakePicture.makepict_thread);
                    b9.Start();
                }

                if (core > 9)
                {
                    while (MakePicture.got == false)
                    {
                        Thread.Sleep(time_interval);
                    }
                    MakePicture.partgo = 9;
                    MakePicture.got = false;
                    Thread b10 = new Thread(MakePicture.makepict_thread);
                    b10.Start();
                }

                if (core > 10)
                {
                    while (MakePicture.got == false)
                    {
                        Thread.Sleep(time_interval);
                    }
                    MakePicture.partgo = 10;
                    MakePicture.got = false;
                    Thread b11 = new Thread(MakePicture.makepict_thread);
                    b11.Start();
                }
                while (t[core - 1] == false)
                {
                    Thread.Sleep(time_interval);
                }
                Action action2 = () => progressBar1.Value = 1;
                if (progressBar1.InvokeRequired) progressBar1.Invoke(action2);
                else progressBar1.Value = 1;
                if (checkBox_filter.CheckState == CheckState.Checked)
                {
                    for (int p = 0; p < counttb; p++)
                    {
                        Action action622 = () => label_status.Text = "Фильтр шаг №" + Convert.ToString(p + 1);
                        if (label_status.InvokeRequired) label_status.Invoke(action622);
                        else label_status.Text = "Фильтр шаг №" + Convert.ToString(p + 1);                  
                        for (int x = 0; x < 12; x++)
                        {
                            t[x] = false;
                        }
                        trig_thread_end = false;
                        MakePicture.partx = widht / core;
                        MakePicture.got = false;
                        MakePicture.partgo = 0;
                        Thread c1 = new Thread(MakePicture.filter_cube);
                        c1.Start();
                        if (core > 1)
                        {
                            while (MakePicture.got == false)
                            {
                                Thread.Sleep(time_interval);
                            }
                            MakePicture.got = false;
                            MakePicture.partgo = 1;
                            Thread c2 = new Thread(MakePicture.filter_cube);
                            c2.Start();
                        }
                        if (core > 2)
                        {
                            while (MakePicture.got == false)
                            {
                                Thread.Sleep(time_interval);
                            }
                            MakePicture.partgo = 2;
                            MakePicture.got = false;
                            Thread c3 = new Thread(MakePicture.filter_cube);
                            c3.Start();
                        }

                        if (core > 3)
                        {
                            while (MakePicture.got == false)
                            {
                                Thread.Sleep(time_interval);
                            }
                            MakePicture.partgo = 3;
                            MakePicture.got = false;
                            Thread c4 = new Thread(MakePicture.filter_cube);
                            c4.Start();
                        }

                        if (core > 4)
                        {
                            while (MakePicture.got == false)
                            {
                                Thread.Sleep(time_interval);
                            }
                            MakePicture.partgo = 4;
                            MakePicture.got = false;
                            Thread c5 = new Thread(MakePicture.filter_cube);
                            c5.Start();
                        }

                        if (core > 5)
                        {
                            while (MakePicture.got == false)
                            {
                                Thread.Sleep(time_interval);
                            }
                            MakePicture.partgo = 5;
                            MakePicture.got = false;
                            Thread c6 = new Thread(MakePicture.filter_cube);
                            c6.Start();
                        }

                        if (core > 6)
                        {
                            while (MakePicture.got == false)
                            {
                                Thread.Sleep(time_interval);
                            }
                            MakePicture.partgo = 6;
                            MakePicture.got = false;
                            Thread c7 = new Thread(MakePicture.filter_cube);
                            c7.Start();
                        }

                        if (core > 7)
                        {
                            while (MakePicture.got == false)
                            {
                                Thread.Sleep(time_interval);
                            }
                            MakePicture.partgo = 7;
                            MakePicture.got = false;
                            Thread c8 = new Thread(MakePicture.filter_cube);
                            c8.Start();
                        }

                        if (core > 8)
                        {
                            while (MakePicture.got == false)
                            {
                                Thread.Sleep(time_interval);
                            }
                            MakePicture.partgo = 8;
                            MakePicture.got = false;
                            Thread c9 = new Thread(MakePicture.filter_cube);
                            c9.Start();
                        }

                        if (core > 9)
                        {
                            while (MakePicture.got == false)
                            {
                                Thread.Sleep(time_interval);
                            }
                            MakePicture.partgo = 9;
                            MakePicture.got = false;
                            Thread c10 = new Thread(MakePicture.filter_cube);
                            c10.Start();
                        }

                        if (core > 10)
                        {
                            while (MakePicture.got == false)
                            {
                                Thread.Sleep(time_interval);
                            }
                            MakePicture.partgo = 10;
                            MakePicture.got = false;
                            Thread c11 = new Thread(MakePicture.filter_cube);
                            c11.Start();
                        }                        
                        while (t[core - 1] == false)
                        {
                            Thread.Sleep(time_interval);
                        }
                        Action action45 = () => progressBar1.Value = 2 + p;
                        if (progressBar1.InvokeRequired) progressBar1.Invoke(action45);
                        else progressBar1.Value = 2 + p;
                    }
                }              
                if (pictureBox_Drawn_image.InvokeRequired) pictureBox_Drawn_image.Invoke(new Action<Bitmap>((s) => pictureBox_Drawn_image.Image = s), drawed_picture); //вставляем нарисованное изображение, если выбрано, то после эффекта фильтров
                else pictureBox_Drawn_image.Image = drawed_picture;
                //button_do2.Enabled = true;
                if (button_do2.InvokeRequired) button_do2.Invoke(new Action<bool>((s) => button_do2.Enabled = s), true);
                else button_do2.Enabled = true;
                Action action7 = () => savepaintToolStripMenuItem.Enabled = true;
                if (this.InvokeRequired) this.Invoke(action7);
                else savepaintToolStripMenuItem.Enabled = true;
                Action action75 = () => checkBoxnumbers.Enabled = true;
                if (this.InvokeRequired) this.Invoke(action75);
                else checkBoxnumbers.Enabled = true;
                Action action19 = () => tabControl1.SelectedTab = tabPage2;
                if (tabControl1.InvokeRequired) tabControl1.Invoke(action19);
                else tabControl1.SelectedTab = tabPage2;
                Action action15 = () => checkBoxline1.Enabled = true;
                if (checkBoxline1.InvokeRequired) checkBoxline1.Invoke(action15);
                else checkBoxline1.Enabled = true;
                Action action12 = () => checkBoxline2.Enabled = true;
                if (checkBoxline2.InvokeRequired) checkBoxline1.Invoke(action12);
                else checkBoxline2.Enabled = true;
                Action action13 = () => checkBoxline3.Enabled = true;
                if (checkBoxline3.InvokeRequired) checkBoxline3.Invoke(action13);
                else checkBoxline3.Enabled = true;
                Action action14 = () => label11.Enabled = true;
                if (label11.InvokeRequired) label11.Invoke(action14);
                else label11.Enabled = true;
            }
            else
            {
                MessageBox.Show("Нет цветов");
            }
            Action action8 = () => progressBar1.Value = 0;
            if (progressBar1.InvokeRequired) progressBar1.Invoke(action8);
            else progressBar1.Value = 0;

            Action action623 = () => label_status.Text = "";
            if (label_status.InvokeRequired) label_status.Invoke(action623);
            else label_status.Text = "";
            Action action10 = () => parametry.Text = "Размеры изображения: " + widht + " x " + height + " Используемая палитра: " + Convert.ToString(listBox_colors.Items[usespalitra]);
            if (parametry.InvokeRequired) parametry.Invoke(action10);
            else parametry.Text = "Размеры изображения: " + widht + " x " + height + " Используемая палитра: " + Convert.ToString(listBox_colors.Items[usespalitra]);
            Action action44 = () => progressBar1.Value = 0;
            if (progressBar1.InvokeRequired) progressBar1.Invoke(action44);
            else progressBar1.Value = 0;
            if (checkBoxnumbers.CheckState == CheckState.Checked)
            {
                Action action55 = () => textsize.Enabled = true;
                if (textsize.InvokeRequired) textsize.Invoke(action55);
                else textsize.Enabled = true;
            }
            else
            {
                Action action66 = () => textsize.Enabled = false;
                if (textsize.InvokeRequired) textsize.Invoke(action66);
                else textsize.Enabled = false;
            }
            enab_all();
        }
        private void do_button1(object sender, EventArgs e)
        {
            Thread b1 = new Thread(do_button1_func);
            // Запуск потока:
            b1.Start();
        }

        ////////////////////////////////////////Создаем раскрашенное изображение-конец


        ////////////////////////////////////////Создаем раскраску - начало кода

        bool numbers_event = false; // рисуем ли цифры на раскраске
        void do_button2_func()
        {
            dis_all(); // отключаем интерфейс
            bool textgo1 = true;
            bool textgo2 = true;
            int sizeft = 7;
            if (checkBoxnumbers.CheckState == CheckState.Checked)
            {
                try
                {
                    sizeft = Convert.ToInt32(textsize.Text);
                    textgo1 = true;
                    if (sizeft >= 7)
                    {
                        textgo2 = true;
                    }
                    else
                    {
                        MessageBox.Show("Размер шрифта меньше 7. Увеличьте шрифт и нажмите \"Раскраска\" заного.");
                        textgo2 = false;
                    }
                }
                catch
                {
                    MessageBox.Show("Неправильно задан размер шрифта.");
                    textgo1 = false;
                }
            }
            if (textgo1 == true && textgo2 == true)
            {
                height = drawed_picture.Height;
                widht = drawed_picture.Width;
                Color linego = Color.FromArgb(255, 0, 0, 0); //создаем цвет для контура
                if (checkBoxline1.CheckState == CheckState.Checked)
                {
                    linego = line1; //на самом деле это можно и не делать, т.к. linego уже 0,0,0 по rgb
                }
                else
                {
                    if (checkBoxline2.CheckState == CheckState.Checked)
                    {
                        linego = line2;
                    }
                    else
                    {
                        if (checkBoxline3.CheckState == CheckState.Checked)
                        {
                            linego = line3;
                        }
                    }
                }

                if (numbers_event == true)
                {
                    Action action2 = () => progressBar1.Maximum = 2;
                    if (progressBar1.InvokeRequired) progressBar1.Invoke(action2);
                    else progressBar1.Maximum = 2;
                }
                else
                {
                    Action action2 = () => progressBar1.Maximum = 1;
                    if (progressBar1.InvokeRequired) progressBar1.Invoke(action2);
                    else progressBar1.Maximum = 1;
                }
                lines_picture = DrawSectors.MakeLine(drawed_picture, linego);
                Action action1 = () => progressBar1.Value = 1;
                if (progressBar1.InvokeRequired) progressBar1.Invoke(action1);
                else progressBar1.Value = 1;
                if (checkBoxnumbers.CheckState == CheckState.Checked)
                {
                    Action action622 = () => label_status.Text = "Расставляются номера цветов на раскраске...";
                    if (label_status.InvokeRequired) label_status.Invoke(action622);
                    else label_status.Text = "Расставляются номера цветов на раскраске...";
                    lines_picture = DrawSectors.filter_text(lines_picture, drawed_picture, linego, sizeft); ; //1 - раскраска для подписи, 2 - раскрашенное изображение, 3 - номер цвета для подписи, 4 - размер шрифта
                    
                    Action action3 = () => progressBar1.Value = 2;
                    if (progressBar1.InvokeRequired) progressBar1.Invoke(action3);
                    else progressBar1.Value = 2;
                }
                if (pictureBox_with_line.InvokeRequired) pictureBox_with_line.Invoke(new Action<Bitmap>((s) => pictureBox_with_line.Image = s), lines_picture);
                else pictureBox_with_line.Image = lines_picture;
                Action action4 = () => savelinesToolStripMenuItem.Enabled = true;
                if (this.InvokeRequired) this.Invoke(action4);
                else savelinesToolStripMenuItem.Enabled = true;
                Action action5 = () => progressBar1.Value = 0;
                if (progressBar1.InvokeRequired) progressBar1.Invoke(action5);
                else progressBar1.Value = 0;
                Action action6 = () => tabControl1.SelectedTab = tabPage3;
                if (tabControl1.InvokeRequired) tabControl1.Invoke(action6);
                else tabControl1.SelectedTab = tabPage3;
            }
            Action action7 = () => label_status.Text = "";
            if (label_status.InvokeRequired) label_status.Invoke(action7);
            else label_status.Text = "";
            enab_all(); //включаем интерфейс
        }
        private void do_button2(object sender, EventArgs e) //чертим контуры раскраски
        {
            Thread b1 = new Thread(do_button2_func);
            // Запуск потока:
            b1.Start();
        }

        ////////////////////////////////////////Создаем раскраску - конец кода

        private void textBoxAlpha_KeyPress(object sender, KeyPressEventArgs e) //только цифры
        {
            char number = e.KeyChar;

            if (!Char.IsDigit(number) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }

        }

        private void textBoxRed_KeyPress(object sender, KeyPressEventArgs e) //только цифры
        {
            char number = e.KeyChar;

            if (!Char.IsDigit(number) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBoxGreen_KeyPress(object sender, KeyPressEventArgs e) //только цифры
        {
            char number = e.KeyChar;

            if (!Char.IsDigit(number) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBoxBlue_KeyPress(object sender, KeyPressEventArgs e) //только цифры
        {
            char number = e.KeyChar;

            if (!Char.IsDigit(number) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.CheckState == CheckState.Checked)
            {
                textBoxAlpha.Text = "255";
                textBoxAlpha.Enabled = false;
            }
            else
            { textBoxAlpha.Enabled = true; }
        }
        private void button_addcolor_Click(object sender, EventArgs e) //кнопка добавления цвета в палитру
        {
            try {
                if (textBoxAlpha.Text != "" && textBoxAlpha.Text != " " && textBoxRed.Text != "" && textBoxRed.Text != " " && textBoxGreen.Text != "" && textBoxGreen.Text != " " && textBoxBlue.Text != "" && textBoxBlue.Text != " ")
                {

                    if (Convert.ToInt32(textBoxAlpha.Text) >= 0 && Convert.ToInt32(textBoxAlpha.Text) <= 255 && Convert.ToInt32(textBoxRed.Text) >= 0 && Convert.ToInt32(textBoxRed.Text) <= 255 && Convert.ToInt32(textBoxGreen.Text) >= 0 && Convert.ToInt32(textBoxGreen.Text) <= 255 && Convert.ToInt32(textBoxBlue.Text) >= 0 && Convert.ToInt32(textBoxBlue.Text) <= 255)
                    {
                        if (Convert.ToByte(textBoxRed.Text) == Convert.ToByte(255) && Convert.ToByte(textBoxGreen.Text) == Convert.ToByte(255) && Convert.ToByte(textBoxBlue.Text) == Convert.ToByte(255)
                             || Convert.ToByte(textBoxRed.Text) == Convert.ToByte(0) && Convert.ToByte(textBoxGreen.Text) == Convert.ToByte(0) && Convert.ToByte(textBoxBlue.Text) == Convert.ToByte(0))
                        {
                            MessageBox.Show("Нельзя использовать 0.0.0 или 255.255.255");
                        }
                        else
                        {
                            addcolor(cur, textBox_colorname.Text, Convert.ToByte(textBoxAlpha.Text), Convert.ToByte(textBoxRed.Text), Convert.ToByte(textBoxGreen.Text), Convert.ToByte(textBoxBlue.Text));
                            refreshcurrentpalitra();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неправильный цвет");
                    }
                }
                else
                {
                    MessageBox.Show("Неправильный цвет");
                }
            }
            catch
            {
                MessageBox.Show("Неправильный цвет");
            }
        }
        private void clear_palitra_button(object sender, EventArgs e)
        {
            int kol = Palitra[cur].Count;
            for (int r = 0; r < kol; r++)
            {
                Palitra[cur].RemoveAt(0);
                realizecolor[cur].RemoveAt(0);
            }
            refreshcurrentpalitra(); //обновляем список цветов
        }
        private void clear_current_palitra()
        {
            int kol = CurrentPalitra.Count;
            for (int r = 0; r < kol; r++)
            {
                CurrentPalitra.RemoveAt(0);
            }
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!Char.IsDigit(number) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (numbcolor.Text != "")
            {
                int number = Convert.ToInt32(numbcolor.Text);

                if (number >= 0 && number - 1 < Palitra[cur].Count)
                {
                    Palitra[cur].RemoveAt(number - 1);
                    realizecolor[cur].RemoveAt(number - 1);
                    refreshcurrentpalitra();
                }
                else
                {
                    MessageBox.Show("Неправильный номер цвета");
                }
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OpenPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open_dialog = new OpenFileDialog();
            open_dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*"; //формат загружаемого файла
            if (open_dialog.ShowDialog() == DialogResult.OK) //если в окне была нажата кнопка "ОК"
            {
                try
                {
                    image_orig = new Bitmap(open_dialog.FileName); //зачем копия image1?
                    pictureBox_orig.Image = image_orig;
                    pictureBox_orig.Invalidate();
                    vys = pictureBox_orig.Image.Height;
                    shir = pictureBox_orig.Image.Width;
                    label14.Text = Convert.ToString(shir) + "x" + Convert.ToString(vys);
                    labelres1.Text = Convert.ToString(shir) + "x" + Convert.ToString(vys);
                    resk = 100;
                    bool trig = false;
                    int rr = 0;
                    int tt = 0;
                    while (trig == false)
                    {
                        rr = Convert.ToInt32((double)vys / resk);

                        tt = Convert.ToInt32((double)shir / resk);

                        if (rr > 12 && tt > 12)
                        {
                            trig = true;                           
                            trackBar_resize.Maximum = resk;                            
                        }
                        else
                        {
                            resk = resk - 1;
                        }

                    }

                    changeval1 = (double)vys / resk;
                    changeval2 = (double)shir / resk;
                    while (changeval1 * resk > vys || changeval2 * resk > shir)
                    {
                        changeval1 = changeval1 - 0.01;
                        changeval2 = changeval2 - 0.01;
                    }
                    labelres2.Text = Convert.ToString(Convert.ToInt32(changeval2 * 1)) + "x" + Convert.ToString(Convert.ToInt32(changeval1 * 1));
                    resimage.height_res = vys;
                    resimage.width_res = shir;
                    trackBar_resize.Value = trackBar_resize.Maximum;
                    groupBox_resize.Enabled = true;
                    tabControl1.SelectedTab = tabPage4;
                }
                catch
                {
                    DialogResult rezult = MessageBox.Show("Невозможно открыть выбранный файл",///////переместить!
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);///////переместить!
                }
            }
        }

        private void SavepaintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox_Drawn_image.Image != null) //если в pictureBox есть изображение
            {
                //создание диалогового окна "Сохранить как..", для сохранения изображения
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранить картинку как...";
                //отображать ли предупреждение, если пользователь указывает имя уже существующего файла
                savedialog.OverwritePrompt = true;
                //отображать ли предупреждение, если пользователь указывает несуществующий путь
                savedialog.CheckPathExists = true;
                //список форматов файла, отображаемый в поле "Тип файла"
                savedialog.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
                //отображается ли кнопка "Справка" в диалоговом окне
                //savedialog.ShowHelp = true;
                if (savedialog.ShowDialog() == DialogResult.OK) //если в диалоговом окне нажата кнопка "ОК"
                {
                    try
                    {
                        pictureBox_Drawn_image.Image.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        MessageBox.Show("Сохранено");
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SavelinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox_with_line.Image != null) //если в pictureBox есть изображение
            {
                //создание диалогового окна "Сохранить как..", для сохранения изображения
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранить картинку как...";
                //отображать ли предупреждение, если пользователь указывает имя уже существующего файла
                savedialog.OverwritePrompt = true;
                //отображать ли предупреждение, если пользователь указывает несуществующий путь
                savedialog.CheckPathExists = true;
                //список форматов файла, отображаемый в поле "Тип файла"
                savedialog.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
                //отображается ли кнопка "Справка" в диалоговом окне
                //savedialog.ShowHelp = true; //при использовании открывается неудобное окно для сохранения
                if (savedialog.ShowDialog() == DialogResult.OK) //если в диалоговом окне нажата кнопка "ОК"
                {
                    try
                    {
                        pictureBox_with_line.Image.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        MessageBox.Show("Сохранено");
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Trackbar_filter_cube_Scroll(object sender, EventArgs e)
        {
            filter_count1.Text = Convert.ToString(trackbar_filter_cube.Value * 2);
            trackbar_filter_max.Text = Convert.ToString(trackbar_filter_cube.Maximum * 2);


            trackbar_cache_value = trackbar_filter_cube.Value; //для класса MakePicture
        }

        private void CheckBox_filter_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBox_filter.CheckState == CheckState.Checked)
            { trackbar_filter_cube.Enabled = true;
                trackBar_count.Enabled = true;
            }
            else
            { trackbar_filter_cube.Enabled = false;
                trackBar_count.Enabled = false;
            }

            if (filt_on == false)
            {
                MessageBox.Show("Изображение слишком маленькое");
                trackbar_filter_cube.Enabled = false;
                trackBar_count.Enabled = false;
                checkBox_filter.Enabled = false;
            }
        }

        public class Colorsave
        {
            public string Name { get; set; }
            public int Number { get; set; }
            public byte Alpha { get; set; }
            public byte Red { get; set; }
            public byte Green { get; set; }
            public byte Blue { get; set; }
        }
        private void ColorSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Colorsave> ColorSave = new List<Colorsave>();
            for (int i = 0; i < CurrentPalitra.Count; i++)
            {
                ColorSave.Add(new Colorsave
                {
                    Name = CurrentPalitra[i].N,
                    Number = i + 1,
                    Alpha = CurrentPalitra[i].A,
                    Red = CurrentPalitra[i].R,
                    Green = CurrentPalitra[i].G,
                    Blue = CurrentPalitra[i].B
                }
                );
            }
            string SPO = JsonConvert.SerializeObject(ColorSave);
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Сохранить палитру как...";
            //отображать ли предупреждение, если пользователь указывает имя уже существующего файла
            saveFileDialog1.OverwritePrompt = true;
            //отображать ли предупреждение, если пользователь указывает несуществующий путь
            saveFileDialog1.CheckPathExists = true;
            //список форматов файла, отображаемый в поле "Тип файла"
            saveFileDialog1.Filter = "JSON(*.JSON)|*.JSON|Text(*.TXT)|*.TXT|All files (*.*)|*.*";
            //отображается ли кнопка "Справка" в диалоговом окне
            //saveFileDialog1.ShowHelp = true; //при использовании открывается неудобное окно для сохранения
            if (saveFileDialog1.ShowDialog() == DialogResult.OK) //если в диалоговом окне нажата кнопка "ОК"
            {
                try
                {

                    File.WriteAllText(saveFileDialog1.FileName, SPO);
                    MessageBox.Show("Сохранено");
                }
                catch
                {
                    MessageBox.Show("Невозможно сохранить палитру", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ColorLoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open_dialog2 = new OpenFileDialog();
            open_dialog2.Filter = "JSON Files(*.JSON;*.TXT)|*.JSON;*.TXT|All files (*.*)|*.*"; //формат загружаемого файла
            //Colorsave Colorload = new Colorsave();
            string Colorload;
            string colorl;
            if (open_dialog2.ShowDialog() == DialogResult.OK) //если в окне была нажата кнопка "ОК"
            {
                try
                {
                    Palitra.Add(new List<Colors>());
                    TextReader tr = new StreamReader(open_dialog2.FileName);

                    // read a line of text
                    colorl = tr.ReadLine();
                    Colorload = colorl;
                    JArray ColorArray = JArray.Parse(Colorload);
                    IList<Colorsave> ColorLoad = ColorArray.Select(p => new Colorsave
                    {
                        Name = (string)p["Name"],
                        Number = (int)p["Number"],
                        Alpha = (byte)p["Alpha"],
                        Red = (byte)p["Red"],
                        Blue = (byte)p["Blue"],
                        Green = (byte)p["Green"]
                    }).ToList();

                    int kol = Palitra[ii].Count;
                    for (int r = 0; r < kol; r++)
                    {
                        Palitra[ii].RemoveAt(0);
                    }
                    //refreshlist(); //обновляем список цветов
                    for (int r = 0; r < ColorLoad.Count; r++)
                    {
                        addcolor(ii, ColorLoad[r].Name, ColorLoad[r].Alpha, ColorLoad[r].Red, ColorLoad[r].Green, ColorLoad[r].Blue);
                    }
                    listBox_colors.Items.Add(open_dialog2.FileName);
                    listBox_colors.SelectedIndex = ii;
                    cur = listBox_colors.SelectedIndex;
                    ii++;
                    refreshcurrentpalitra();
                    //refreshlist();
                    //MessageBox.Show(Convert.ToString(ColorLoad));
                }
                catch
                {
                    DialogResult rezult = MessageBox.Show("Невозможно открыть выбранный файл",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                refreshcurrentpalitra();

            }

        }

        private void Label9_Click(object sender, EventArgs e)
        {

        }

        private void TrackBar_count_Scroll(object sender, EventArgs e)
        {
            label_count.Text = Convert.ToString(trackBar_count.Value);
        }

        private void ФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void CheckBoxline1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxline1.CheckState == CheckState.Checked)
            {
                checkBoxline2.CheckState = CheckState.Unchecked;
                checkBoxline3.CheckState = CheckState.Unchecked;
            }
        }

        private void CheckBoxline2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxline2.CheckState == CheckState.Checked)
            {
                checkBoxline1.CheckState = CheckState.Unchecked;
                checkBoxline3.CheckState = CheckState.Unchecked;
            }
        }

        private void CheckBoxline3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxline3.CheckState == CheckState.Checked)
            {
                checkBoxline1.CheckState = CheckState.Unchecked;
                checkBoxline2.CheckState = CheckState.Unchecked;
            }
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            cur = listBox_colors.SelectedIndex;
            refreshcurrentpalitra();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (textBox_palitra_name.Text == "" || textBox_palitra_name.Text == " ")
            {
                MessageBox.Show("Впишите название новой палитры");
            }
            else
            {
                Palitra.Add(new List<Colors>());
                realizecolor.Add(new List<bool>());
                listBox_colors.Items.Add(textBox_palitra_name.Text);
                listBox_colors.SelectedIndex = ii;
                cur = listBox_colors.SelectedIndex;
                ii++;
                refreshcurrentpalitra();
            }
        }

         // сохранение раздатки - начало
        private void savelistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int otstupx = 10; //отступ по х
            int otstupy = 10; // отступ по у
            int vys = 1755;
            int shir = 1245;
            int coorx = otstupx;
            int coory = otstupy;
            int step = 14;
            Bitmap razdatka = new Bitmap(shir, vys);
            if (palitraname_check.CheckState == CheckState.Checked) //Если "да", то делается раздатка с именем
            {
                if (say_count_colors.CheckState == CheckState.Checked) //Если "да", то пишем количество цветов
                {
                    for (int i = 0; i < shir; i++)   // заполняем все белым цветом для фона
                    {
                        for (int y = 0; y < vys; y++)
                        {
                            razdatka.SetPixel(i, y, Color.FromArgb(255, 255, 255, 255));
                        }
                    }
                    Font namefont = new Font("Arial", step * 3);
                    Font myFont = new Font("Arial", step);
                    Graphics g = Graphics.FromImage(razdatka);
                    string palname = Convert.ToString(listBox_colors.SelectedItem) + " Цветов:" + Convert.ToString(CurrentPalitra.Count); //текст заголовка для раздатки
                    g.DrawString(palname, namefont, Brushes.Black, coorx, coory);
                    coory = otstupy + (step * 5);
                    for (int w = 0; w < CurrentPalitra.Count; w++)
                    {
                        bool bufgo = true;
                        if (only_uses_color_check.CheckState == CheckState.Checked)
                        {
                            if (realizecolor[cur][w] == true && cur == usespalitra)
                            {
                                bufgo = true;
                            }
                            else
                            {
                                bufgo = false;
                            }
                        }
                        if (bufgo == true)
                        {
                            string textraz;
                            if (CurrentPalitra[w].N != "" && CurrentPalitra[w].N != " " && CurrentPalitra[w].N != null)
                            {
                                int rt = w + 1;
                                textraz = "№" + rt + ":" + CurrentPalitra[w].N;
                            }
                            else
                            {
                                int rt = w + 1;
                                textraz = "№" + rt + " R:" + CurrentPalitra[w].R + "," + "G:" + CurrentPalitra[w].G + "B:" + CurrentPalitra[w].B + ".";
                            }
                            g.DrawString(textraz, myFont, Brushes.Black, coorx, coory);
                            for (int p = coorx + 300; p < coorx + 300 + 60; p++)
                            {
                                for (int l = coory; l < coory + 14; l++)
                                {
                                    razdatka.SetPixel(p, l, Color.FromArgb(255, CurrentPalitra[w].R, CurrentPalitra[w].G, CurrentPalitra[w].B));
                                }
                            }
                            coory = (step * 2) + coory;
                            if (w == 58)
                            {
                                coorx = 400;
                                coory = otstupy + (step * 5);
                            }
                            else
                            {
                                if (w == 117)
                                {
                                    coorx = 800;
                                    coory = otstupy + (step * 5);
                                }
                            }
                            if (w == 173)
                            {
                                MessageBox.Show("Слишком много цветов в палитре, на раздатке будут только первые 174 цвета");
                                break;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < shir; i++)   // заполняем все белым цветом для фона
                    {
                        for (int y = 0; y < vys; y++)
                        {
                            razdatka.SetPixel(i, y, Color.FromArgb(255, 255, 255, 255));
                        }
                    }
                    Font namefont = new Font("Arial", step * 3);
                    Font myFont = new Font("Arial", step);
                    Graphics g = Graphics.FromImage(razdatka);
                    string palname = Convert.ToString(listBox_colors.SelectedItem); //текст заголовка для раздатки
                    g.DrawString(palname, namefont, Brushes.Black, coorx, coory);
                    coory = otstupy + (step * 5);
                    for (int w = 0; w < CurrentPalitra.Count; w++)
                    {
                        bool bufgo = true;
                        if (only_uses_color_check.CheckState == CheckState.Checked)
                        {
                            if (realizecolor[cur][w] == true && cur == usespalitra)
                            {
                                bufgo = true;
                            }
                            else
                            {
                                bufgo = false;
                            }
                        }
                        if (bufgo == true)
                        {
                            string textraz;
                            if (CurrentPalitra[w].N != "" && CurrentPalitra[w].N != " " && CurrentPalitra[w].N != null)
                            {
                                int rt = w + 1;
                                textraz = "№" + rt + ":" + CurrentPalitra[w].N;
                            }
                            else
                            {
                                int rt = w + 1;
                                textraz = "№" + rt + " R:" + CurrentPalitra[w].R + "," + "G:" + CurrentPalitra[w].G + "B:" + CurrentPalitra[w].B + ".";
                            }
                            g.DrawString(textraz, myFont, Brushes.Black, coorx, coory);
                            for (int p = coorx + 300; p < coorx + 300 + 60; p++)
                            {
                                for (int l = coory; l < coory + 14; l++)
                                {
                                    razdatka.SetPixel(p, l, Color.FromArgb(255, CurrentPalitra[w].R, CurrentPalitra[w].G, CurrentPalitra[w].B));
                                }
                            }
                            coory = (step * 2) + coory;
                            if (w == 58)
                            {
                                coorx = 400;
                                coory = otstupy + (step * 5);
                            }
                            else
                            {
                                if (w == 117)
                                {
                                    coorx = 800;
                                    coory = otstupy + (step * 5);
                                }
                            }
                            if (w == 173)
                            {
                                MessageBox.Show("Слишком много цветов в палитре, на раздатке будут только первые 174 цвета");
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (say_count_colors.CheckState == CheckState.Checked) //Если "да", то пишем количество цветов
                {
                    for (int i = 0; i < shir; i++)
                    {
                        for (int y = 0; y < vys; y++)
                        {
                            razdatka.SetPixel(i, y, Color.FromArgb(255, 255, 255, 255));
                        }
                    }
                    int bufcoory = coory;
                    int bufcoorx = coorx;
                    coory = (step * 2) + coory; //оставляем пустое место для количества цветов
                    Font myFont = new Font("Arial", step);
                    Graphics g = Graphics.FromImage(razdatka);
                    for (int w = 0; w < CurrentPalitra.Count; w++)
                    {
                        bool bufgo = true;
                        if (only_uses_color_check.CheckState == CheckState.Checked)
                        {
                            if (realizecolor[cur][w] == true && cur == usespalitra)
                            {
                                bufgo = true;
                            }
                            else
                            {
                                bufgo = false;
                            }
                        }
                        if (bufgo == true)
                        {
                            string textraz;
                            if (CurrentPalitra[w].N != "" && CurrentPalitra[w].N != " " && CurrentPalitra[w].N != null)
                            {
                                int rt = w + 1;
                                textraz = "№" + rt + ":" + CurrentPalitra[w].N;
                            }
                            else
                            {
                                int rt = w + 1;
                                textraz = "№" + rt + " R:" + CurrentPalitra[w].R + "," + "G:" + CurrentPalitra[w].G + "B:" + CurrentPalitra[w].B + ".";
                            }
                            g.DrawString(textraz, myFont, Brushes.Black, coorx, coory);
                            for (int p = coorx + 300; p < coorx + 300 + 60; p++)
                            {
                                for (int l = coory; l < coory + 14; l++)
                                {
                                    razdatka.SetPixel(p, l, Color.FromArgb(255, CurrentPalitra[w].R, CurrentPalitra[w].G, CurrentPalitra[w].B));
                                }
                            }
                            coory = (step * 2) + coory;
                            if (w == 60)
                            {
                                coorx = 400;
                                coory = otstupy;
                            }
                            else
                            {
                                if (w == 122)
                                {
                                    coorx = 800;
                                    coory = otstupy;
                                }
                            }
                            if (w == 184)
                            {
                                MessageBox.Show("Слишком много цветов в палитре, на раздатке будут только первые 185 цветов");
                                break;
                            }
                        }
                    }
                    string bufcountcolors = "Количество цветов в палитре = " + CurrentPalitra.Count; //пишем количество цветов
                    g.DrawString(bufcountcolors, myFont, Brushes.Black, bufcoorx, bufcoory);
                }
                else
                {
                    for (int i = 0; i < shir; i++)
                    {
                        for (int y = 0; y < vys; y++)
                        {
                            razdatka.SetPixel(i, y, Color.FromArgb(255, 255, 255, 255));
                        }
                    }
                    Font myFont = new Font("Arial", step);
                    for (int w = 0; w < CurrentPalitra.Count; w++)
                    {
                        bool bufgo = true;
                        if (only_uses_color_check.CheckState == CheckState.Checked)
                        {
                            if (realizecolor[cur][w] == true && cur == usespalitra)
                            {
                                bufgo = true;
                            }
                            else
                            {
                                bufgo = false;
                            }
                        }
                        if (bufgo == true)
                        {
                            string textraz;
                            if (CurrentPalitra[w].N != "" && CurrentPalitra[w].N != " " && CurrentPalitra[w].N != null)
                            {
                                int rt = w + 1;
                                textraz = "№" + rt + ":" + CurrentPalitra[w].N;
                            }
                            else
                            {
                                int rt = w + 1;
                                textraz = "№" + rt + " R:" + CurrentPalitra[w].R + "," + "G:" + CurrentPalitra[w].G + "B:" + CurrentPalitra[w].B + ".";
                            }
                            Graphics g = Graphics.FromImage(razdatka);
                            g.DrawString(textraz, myFont, Brushes.Black, coorx, coory);
                            for (int p = coorx + 300; p < coorx + 300 + 60; p++)
                            {
                                for (int l = coory; l < coory + 14; l++)
                                {
                                    razdatka.SetPixel(p, l, Color.FromArgb(255, CurrentPalitra[w].R, CurrentPalitra[w].G, CurrentPalitra[w].B));
                                }
                            }
                            coory = (step * 2) + coory;
                            if (w == 61)
                            {
                                coorx = 400;
                                coory = otstupy;
                            }
                            else
                            {
                                if (w == 123)
                                {
                                    coorx = 800;
                                    coory = otstupy;
                                }
                            }
                            if (w == 185)
                            {
                                MessageBox.Show("Слишком много цветов в палитре, на раздатке будут только первые 186 цветов");
                                break;
                            }
                        }
                    }

                }
            }
            if (razdatka != null) //если в pictureBox есть изображение
            {
                //создание диалогового окна "Сохранить как..", для сохранения изображения
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранить картинку как...";
                //отображать ли предупреждение, если пользователь указывает имя уже существующего файла
                savedialog.OverwritePrompt = true;
                //отображать ли предупреждение, если пользователь указывает несуществующий путь
                savedialog.CheckPathExists = true;
                //список форматов файла, отображаемый в поле "Тип файла"
                savedialog.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
                //отображается ли кнопка "Справка" в диалоговом окне
                //savedialog.ShowHelp = true; //при использовании открывается неудобное окно для сохранения
                if (savedialog.ShowDialog() == DialogResult.OK) //если в диалоговом окне нажата кнопка "ОК"
                {
                    try
                    {
                        string vyvod = Convert.ToString(listBox_colors.SelectedItem) + " палитра сохранена на раздатке";
                        razdatka.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        MessageBox.Show(vyvod);
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        // сохранение раздатки - конец
        private void ПодписыватьНазваниеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            savesettings();
        }

        private void ОтображатьНаРаздаткеТолькоИспользуемыеЦветаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            savesettings();
        }

        private void ПодписыватьКоличествоЦветовНаРаздаткеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            savesettings();
        }

        private void НастройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void TextBoxRed_TextChanged(object sender, EventArgs e)
        {

        }

        private void TextBox1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!Char.IsDigit(number) && !Char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void CheckBoxfilter_Click(object sender, EventArgs e)
        {
            if (checkBoxnumbers.CheckState == CheckState.Checked)
            {
                textsize.Enabled = true;
                label13.Enabled = true;
            }
            else
            {
                textsize.Enabled = false;
                label13.Enabled = false;
            }
            savesettings();
        }

        private void TabPage4_Click(object sender, EventArgs e)
        {

        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                button_do.Visible = false;
                checkBox_filter.Visible = false;
                filter.Visible = false;
                button_do2.Visible = false;
                Param_paint.Visible = false;
                groupBox_resize.Visible = true;
            }
            else
            {
                button_do.Visible = true;
                checkBox_filter.Visible = true;
                filter.Visible = true;
                button_do2.Visible = true;
                Param_paint.Visible = true;
                groupBox_resize.Visible = false;
            }
        }
        /////////////////////////////Уменьшить размер изображения - начало
        static public int vys = 0;
        static public int shir = 0;       
        int resk = 100;
        int core = Environment.ProcessorCount - 1;
        static public int partx;
        private void textBoxBlue_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textsize_TextChanged(object sender, EventArgs e)
        {
            savesettings();
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
        }


        private void checkBoxnumbers_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxnumbers.CheckState == CheckState.Checked)
            {
                numbers_event = true;
            }
            else
            {
                numbers_event = false;
            }    
        }

        public static bool[] t = new bool[12];
        
        void button_resize_do()
        {
            dis_all();
            resized_image = new Bitmap(resimage.width_res, resimage.height_res);
            Action action11 = () => progressBar1.Maximum = 2;
            if (progressBar1.InvokeRequired) progressBar1.Invoke(action11);
            else progressBar1.Maximum = 2;
            for (int x = 0; x < 12; x++)
            {
                t[x] = false;
            }
            if (resimage.core > 11)
            {
                resimage.core = 11;
            }
            if (resimage.core < 2)
            {
                resimage.core = 2;
            }
            resimage.partx = resimage.width_res / resimage.core;
            resimage.got = false;
            resimage.partgo = 0;
            Thread b1 = new Thread(resimage.res_thread);
            b1.Start();
            while (resimage.got == false)
            {
                Thread.Sleep(time_interval);
            }
            resimage.got = false;
            resimage.partgo = 1;
            Thread b2 = new Thread(resimage.res_thread);
            b2.Start();
            if (resimage.core > 2)
            {
                while (resimage.got == false)
                {
                    Thread.Sleep(time_interval);
                }
                resimage.partgo = 2;
                resimage.got = false;
                Thread b3 = new Thread(resimage.res_thread);
                b3.Start();
            }            
            if (resimage.core > 3)
            {
                while (resimage.got == false)
                {
                    Thread.Sleep(time_interval);
                }
                resimage.partgo = 3;
                resimage.got = false;
                Thread b4 = new Thread(resimage.res_thread);
                b4.Start();
            }

            if (resimage.core > 4)
            {
                while (resimage.got == false)
                {
                    Thread.Sleep(time_interval);
                }
                resimage.partgo = 4;
                resimage.got = false;
                Thread b5 = new Thread(resimage.res_thread);
                b5.Start();
            }

            if (core > 5)
            {
                while (resimage.got == false)
                {
                    Thread.Sleep(time_interval);
                }
                resimage.partgo = 5;
                resimage.got = false;
                Thread b6 = new Thread(resimage.res_thread);
                b6.Start();
            }

            if (core > 6)
            {
                while (resimage.got == false)
                {
                    Thread.Sleep(time_interval);
                }
                resimage.partgo = 6;
                resimage.got = false;
                Thread b7 = new Thread(resimage.res_thread);
                b7.Start();
            }

            if (resimage.core > 7)
            {
                while (resimage.got == false)
                {
                    Thread.Sleep(time_interval);
                }
                resimage.partgo = 7;
                resimage.got = false;
                Thread b8 = new Thread(resimage.res_thread);
                b8.Start();
            }

            if (core > 8)
            {
                while (resimage.got == false)
                {
                    Thread.Sleep(time_interval);
                }
                resimage.partgo = 8;
                resimage.got = false;
                Thread b9 = new Thread(resimage.res_thread);
                b9.Start();
            }

            if (core > 9)
            {
                while (resimage.got == false)
                {
                    Thread.Sleep(time_interval);
                }
                resimage.partgo = 9;
                resimage.got = false;
                Thread b10 = new Thread(resimage.res_thread);
                b10.Start();
            }
            if (resimage.core > 10)
            {
                while (resimage.got == false)
                {
                    Thread.Sleep(time_interval);
                }
                resimage.partgo = 10;
                resimage.got = false;
                Thread b11 = new Thread(resimage.res_thread);
                b11.Start();
            }
            Action action2 = () => progressBar1.Value = 1;
            if (progressBar1.InvokeRequired) progressBar1.Invoke(action2);
            else progressBar1.Value = 1;
            while (t[resimage.core - 1] == false)
            {                  
                    Thread.Sleep(time_interval);                   
            }           
            widht = resized_image.Width;
            height = resized_image.Height;
            Action action1 = () => pictureBox_resized_image.Image = resized_image;
            if (pictureBox_resized_image.InvokeRequired) pictureBox_resized_image.Invoke(action1);
            else pictureBox_resized_image.Image = resized_image;  
            Action action22 = () => parametry.Text = "Размеры изображения: " +  widht + " x " + height;
            if (parametry.InvokeRequired) parametry.Invoke(action22);
            else parametry.Text = "Размеры изображения: " + widht + " x " + height;
            if (height / 100 < widht / 100) //выставляем максимальный параметр для фильтра 
            { 
                Action action3 = () => trackbar_filter_cube.Maximum = height / 100;
                if (trackbar_filter_cube.InvokeRequired) trackbar_filter_cube.Invoke(action3);
                else trackbar_filter_cube.Maximum = height / 100;
                Action action4 = () => trackbar_filter_max.Text = Convert.ToString((height / 100) * 2);
                if (trackbar_filter_max.InvokeRequired) trackbar_filter_max.Invoke(action4);
                else trackbar_filter_max.Text = Convert.ToString((height / 100) * 2);
                if (height / 100 < 2 || widht / 100 < 2)
                {
                    MessageBox.Show("Изображение слишком маленькое. Фильтр будет отключен");
                    filt_on = false; //чтобы чекбокс не включался
                    trigmakepaint = false; //чтобы чекбокс не включался
                    //checkBox_filter.Enabled = false;
                    Action action5 = () => checkBox_filter.Enabled = false;
                    if (checkBox_filter.InvokeRequired) checkBox_filter.Invoke(action5);
                    else checkBox_filter.Enabled = false;
                }
                else
                {
                    filt_on = true;
                }
            }
            else
            {
                Action action6 = () => trackbar_filter_cube.Maximum = widht / 100;
                if (trackbar_filter_cube.InvokeRequired) trackbar_filter_cube.Invoke(action6);
                else trackbar_filter_cube.Maximum = widht / 100;
                Action action7 = () => trackbar_filter_max.Text = Convert.ToString((widht / 100) * 2);
                if (trackbar_filter_max.InvokeRequired) trackbar_filter_max.Invoke(action7);
                else trackbar_filter_max.Text = Convert.ToString((widht / 100) * 2);
                if (height / 100 < 2 || widht / 100 < 2)///////переместить!
                {
                    MessageBox.Show("Изображение слишком маленькое. Фильтр будет отключен");
                    filt_on = false;
                    Action action8 = () => checkBox_filter.Enabled = false;
                    if (checkBox_filter.InvokeRequired) checkBox_filter.Invoke(action8);
                    else checkBox_filter.Enabled = false;
                }
                else
                {
                    filt_on = true;
                    Action action12 = () => checkBox_filter.Enabled = true;
                    if (checkBox_filter.InvokeRequired) checkBox_filter.Invoke(action12);
                    else checkBox_filter.Enabled = true;
                }
            }
            Action action44 = () => progressBar1.Value = 2;
            if (progressBar1.InvokeRequired) progressBar1.Invoke(action44);
            else progressBar1.Value = 2;
            Action action9 = () => filter_count1.Text = Convert.ToString(trackbar_filter_cube.Value * 2);
            if (filter_count1.InvokeRequired) filter_count1.Invoke(action9);
            else filter_count1.Text = Convert.ToString(trackbar_filter_cube.Value * 2);
            Action action998 = () => trackbar_cache_value = trackbar_filter_cube.Value; //для класса MakePicture
            if (filter_count1.InvokeRequired) filter_count1.Invoke(action998);
            else trackbar_cache_value = trackbar_filter_cube.Value;
            Action action10 = () => tabControl1.SelectedTab = tabPage1;
            if (tabControl1.InvokeRequired) tabControl1.Invoke(action10);
            else tabControl1.SelectedTab = tabPage1; 
            Action action111 = () => button_do.Enabled = true;
            if (button_do.InvokeRequired) button_do.Invoke(action111);
            else button_do.Enabled = true;
            Action action33 = () => progressBar1.Value = 0;
            if (progressBar1.InvokeRequired) progressBar1.Invoke(action33);
            else progressBar1.Value = 0;  
            enab_all();
        }
        private void Button_resize_Click(object sender, EventArgs e)
        {
            Thread b1 = new Thread(button_resize_do);
            // Запуск потока:
            b1.Start();
        }
        double changeval1 = 1; 
        double changeval2 = 1; 
        private void Label16_Click(object sender, EventArgs e)
        {

        }
        public static int trackbar_cache_value; //для копии значения с trackbar_filter_cube
        private void TrackBar_resize_Scroll(object sender, EventArgs e)
        {
            if (trackBar_resize.Value == trackBar_resize.Maximum)
            {
                resimage.height_res = vys;
                resimage.width_res = shir;
            }
            else
            {
                resimage.height_res = Convert.ToInt32(changeval1 * trackBar_resize.Value);

                resimage.width_res = Convert.ToInt32(changeval2 * trackBar_resize.Value);
            }

            label14.Text = Convert.ToString(resimage.width_res) + "x" + Convert.ToString(resimage.height_res);
        }
    }
    
}
