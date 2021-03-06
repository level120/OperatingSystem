﻿using AlgorithmTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Interop;
using Technewlogic.WpfDialogManagement;

namespace WpUI
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Window help icon in title bar
        private const uint WS_EX_CONTEXTHELP = 0x00000400;
        private const uint WS_MINIMIZEBOX = 0x00020000;
        private const uint WS_MAXIMIZEBOX = 0x00010000;
        private const int GWL_STYLE = -16;
        private const int GWL_EXSTYLE = -20;
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_FRAMECHANGED = 0x0020;
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_CONTEXTHELP = 0xF180;


        [DllImport( "user32.dll" )]
        private static extern uint GetWindowLong( IntPtr hwnd, int index );

        [DllImport( "user32.dll" )]
        private static extern int SetWindowLong( IntPtr hwnd, int index, uint newStyle );

        [DllImport( "user32.dll" )]
        private static extern bool SetWindowPos( IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags );


        protected override void OnSourceInitialized( EventArgs e )
        {
            base.OnSourceInitialized( e );
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper( this ).Handle;
            uint styles = GetWindowLong( hwnd, GWL_STYLE );
            styles &= 0xFFFFFFFF ^ ( WS_MINIMIZEBOX | WS_MAXIMIZEBOX );
            SetWindowLong( hwnd, GWL_STYLE, styles );
            styles = GetWindowLong( hwnd, GWL_EXSTYLE );
            styles |= WS_EX_CONTEXTHELP;
            SetWindowLong( hwnd, GWL_EXSTYLE, styles );
            SetWindowPos( hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED );
            ( ( HwndSource )PresentationSource.FromVisual( this ) ).AddHook( HelpHook );
        }

        private IntPtr HelpHook( IntPtr hwnd,
                int msg,
                IntPtr wParam,
                IntPtr lParam,
                ref bool handled )
        {
            if ( msg == WM_SYSCOMMAND &&
                    ( ( int )wParam & 0xFFF0 ) == SC_CONTEXTHELP )
            {
                var dialogManager = new DialogManager( this, Dispatcher );
                dialogManager
                    .CreateCustomContentDialog( new ModalDialog(), DialogMode.Ok ).Show();
                handled = true;
            }
            return IntPtr.Zero;
        }
        #endregion

        private enum proc { FCFS = 1, SJF, SRT, HRN, PRIORITY, ROUNDROBIN };
        private int select_flag = -1;   // 1:fcfs, 2:sjf, 3:srt, 4:hrn, 5:priority, 6:round_robin, 0:No page
        private bool firstRun = true;

        private FCFS        fcfs;
        private SJF         sjf;
        private SRT         srt;
        private HRN         hrn;
        private Priority    prio;
        private RoundRobin  rrb;

        private List<ProcessData>   data;
        private double[]            wait_time;
        private double[]            return_time;

        private readonly object thislock = new object();     // 임계구역 Flag (Critical Section)

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
            Init_FirstRun();
        }

        #region working delegate
        /* FCFS */
        private void Item1_Clicked()
        {
            select_flag = ( int )proc.FCFS;
            tbTitle.Content = "F C F S";
            tbDescription.Content = @":  준비 큐에 들어온 순서대로 처리합니다.";
            lfMenu.lbTimequantum.Visibility = Visibility.Hidden;
            lfMenu.sliderTimequantum.Visibility = Visibility.Hidden;
            tableProcess.UnselectAll();
            set_color( false );
        }
        /* SJF */
        private void Item2_Clicked()
        {
            select_flag = ( int )proc.SJF;
            tbTitle.Content = "S J F";
            tbDescription.Content = @":  서비스 시간이 가장 짧은 프로세스를 우선적으로 처리합니다.";
            lfMenu.lbTimequantum.Visibility = Visibility.Hidden;
            lfMenu.sliderTimequantum.Visibility = Visibility.Hidden;
            tableProcess.UnselectAll();
            set_color( false );
        }
        /* SRT */
        private void Item3_Clicked()
        {
            select_flag = ( int )proc.SRT;
            tbTitle.Content = "S R T";
            tbDescription.Content = @":  잔여 서비스 시간이 가장 짧은 프로세스를 우선적으로 처리합니다.";
            lfMenu.lbTimequantum.Visibility = Visibility.Hidden;
            lfMenu.sliderTimequantum.Visibility = Visibility.Hidden;
            tableProcess.UnselectAll();
            set_color( false );
        }
        /* HRN */
        private void Item4_Clicked()
        {
            select_flag = ( int )proc.HRN;
            tbTitle.Content = "H R N";
            tbDescription.Content = @":  스케쥴링 시점에서 높은 응답률을 가진 프로세스를 우선 처리합니다.";
            lfMenu.lbTimequantum.Visibility = Visibility.Hidden;
            lfMenu.sliderTimequantum.Visibility = Visibility.Hidden;
            tableProcess.UnselectAll();
            set_color( false );
        }
        /* Priority */
        private void Item5_Clicked()
        {
            select_flag = ( int )proc.PRIORITY;
            tbTitle.Content = "Priority";
            tbDescription.Content = @":  프로세스의 우선순위가 높은 순서(9:Highest Priority)대로 처리합니다.";
            lfMenu.lbTimequantum.Visibility = Visibility.Hidden;
            lfMenu.sliderTimequantum.Visibility = Visibility.Hidden;
            tableProcess.UnselectAll();
            set_color( true );
        }
        /* Round-Robin */
        private void Item6_Clicked()
        {
            select_flag = ( int )proc.ROUNDROBIN;
            tbTitle.Content = "Round-Robin";
            tbDescription.Content = @":  FCFS 기법으로 처리하되, CPU Time 단위로 처리합니다.";
            lfMenu.lbTimequantum.Visibility = Visibility.Visible;
            lfMenu.sliderTimequantum.Visibility = Visibility.Visible;
            tableProcess.UnselectAll();
            set_color( false );
        }

        /* Run */
        private void Run_Auto()
        {
            /* Set Critical Section */
            lock ( thislock )
            {
                try
                {
                    tableProcess.IsReadOnly = true;
                    btnRun.Visibility = Visibility.Visible;
                    CreateTables();
                    tableProcess.UnselectAll();
                    tableProcess.SelectionUnit = DataGridSelectionUnit.FullRow;
                    ChangeTables();

                    if ( select_flag == ( int )proc.PRIORITY )
                    {
                        set_color( true );
                    }
                    else
                    {
                        set_color( false );
                    }
                } catch
                {
                    MessageBox.Show( "테이블이 아직 편집상태 입니다.\n확인 후 다시 작업을 요청하십시오.", "오류", MessageBoxButton.OK, MessageBoxImage.Error );
                }
            }
        }

        private void Run_Static()
        {
            /* Set Critical Section */
            lock ( thislock )
            {
                try
                {
                    tableProcess.IsReadOnly = false;
                    PTHeader1.IsReadOnly = PTHeader2.IsReadOnly = true;
                    btnRun.Visibility = Visibility.Visible;
                    CreateTables();
                    tableProcess.UnselectAll();
                    tableProcess.SelectionUnit = DataGridSelectionUnit.Cell;
                    ChangeTables();

                    if ( select_flag == ( int )proc.PRIORITY )
                    {
                        set_color( true );
                    }
                    else
                    {
                        set_color( false );
                    }
                } catch
                {
                    MessageBox.Show( "테이블이 아직 편집상태 입니다.\n확인 후 다시 작업을 요청하십시오.", "오류", MessageBoxButton.OK, MessageBoxImage.Error );
                }
            }
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

            chartProcess.Titles.Add( "스케쥴링 결과" );
            chartWait.Titles.Add( "평균대기시간" );
            chartReturn.Titles.Add( "평균반환시간" );
        }

        /* Seeting File IO */
        public void Init_FirstRun()
        {
            try
            {
                using ( StreamReader sr = new StreamReader( "config.dat" ) )
                {
                    string flag = sr.ReadLine();
                    if ( flag == "1" )  { firstRun = false; }
                    else                { firstRun = true; }
                    sr.Close();
                }

                using ( StreamWriter sw = new StreamWriter( "config.dat" ) )
                {
                    sw.WriteLine( "1" );
                    sw.Close();
                }
            }
            catch
            {
                using ( StreamWriter sw = new StreamWriter( "config.dat" ) )
                {
                    sw.WriteLine( "1" );
                    sw.Close();
                }

                firstRun = true;
            }
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
            chartProcess.Series[ 0 ].Name = "Process";
            chartProcess.ChartAreas[ 0 ].RecalculateAxesScale();
            chartProcess.Update();
        }

        /* Wait Chart Update */
        private void WaitUpdate()
        {
            double checking = 0.0;
            string[] name = new string[] { "FCFS", "SJF", "SRT", "HRN", "Priority", "Round-Robin" };
            chartWait.Series.Clear();

            Series seriesPie = new Series();
            seriesPie.ChartType = SeriesChartType.Pie;

            for ( int i = 0; i < wait_time.Length; i++ )
            {
                seriesPie.Points.AddXY( name[ i ], wait_time[ i ] );
                checking += wait_time[ i ];
            }
            
            chartWait.Series.Add( seriesPie );
            chartWait.Series[ 0 ].IsValueShownAsLabel = true;
            chartWait.Update();

            if ( checking == 0 )
            {
                MessageBox.Show( "모든 기법의 평균대기시간이 0 이므로,\n평균대기시간 차트가 나타나지 않습니다.", "정보", MessageBoxButton.OK, MessageBoxImage.Information );
            }
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

            data.Clear();
            
            for ( int i = 0; i < limit; i++ )
            {
                if ( ( bool )topBar.tgBtn.IsChecked )
                    data.Add( new ProcessData() { no = "" + ( i + 1 ), pid = "" + ( Common.START_PID + i ), priority = "" + rand.Next( 8 ), arrived_time = "" + ( rand.Next( 10 ) + 1 ), service_time = "" + ( rand.Next( 10 ) + 1 ) } );
                else
                    data.Add( new ProcessData() { no = "" + ( i + 1 ), pid = "" + ( Common.START_PID + i ), priority = "0", arrived_time = "", service_time = "" } );
            }
            
            tableProcess.ItemsSource = data;
            tableProcess.Items.Refresh();
        }

        /* When No Running */
        private void ChangeTables()
        {
            foreach ( var i in data )
            {
                i.wait_time = i.return_time = "-";
            }
            tableProcess.ItemsSource = data;
            tableProcess.Items.Refresh();
        }

        /* After creating table, Drawing for chart */
        private void btnRun_Click( object sender, RoutedEventArgs e )
        {
            try
            {
                tableProcess.UnselectAll();
                
                if ( select_flag > 0 )
                {
                    running();

                    if ( select_flag == ( int )proc.PRIORITY )
                    {
                        set_color( true );
                    }
                    else
                    {
                        set_color( false );
                    }
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
            int time_quantum = Convert.ToInt32( lfMenu.sliderTimequantum.Value );
            //data = get_data();        // 얕은 복사 실행(이 코드에선 사용금지)
            List<ProcessData> tmp = GenericCopier<List<ProcessData>>.DeepCopy( get_data() );    // 깊은 복사 실행

            if ( !checkValues( tmp ) )
            {
                MessageBox.Show( "테이블 값의 범위는 우선순위 0~9, 나머지는 1~30 이어야 합니다.\n확인 후 다시 작업을 요청하십시오.", "오류", MessageBoxButton.OK, MessageBoxImage.Error );
                return;
            }

            List<ProcessData> data_fcfs = GenericCopier<List<ProcessData>>.DeepCopy( tmp );    // 깊은 복사 실행
            List<ProcessData> data_sjf = GenericCopier<List<ProcessData>>.DeepCopy( tmp );    // 깊은 복사 실행
            List<ProcessData> data_srt = GenericCopier<List<ProcessData>>.DeepCopy( tmp );    // 깊은 복사 실행
            List<ProcessData> data_hrn = GenericCopier<List<ProcessData>>.DeepCopy( tmp );    // 깊은 복사 실행
            List<ProcessData> data_prio = GenericCopier<List<ProcessData>>.DeepCopy( tmp );    // 깊은 복사 실행
            List<ProcessData> data_rrb = GenericCopier<List<ProcessData>>.DeepCopy( tmp );    // 깊은 복사 실행
            
            fcfs    = new FCFS( data_fcfs );
            sjf     = new SJF( data_sjf );
            srt     = new SRT( data_srt );
            hrn     = new HRN( data_hrn );
            prio    = new Priority( data_prio );
            rrb     = new RoundRobin( data_rrb, time_quantum );

            List<ProcessData> temp;

            switch ( select_flag )
            {
                case ( int )proc.FCFS:
                    temp = fcfs.working();
                    sjf.working(); srt.working();hrn.working();
                    prio.working(); rrb.working();
                    calc_time( fcfs.get_wait_time(), fcfs.get_return_time() );
                    break;
                case ( int )proc.SJF:
                    temp = sjf.working();
                    fcfs.working(); srt.working();
                    hrn.working(); prio.working(); rrb.working();
                    calc_time( sjf.get_wait_time(), sjf.get_return_time() );
                    break;
                case ( int )proc.SRT:
                    temp = srt.working();
                    fcfs.working(); sjf.working();
                    hrn.working(); prio.working(); rrb.working();
                    calc_time( srt.get_wait_time(), srt.get_return_time() );
                    break;
                case ( int )proc.HRN:
                    temp = hrn.working();
                    fcfs.working(); sjf.working(); srt.working();
                    prio.working(); rrb.working();
                    calc_time( hrn.get_wait_time(), hrn.get_return_time() );
                    break;
                case ( int )proc.PRIORITY:
                    temp = prio.working();
                    fcfs.working(); sjf.working(); srt.working();
                    hrn.working(); rrb.working();
                    calc_time( prio.get_wait_time(), prio.get_return_time() );
                    break;
                case ( int )proc.ROUNDROBIN:
                    temp = rrb.working();
                    fcfs.working(); sjf.working(); srt.working();
                    hrn.working(); prio.working();
                    calc_time( rrb.get_wait_time(), rrb.get_return_time() );
                    break;
                default:
                    MessageBox.Show( "올바르지 않은 접근입니다.\n확인 후 다시 작업을 요청하십시오.", "오류", MessageBoxButton.OK, MessageBoxImage.Error );
                    return;
            }

            wait_time[ ( int )proc.FCFS - 1 ] = Convert.ToDouble( string.Format( "{0:#0.00}", fcfs.avg_wait() ) );
            wait_time[ ( int )proc.SJF - 1 ] = Convert.ToDouble( string.Format( "{0:#0.00}", sjf.avg_wait() ) );
            wait_time[ ( int )proc.SRT - 1 ] = Convert.ToDouble( string.Format( "{0:#0.00}", srt.avg_wait() ) );
            wait_time[ ( int )proc.HRN - 1 ] = Convert.ToDouble( string.Format( "{0:#0.00}", hrn.avg_wait() ) );
            wait_time[ ( int )proc.PRIORITY - 1 ] = Convert.ToDouble( string.Format( "{0:#0.00}", prio.avg_wait() ) );
            wait_time[ ( int )proc.ROUNDROBIN - 1 ] = Convert.ToDouble( string.Format( "{0:#0.00}", rrb.avg_wait() ) );

            return_time[ ( int )proc.FCFS - 1 ] = Convert.ToDouble( string.Format( "{0:#0.00}", fcfs.avg_return() ) );
            return_time[ ( int )proc.SJF - 1 ] = Convert.ToDouble( string.Format( "{0:#0.00}", sjf.avg_return() ) );
            return_time[ ( int )proc.SRT - 1 ] = Convert.ToDouble( string.Format( "{0:#0.00}", srt.avg_return() ) );
            return_time[ ( int )proc.HRN - 1 ] = Convert.ToDouble( string.Format( "{0:#0.00}", hrn.avg_return() ) );
            return_time[ ( int )proc.PRIORITY - 1 ] = Convert.ToDouble( string.Format( "{0:#0.00}", prio.avg_return() ) );
            return_time[ ( int )proc.ROUNDROBIN - 1 ] = Convert.ToDouble( string.Format( "{0:#0.00}", rrb.avg_return() ) );

            ProcessUpdate( temp );
            WaitUpdate();
            ReturnUpdate();
        }

        private void calc_time(List<int> wait, List<int> ret)
        {
            for ( int i = 0; i < data.Count; i++ )
            {
                data[ i ].wait_time = "" + wait[ i ];
                data[ i ].return_time = "" + ret[ i ];
            }

            tableProcess.ItemsSource = data;
            tableProcess.Items.Refresh();
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

        private bool checkValues(List<ProcessData> list)
        {
            bool res = true;
            
            foreach ( ProcessData item in list )
            {
                if ( item.priority != null && item.arrived_time != null && item.arrived_time != null )
                {
                    if ( item.priority != "" && item.arrived_time != "" && item.arrived_time != "" )
                    {
                        if ( Convert.ToInt32( item.priority ) > -1 && Convert.ToInt32( item.arrived_time ) > 0 && Convert.ToInt32( item.service_time ) > 0 )
                        {
                            if ( Convert.ToInt32( item.priority ) < 10 && Convert.ToInt32( item.arrived_time ) < 31 && Convert.ToInt32( item.service_time ) < 31 )
                            {
                                res &= true;
                            }
                            else
                            {
                                res &= false;
                            }
                        }
                        else
                        {
                            res &= false;
                        }
                    }
                    else
                    {
                        res &= false;
                    }
                }
                else
                {
                    res &= false;
                }
            }

            return res;
        }

        #region Change Table Color
        /* Change Color, Change to Column hide and show */
        private void set_color( bool flag )
        {
            if ( !flag )
            {
                tableProcess.Columns[ 2 ].Visibility = Visibility.Hidden;
            }
            else
            {
                tableProcess.Columns[ 2 ].Visibility = Visibility.Visible;
            }
        }
        #endregion

        /* if when static mode, table select mode defined */
        private void tableProcess_PreviewKeyUp( object sender, KeyEventArgs e )
        {
            bool checking = true;

            foreach ( ProcessData r in tableProcess.ItemsSource )
            {
                if ( r.arrived_time == "" || r.service_time == "" )
                {
                    checking = false;
                }
            }

            if ( checking )
            {
                tableProcess.SelectionUnit = DataGridSelectionUnit.FullRow;
            }
            else
            {
                tableProcess.SelectionUnit = DataGridSelectionUnit.Cell;
            }
        }


        /* F1 key enable */
        private void Window_Help_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.Help.ShowHelp( null, @"help.chm" );
        }


        /* Modal Dialog */
        private void Window_Loaded( object sender, RoutedEventArgs e )
        {
            if ( firstRun )
            {
                var dialogManager = new DialogManager( this, Dispatcher );
                dialogManager
                    .CreateCustomContentDialog( new ModalDialog(), DialogMode.Ok ).Show();
            }
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
