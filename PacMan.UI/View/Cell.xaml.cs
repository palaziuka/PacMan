﻿using PacMan.Logic.Model;
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

namespace PacMan.UI.View
{
    /// <summary>
    /// Interaction logic for Cell.xaml
    /// </summary>
    public partial class Cell : UserControl
    {
        public int Col { get; private set; }
        public int Row { get; private set; }
        // конструктор по умолчанию нужен
        public Cell() : this(-1, -1)
        {
        }
       // public static readonly DependencyProperty CellProperty =
       //DependencyProperty.Register("_Cell", typeof(string), typeof(Cell));

       // public int _Cell
       // {
       //     get { return (int)GetValue(CellProperty); }
       //     set { SetValue(CellProperty, value); }
       // }
        public Cell(int Col, int Row)
        {
            InitializeComponent();
            this.Col = Col;
            this.Row = Row;
           // ContentElement.SetValue(CellProperty,"i="+Col+" j="+Row);
        }
       
    }
}