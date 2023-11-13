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

namespace To_Do_List_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string directory = "C:\\Users\\spenc\\Documents\\My Files\\Repository\\Lists";

        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public class Parser
    {
        private string _directory;

        public Parser(string directory)
        {
            _directory = directory;
        }

        public void ParseDirectory()
        {

        }

        public void ParseFile()
        {

        }
    }
}
