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
using System.Windows.Shapes;
using Referring.Core;

namespace Referring.Client
{
    /// <summary>
    /// Логика взаимодействия для EssayWindow.xaml
    /// </summary>
    public partial class EssayWindow : Window
    {
        public EssayWindow()
        {
            InitializeComponent();

            essayTextBox.DataContext = ReferringManager.Instance;

            closeButton.Click += CloseButton_Click;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
