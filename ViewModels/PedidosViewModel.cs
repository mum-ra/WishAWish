using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using WishAWish.Models;
using WishAWish.Services;

namespace WishAWish.ViewModels
{
    public class PedidosViewModel : BaseViewModel
    {
        private readonly OrderService _orders = new OrderService();

        public ObservableCollection<Pedido> Pedidos { get; } = new ObservableCollection<Pedido>();
        public ICollectionView PedidosView { get; }

        public Pedido Selecionado { get => _selecionado; set => Set(ref _selecionado, value); }
        private Pedido _selecionado;

        public bool SomenteEntregues { get => _somenteEntregues; set { if (Set(ref _somenteEntregues, value)) PedidosView.Refresh(); } }
        private bool _somenteEntregues;
        public bool SomentePagos { get => _somentePagos; set { if (Set(ref _somentePagos, value)) PedidosView.Refresh(); } }
        private bool _somentePagos;
        public bool SomentePendentesPagamento { get => _somentePend; set { if (Set(ref _somentePend, value)) PedidosView.Refresh(); } }
        private bool _somentePend;

        public RelayCommand MarcarPagoCmd { get; }
        public RelayCommand MarcarEnviadoCmd { get; }
        public RelayCommand MarcarRecebidoCmd { get; }

        public PedidosViewModel()
        {
            PedidosView = CollectionViewSource.GetDefaultView(Pedidos);
            PedidosView.Filter = o =>
            {
                var p = o as Pedido; if (p == null) return false;
                bool ok = true;
                if (SomenteEntregues) ok &= p.Status == StatusPedido.Recebido;
                if (SomentePagos) ok &= p.Status == StatusPedido.Pago;
                if (SomentePendentesPagamento) ok &= p.Status == StatusPedido.Pendente;
                return ok;
            };

            MarcarPagoCmd     = new RelayCommand(_ => AtualizarStatus(Selecionado, StatusPedido.Pago),     _ => Selecionado != null);
            MarcarEnviadoCmd  = new RelayCommand(_ => AtualizarStatus(Selecionado, StatusPedido.Enviado),  _ => Selecionado != null);
            MarcarRecebidoCmd = new RelayCommand(_ => AtualizarStatus(Selecionado, StatusPedido.Recebido), _ => Selecionado != null);

            _ = LoadAsync();
        }

        private async System.Threading.Tasks.Task LoadAsync()
        {
            Pedidos.Clear();
            foreach (var p in await _orders.LoadAsync()) Pedidos.Add(p);
            PedidosView.Refresh();
        }

        private async void AtualizarStatus(Pedido p, StatusPedido status)
        {
            if (p == null) return;
            p.Status = status;
            var todos = await _orders.LoadAsync();
            var idx = todos.ToList().FindIndex(x => x.Id == p.Id);
            if (idx >= 0) todos[idx] = p;
            await _orders.SaveAsync(todos);
            PedidosView.Refresh();
        }
    }
}
