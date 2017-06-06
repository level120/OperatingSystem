﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace WpUI
{
    /// <summary>
    /// ModalDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModalDialog : UserControl
    {
        public ModalDialog()
        {
            InitializeComponent();
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Help.ShowHelp(null, @"help.chm");
        }
    }
}
