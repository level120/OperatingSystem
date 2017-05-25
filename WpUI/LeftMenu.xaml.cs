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
    /// LeftMenu.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LeftMenu : UserControl
    {
        public event item1_Click _item1_Click;
        public event item2_Click _item2_Click;
        public event item3_Click _item3_Click;
        public event item4_Click _item4_Click;
        public event item5_Click _item5_Click;
        public event item6_Click _item6_Click;

        public LeftMenu()
        {
            InitializeComponent();
        }

        private void item1_PreviewMouseDown( object sender, MouseButtonEventArgs e )
        {
            _item1_Click?.Invoke();
        }

        private void item2_PreviewMouseDown( object sender, MouseButtonEventArgs e )
        {
            _item2_Click?.Invoke();
        }

        private void item3_PreviewMouseDown( object sender, MouseButtonEventArgs e )
        {
            _item3_Click?.Invoke();
        }

        private void item4_PreviewMouseDown( object sender, MouseButtonEventArgs e )
        {
            _item4_Click?.Invoke();
        }

        private void item5_PreviewMouseDown( object sender, MouseButtonEventArgs e )
        {
            _item5_Click?.Invoke();
        }

        private void item6_PreviewMouseDown( object sender, MouseButtonEventArgs e )
        {
            _item6_Click?.Invoke();
        }
    }
}
