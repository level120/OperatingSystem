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
using static WpUI.MainWindow;

namespace WpUI
{
    /// <summary>
    /// TopStatus.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TopStatus : UserControl
    {
        public event run_Auto _runAuto;
        public event run_Static _runStatic;

        public TopStatus()
        {
            InitializeComponent();
        }

        private void Button_PreviewMouseDown( object sender, MouseButtonEventArgs e )
        {
            if ( ( bool )tgBtn.IsChecked )
            {
                _runAuto?.Invoke();
            }
            else
            {
                _runStatic?.Invoke();
            }
        }
    }
}
