using System;
using System.Collections.Generic;
using System.IO;
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
using Markdig;
using Markdig.Parsers;
using Markdig.Syntax;

namespace Crater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string directory = "C:\\Users\\spenc\\source\\repos\\To-Do List App\\To-Do List App\\Template.md";

        public MainWindow()
        {
            InitializeComponent();
            ParseFile(directory);
        }

        public void ParseFile(string filepath)
        {
            ListParser listParser = new ListParser();
            List list = listParser.CreateFromFilepath(filepath);
        }
    }
}
