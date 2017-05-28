using AlgorithmTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
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
        private enum proc { FCFS = 1, SJF, SRT, HRN, PRIORITY, ROUNDROBIN };
        private int select_flag = -1;   // 1:fcfs, 2:sjf, 3:srt, 4:hrn, 5:priority, 6:round_robin, 0:No page

        private FCFS        fcfs;
        private SJF         sjf;
        private SRT         srt;
        private HRN         hrn;
        private Priority    prio;
        private RoundRobin  rrb;

        private List<ProcessData>   data;
        private double[]            wait_time;
        private double[]            return_time;

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
            Init_Method();
        }

        #region working delegate
        /* FCFS */
        private void Item1_Clicked()
        {
            select_flag = ( int )proc.FCFS;
            tbTitle.Content = "F C F S";
            tbDescription.Content = @":  먼저 들어온 순서대로 처리합니다.";
            lbTimequantum.Visibility = Visibility.Hidden;
            sliderTimequantum.Visibility = Visibility.Hidden;
        }
        /* SJF */
        private void Item2_Clicked()
        {
            select_flag = ( int )proc.SJF;
            tbTitle.Content = "S J F";
            tbDescription.Content = @":  서비스 시간이 가장 짧은 순서대로 처리합니다.";
            lbTimequantum.Visibility = Visibility.Hidden;
            sliderTimequantum.Visibility = Visibility.Hidden;
        }
        /* SRT */
        private void Item3_Clicked()
        {
            select_flag = ( int )proc.SRT;
            tbTitle.Content = "S R T";
            tbDescription.Content = @":  SJF의 선점형 구조로서 매번 누가 짧은지 확인하여 처리합니다.";
            lbTimequantum.Visibility = Visibility.Hidden;
            sliderTimequantum.Visibility = Visibility.Hidden;
        }
        /* HRN */
        private void Item4_Clicked()
        {
            select_flag = ( int )proc.HRN;
            tbTitle.Content = "H R N";
            tbDescription.Content = @":  설명이 필요합니다.";
            lbTimequantum.Visibility = Visibility.Hidden;
            sliderTimequantum.Visibility = Visibility.Hidden;
        }
        /* Priority */
        private void Item5_Clicked()
        {
            select_flag = ( int )proc.PRIORITY;
            tbTitle.Content = "Priority";
            tbDescription.Content = @":  선점형이며 우선순위가 높은 순서대로 처리합니다.";
            lbTimequantum.Visibility = Visibility.Hidden;
            sliderTimequantum.Visibility = Visibility.Hidden;
        }
        /* Round-Robin */
        private void Item6_Clicked()
        {
            select_flag = ( int )proc.ROUNDROBIN;
            tbTitle.Content = "Round-Robin";
            tbDescription.Content = @":  Time Quantum 단위로 처리합니다.";
            lbTimequantum.Visibility = Visibility.Visible;
            sliderTimequantum.Visibility = Visibility.Visible;
        }

        /* Run */
        private void Run_Auto()
        {
            tableProcess.IsReadOnly = true;
            btnRun.Visibility = Visibility.Visible;
            CreateTables();
        }
        private void Run_Static()
        {
            tableProcess.IsReadOnly = false;
            PTHeader1.IsReadOnly = PTHeader2.IsReadOnly = true;
            btnRun.Visibility = Visibility.Visible;
            CreateTables();
        }
        #endregion

        public void Init_Method()
        {
            data        = new List<ProcessData>();
            wait_time   = new double[ 6 ];
            return_time = new double[ 6 ];

            chartProcess.ChartAreas.Add( "GanttArea" );
            chartWait.ChartAreas.Add( "PieArea" );
            chartReturn.ChartAreas.Add( "PieArea" );

            chartWait.Titles.Add( "평균대기시간" );
            chartReturn.Titles.Add( "평균반환시간" );
        }

        #region Drawing Chart Area
        /* Gantt Chart Update */
        private void ProcessUpdate( List<ProcessData> chart )
        {
            chartProcess.Series.Clear();

            Series seriesGantt = new Series();
            seriesGantt.ChartType = SeriesChartType.RangeBar;

            seriesGantt.YValuesPerPoint = 2;
            
            for ( int i = 0; i < chart.Count; i++ )
            {
                seriesGantt.Points.AddXY( Convert.ToInt32( chart[ i ].pid ), Convert.ToInt32( chart[ i ].arrived_time ), ( Convert.ToInt32( chart[ i ].arrived_time ) + Convert.ToInt32( chart[ i ].service_time ) ) );
            }

            chartProcess.Series.Add( seriesGantt );
            chartProcess.Update();
        }

        /* Wait Chart Update */
        private void WaitUpdate()
        {
            string[] name = new string[] { "FCFS", "SJF", "SRT", "HRN", "Priority", "Round-Robin" };
            chartWait.Series.Clear();

            Series seriesPie = new Series();
            seriesPie.ChartType = SeriesChartType.Pie;

            for ( int i = 0; i < wait_time.Length; i++ )
            {
                seriesPie.Points.AddXY( name[ i ], wait_time[ i ] );
            }
            
            chartWait.Series.Add( seriesPie );
            chartWait.Series[ 0 ].IsValueShownAsLabel = true;
            chartWait.Update();
        }

        /* Return Time Chart Update */
        private void ReturnUpdate()
        {
            string[] name = new string[] { "FCFS", "SJF", "SRT", "HRN", "Priority", "Round-Robin" };
            chartReturn.Series.Clear();

            Series seriesPie = new Series();
            seriesPie.ChartType = SeriesChartType.Pie;

            for ( int i = 0; i < return_time.Length; i++ )
            {
                seriesPie.Points.AddXY( name[ i ], return_time[ i ] );
            }

            chartReturn.Series.Add( seriesPie );
            chartReturn.Series[ 0 ].IsValueShownAsLabel = true;
            chartReturn.Update();
        }
        #endregion

        /* DataGrid Changing */
        private void CreateTables()
        {
            Random rand = new Random();
            int limit = Convert.ToInt32( topBar.sliderProcess.Value );
            List<ProcessData> temp = new List<ProcessData>();
            
            for ( int i = 0; i < limit; i++ )
            {
                if ( ( bool )topBar.tgBtn.IsChecked )
                    temp.Add( new ProcessData() { no = "" + ( i + 1 ), pid = "" + ( Common.START_PID + i ), priority = "" + rand.Next( 8 ), arrived_time = "" + ( rand.Next( 10 ) + 1 ), service_time = "" + ( rand.Next( 10 ) + 1 ) } );
                else
                    temp.Add( new ProcessData() { no = "" + ( i + 1 ), pid = "" + ( Common.START_PID + i ), priority = "", arrived_time = "", service_time = "" } );
            }

            tableProcess.ItemsSource = temp;
            tableProcess.Items.Refresh();
        }

        private void Reset()
        {
            chartProcess.Series.Clear();
            chartWait.Series.Clear();
            chartReturn.Series.Clear();
            tableProcess.ItemsSource = null;
            tableProcess.Items.Refresh();
        }

        /* After creating table, Drawing for chart */
        private void btnRun_Click( object sender, RoutedEventArgs e )
        {
            try
            {
                if ( select_flag > 0 )
                {
                    running();
                }
                else
                {
                    MessageBox.Show( "알고리즘 기법이 선택되지 않았습니다.\n확인 후 다시 작업을 요청하십시오.", "오류", MessageBoxButton.OK, MessageBoxImage.Error );
                }
            }
            catch
            {
                MessageBox.Show("테이블에 올바르지 않은 값이 포함되어 있습니다.\n확인 후 다시 작업을 요청하십시오.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void running()
        {
            int time_quantum = Convert.ToInt32( sliderTimequantum.Value );
            //data = get_data();        // 얕은 복사 실행(이 코드에선 사용금지)
            List<ProcessData> tmp = GenericCopier<List<ProcessData>>.DeepCopy( get_data() );    // 깊은 복사 실행
            List<ProcessData> data_fcfs = GenericCopier<List<ProcessData>>.DeepCopy( tmp );    // 깊은 복사 실행
            List<ProcessData> data_sjf = GenericCopier<List<ProcessData>>.DeepCopy( tmp );    // 깊은 복사 실행
            List<ProcessData> data_srt = GenericCopier<List<ProcessData>>.DeepCopy( tmp );    // 깊은 복사 실행
            List<ProcessData> data_hrn = GenericCopier<List<ProcessData>>.DeepCopy( tmp );    // 깊은 복사 실행
            List<ProcessData> data_prio = GenericCopier<List<ProcessData>>.DeepCopy( tmp );    // 깊은 복사 실행
            List<ProcessData> data_rrb = GenericCopier<List<ProcessData>>.DeepCopy( tmp );    // 깊은 복사 실행

            //foreach ( var i in data )
            //    Console.WriteLine( i.ToString() );

            fcfs    = new FCFS( data_fcfs );
            sjf     = new SJF( data_sjf );
            srt     = new SRT( data_srt );
            hrn     = new HRN( data_hrn );
            prio    = new Priority( data_prio );
            rrb     = new RoundRobin( data_rrb, time_quantum );

            List<ProcessData> temp = new List<ProcessData>();

            switch ( select_flag )
            {
                case ( int )proc.FCFS:
                    temp = fcfs.working();
                    sjf.working(); srt.working();hrn.working();
                    prio.working(); rrb.working();
                    break;
                case ( int )proc.SJF:
                    temp = sjf.working();
                    fcfs.working(); srt.working();
                    hrn.working(); prio.working(); rrb.working();
                    break;
                case ( int )proc.SRT:
                    temp = srt.working();
                    fcfs.working(); sjf.working();
                    hrn.working(); prio.working(); rrb.working();
                    break;
                case ( int )proc.HRN:
                    temp = hrn.working();
                    fcfs.working(); sjf.working(); srt.working();
                    prio.working(); rrb.working();
                    break;
                case ( int )proc.PRIORITY:
                    temp = prio.working();
                    fcfs.working(); sjf.working(); srt.working();
                    hrn.working(); rrb.working();
                    break;
                case ( int )proc.ROUNDROBIN:
                    temp = rrb.working();
                    fcfs.working(); sjf.working(); srt.working();
                    hrn.working(); prio.working();
                    break;
                default:
                    MessageBox.Show( "올바르지 않은 접근입니다.\n확인 후 다시 작업을 요청하십시오.", "오류", MessageBoxButton.OK, MessageBoxImage.Error );
                    return;
            }

            //Console.WriteLine( "############################" );
            //foreach ( var i in data )
            //    Console.WriteLine( i.ToString() );

            wait_time[ ( int )proc.FCFS - 1 ] = fcfs.avg_wait();
            wait_time[ ( int )proc.FCFS - 1 ] = 1;
            wait_time[ ( int )proc.SJF - 1 ] = sjf.avg_wait();
            wait_time[ ( int )proc.SRT - 1 ] = srt.avg_wait();
            wait_time[ ( int )proc.HRN - 1 ] = hrn.avg_wait();
            wait_time[ ( int )proc.PRIORITY - 1 ] = prio.avg_wait();
            wait_time[ ( int )proc.ROUNDROBIN - 1 ] = rrb.avg_wait();

            return_time[ ( int )proc.FCFS - 1 ] = fcfs.avg_return();
            return_time[ ( int )proc.FCFS - 1 ] = 1;
            return_time[ ( int )proc.SJF - 1 ] = sjf.avg_return();
            return_time[ ( int )proc.SRT - 1 ] = srt.avg_return();
            return_time[ ( int )proc.HRN - 1 ] = hrn.avg_return();
            return_time[ ( int )proc.PRIORITY - 1 ] = prio.avg_return();
            return_time[ ( int )proc.ROUNDROBIN - 1 ] = rrb.avg_return();

            ProcessUpdate( temp );
            WaitUpdate();
            ReturnUpdate();
        }

        private List<ProcessData> get_data()
        {
            List<ProcessData> res = new List<ProcessData>();

            foreach ( ProcessData item in tableProcess.ItemsSource )
            {
                res.Add( item );
            }

            return res;
        }
    }

    /* Deep Copy를 위해 반드시 필요 */
    public static class GenericCopier<T>
    {
        public static T DeepCopy( object objectToCopy )
        {
            using ( MemoryStream memoryStream = new MemoryStream() )
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize( memoryStream, objectToCopy );
                memoryStream.Position = 0;
                return ( T )binaryFormatter.Deserialize( memoryStream );
            }
        }
    }
}
