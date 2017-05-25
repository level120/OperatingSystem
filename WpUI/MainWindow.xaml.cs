using System;
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

namespace WpUI
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private int select_flag = -1;   // 1:fcfs, 2:sjf, 3:srt, 4:hrn, 5:priority, 6:round_robin, 0:No page

        #region define delegate and events
        public delegate void item1_Click();
        public delegate void item2_Click();
        public delegate void item3_Click();
        public delegate void item4_Click();
        public delegate void item5_Click();
        public delegate void item6_Click();

        public delegate void run_Auto();
        public delegate void run_Static();

        private void Init_Events()
        {
            lfMenu._item1_Click += new item1_Click( Item1_Clicked );
            lfMenu._item2_Click += new item2_Click( Item2_Clicked );
            lfMenu._item3_Click += new item3_Click( Item3_Clicked );
            lfMenu._item4_Click += new item4_Click( Item4_Clicked );
            lfMenu._item5_Click += new item5_Click( Item5_Clicked );
            lfMenu._item6_Click += new item6_Click( Item6_Clicked );

            topBar._runAuto += new run_Auto( Run_Auto );
            topBar._runStatic += new run_Static( Run_Static );
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            Init_Events();
        }

        #region working delegate
        /* FCFS */
        private void Item1_Clicked()
        {
            select_flag = 1;
            tbTitle.Content = "F C F S";
            tbDescription.Content = @":  먼저 들어온 순서대로 처리합니다.";
        }
        /* SJF */
        private void Item2_Clicked()
        {
            select_flag = 2;
            tbTitle.Content = "S J F";
            tbDescription.Content = @":  서비스 시간이 가장 짧은 순서대로 처리합니다.";
        }
        /* SRT */
        private void Item3_Clicked()
        {
            select_flag = 3;
            tbTitle.Content = "S R T";
            tbDescription.Content = @":  SJF의 선점형 구조로서 매번 누가 짧은지 확인하여 처리합니다.";
        }
        /* HRN */
        private void Item4_Clicked()
        {
            select_flag = 4;
            tbTitle.Content = "H R N";
            tbDescription.Content = @":  설명이 필요합니다.";
        }
        /* Priority */
        private void Item5_Clicked()
        {
            select_flag = 5;
            tbTitle.Content = "Priority";
            tbDescription.Content = @":  선점형이며 우선순위가 높은 순서대로 처리합니다.";
        }
        /* Round-Robin */
        private void Item6_Clicked()
        {
            select_flag = 6;
            tbTitle.Content = "Round-Robin";
            tbDescription.Content = @":  Time Quantum 단위로 처리합니다.";
        }

        /* Run */
        private void Run_Auto()
        {
            MessageBox.Show( "Auto" );
        }
        private void Run_Static()
        {
            MessageBox.Show( "Static" );
        }
        #endregion


    }
}
