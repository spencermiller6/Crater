using System.Windows;
using System.Windows.Documents;
using Crater.Models;

namespace Crater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string directory = "C:\\Users\\spenc\\source\\repos\\Crater\\Crater\\Template.md";

        public MainWindow()
        {
            InitializeComponent();
            ParseFile(directory);
        }

        public void ParseFile(string filepath)
        {
            ListParser listParser = new ListParser();
            CraterList list = listParser.CreateFromFilepath(filepath);
        }
    }
}
