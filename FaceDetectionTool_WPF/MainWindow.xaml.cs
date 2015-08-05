﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using static FaceDetectionTool_WPF.Properties.Settings;

namespace FaceDetectionTool_WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Width = Default.WinWidth;
            Height = Default.WinHeight;
        }

        ImagePath imagePath = new ImagePath();
        private List<ImageInfo> imageInfoList;
        private int index = 0;

        private void New_Click(object sender, RoutedEventArgs e)
        {
            var new_configuration = new Configuration();
            new_configuration.Accept = (s) =>
            {
                btnLast.IsEnabled = true;
                btnNext.IsEnabled = true;

                imagePath = s.ImagePath;
                imageInfoList = imagePath.GetImageInfoList();
               // ShowImg();
                this.Activate();
            };
            new_configuration.Show();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.A: btnLast_Click(btnLast, null); break;
                case Key.D: btnNext_Click(btnNext, null); break;
                case Key.I: Info_Click(null, null); break;
            }
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            index--;
            if (index == -1)
                index = imageInfoList.Count - 1;
            ShowImg();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            index++;
            if (index == imageInfoList.Count)
                index = 0;
            ShowImg();
        }

        private void ShowImg()
        {
            var ii = imageInfoList[index];

            canvas.Children.Clear();
            var image = new Image() { Source = ii.Bitmap };
            canvas.Width = image.Width;
            canvas.Height = image.Height;
            canvas.Children.Add(image);

            if (ii.D_Shapes == null || ii.G_Shapes == null)
            {
                ii.AddShapes(Brushes.LightBlue, TypeE.Detection);
                ii.AddShapes(Brushes.DeepPink, TypeE.Gt);
            }
            foreach (var item in ii.D_Shapes)
                canvas.Children.Add(item);
            foreach (var item in ii.G_Shapes)
                canvas.Children.Add(item);

            Calc_Click(null, null);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Default.WinWidth = ActualWidth;
            Default.WinHeight = ActualHeight;
            Default.Save();
            Application.Current.Shutdown();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            if (imageInfoList == null)
                return;
            var info_win = new Info(imageInfoList[index]);
            info_win.Owner = this;
            info_win.Show();
        }

        private void Calc_Click(object sender, RoutedEventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();
            var ms = imageInfoList[index].Matches;
            var str = string.Join("\n", ms.Select(m => m.IoU));
            tb_Result.Text += $"{str}\n{sw.ElapsedMilliseconds}ms";
            sw.Stop();
            foreach (var item in ms)
            {
                item.D_Shape.Fill = Brushes.Blue;
                item.G_Shape.Fill = Brushes.Red;
            }
        }

        private void Eval_Click(object sender, RoutedEventArgs e)
        {
            var win = new EvaluationInfo(imageInfoList);
            win.Show();
        }
    }
}