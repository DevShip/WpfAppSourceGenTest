using WpfApp.ViewModels;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MainViewModel _viewModel = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }
    }
}
