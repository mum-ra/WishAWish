using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WishAWish.Models;
using WishAWish.Services;
using WishAWish.Views;

namespace WishAWish.ViewModels
{
    public class PessoasViewModel : BaseViewModel
    {
        private readonly PersonService _people = new PersonService();
        private readonly OrderService _orders = new OrderService();
        private readonly ProductService _products = new ProductService();

        public ObservableCollection<Pessoa> Pessoas { get; } = new ObservableCollection<Pessoa>();
        public ICollectionView PessoasView { get; }

        public ObservableCollection<Pedido> PedidosDaPessoa { get; } = new ObservableCollection<Pedido>();
        public ICollectionView PedidosDaPessoaView { get; }

        private Pessoa _selecionada;
        public Pessoa Selecionada
        {
            get => _selecionada;
            set { if (Set(ref _selecionada, value)) { CarregarPedidosDaPessoa(); CommandManager.InvalidateRequerySuggested(); } }
        }

        public string FiltroNome { get => _filtroNome; set { if (Set(ref _filtroNome, value)) PessoasView.Refresh(); } }
        private string _filtroNome;
        public string FiltroCpf  { get => _filtroCpf;  set { if (Set(ref _filtroCpf,  value)) PessoasView.Refresh(); } }
        private string _filtroCpf;

        public bool MostrarEntregues { get => _mostrarEntregues; set { if (Set(ref _mostrarEntregues, value)) PedidosDaPessoaView.Refresh(); } }
        private bool _mostrarEntregues;
        public bool MostrarPagos { get => _mostrarPagos; set { if (Set(ref _mostrarPagos, value)) PedidosDaPessoaView.Refresh(); } }
        private bool _mostrarPagos;
        public bool MostrarPendentes { get => _mostrarPendentes; set { if (Set(ref _mostrarPendentes, value)) PedidosDaPessoaView.Refresh(); } }
        private bool _mostrarPendentes;

        public RelayCommand NovoCmd { get; }
        public RelayCommand SalvarCmd { get; }
        public RelayCommand ExcluirCmd { get; }
        public RelayCommand IncluirPedidoCmd { get; }
        public RelayCommand MarcarPagoPessoaCmd { get; }
        public RelayCommand MarcarEnviadoPessoaCmd { get; }
        public RelayCommand MarcarRecebidoPessoaCmd { get; }

        public PessoasViewModel()
        {
            PessoasView = CollectionViewSource.GetDefaultView(Pessoas);
            PessoasView.Filter = o =>
            {
                var p = o as Pessoa; if (p == null) return false;
                bool ok = true;
                if (!string.IsNullOrWhiteSpace(FiltroNome)) ok &= (p.Nome ?? "").ToLower().Contains(FiltroNome.ToLower());
                if (!string.IsNullOrWhiteSpace(FiltroCpf))  ok &= (p.Cpf ?? "").Contains(FiltroCpf);
                return ok;
            };

            PedidosDaPessoaView = CollectionViewSource.GetDefaultView(PedidosDaPessoa);
            PedidosDaPessoaView.Filter = PedidoFiltro;

            NovoCmd = new RelayCommand(() => Selecionada = new Pessoa());
            SalvarCmd = new RelayCommand(Salvar, () => Selecionada != null && ValidaPessoa(Selecionada));
            ExcluirCmd = new RelayCommand(Excluir, () => Selecionada != null && Selecionada.Id != 0);

            IncluirPedidoCmd = new RelayCommand(IncluirPedido, () => Selecionada != null && Selecionada.Id != 0);

            MarcarPagoPessoaCmd     = new RelayCommand(o => MarcarStatus(o, StatusPedido.Pago));
            MarcarEnviadoPessoaCmd  = new RelayCommand(o => MarcarStatus(o, StatusPedido.Enviado));
            MarcarRecebidoPessoaCmd = new RelayCommand(o => MarcarStatus(o, StatusPedido.Recebido));

            _ = LoadAsync();
        }

        private bool PedidoFiltro(object o)
        {
            var p = o as Pedido; if (p == null) return false;
            bool ok = p.PessoaId == (Selecionada?.Id ?? -1);
            if (MostrarEntregues) ok &= p.Status == StatusPedido.Recebido;
            if (MostrarPagos) ok &= p.Status == StatusPedido.Pago;
            if (MostrarPendentes) ok &= p.Status == StatusPedido.Pendente;
            return ok;
        }

        private async System.Threading.Tasks.Task LoadAsync()
        {
            Pessoas.Clear();
            foreach (var p in await _people.LoadAsync()) Pessoas.Add(p);
            PessoasView.Refresh();
        }

        private bool ValidaPessoa(Pessoa p)
        {
            if (string.IsNullOrWhiteSpace(p.Nome)) return false;
            if (string.IsNullOrWhiteSpace(p.Cpf) || !Cpf.Valido(p.Cpf)) return false;
            return true;
        }

        private async void Salvar()
        {
            var list = Pessoas.ToList();
            if (Selecionada.Id == 0)
            {
                Selecionada.Id = _people.NextId(list);
                Pessoas.Add(Selecionada);
            }
            await _people.SaveAsync(Pessoas.ToList());
            PessoasView.Refresh();
        }

        private async void Excluir()
        {
            if (Selecionada == null) return;
            Pessoas.Remove(Selecionada);
            await _people.SaveAsync(Pessoas.ToList());
            Selecionada = null;
        }

        private async void IncluirPedido()
        {
            if (Selecionada == null) return;

            var vm = new PedidoEditorViewModel(Selecionada);
            var win = new PedidoEditorWindow { DataContext = vm, Owner = Application.Current.MainWindow };
            var ok = win.ShowDialog() ?? false;
            if (ok) await CarregarPedidosDaPessoa(true);
        }

        private async System.Threading.Tasks.Task CarregarPedidosDaPessoa(bool reload = false)
        {
            if (reload) { /* no-op */ }
            PedidosDaPessoa.Clear();
            foreach (var p in (await _orders.LoadAsync()).Where(p => p.PessoaId == (Selecionada?.Id ?? -1)))
                PedidosDaPessoa.Add(p);
            PedidosDaPessoaView.Refresh();
        }

        private async void MarcarStatus(object pedidoObj, StatusPedido novo)
        {
            var ped = pedidoObj as Pedido; if (ped == null) return;
            ped.Status = novo;

            var todos = await _orders.LoadAsync();
            var idx = todos.ToList().FindIndex(x => x.Id == ped.Id);
            if (idx >= 0) todos[idx] = ped;
            await _orders.SaveAsync(todos);
            PedidosDaPessoaView.Refresh();
        }
    }
}
