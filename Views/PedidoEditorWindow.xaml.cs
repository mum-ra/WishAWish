using System.ComponentModel;
using System.Windows;
using WishAWish.ViewModels;

namespace WishAWish.Views
{
    public partial class PedidoEditorWindow : Window
    {
        public PedidoEditorWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object s, RoutedEventArgs e)
        {
            var vm = DataContext as PedidoEditorViewModel;
            if (vm != null)
                vm.PropertyChanged += VmOnPropertyChanged;
        }

        private void VmOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vm = sender as PedidoEditorViewModel;
            if (vm != null && e.PropertyName == nameof(PedidoEditorViewModel.Finalizado) && vm.Finalizado)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}
