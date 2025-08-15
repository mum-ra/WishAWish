using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WishAWish.Models;
using WishAWish.Services;

namespace WishAWish.ViewModels
{
    public class PedidoEditorViewModel : BaseViewModel
    {
        private readonly ProductService _products = new ProductService();
        private readonly OrderService _orders = new OrderService();

        public Pessoa Pessoa { get; }
        public ObservableCollection<Produto> Produtos { get; } = new ObservableCollection<Produto>();
        public ObservableCollection<ItemPedido> Itens { get; } = new ObservableCollection<ItemPedido>();

        public Produto ProdutoSel { get => _produtoSel; set => Set(ref _produtoSel, value); }
        private Produto _produtoSel;
        public int Quantidade { get => _qtd; set => Set(ref _qtd, value); }
        private int _qtd = 1;

        public FormaPagamento Forma { get => _forma; set => Set(ref _forma, value); }
        private FormaPagamento _forma;

        public decimal Total => Itens.Sum(i => i.Subtotal);
        public bool Finalizado { get => _finalizado; set => Set(ref _finalizado, value); }
        private bool _finalizado;

        public ICommand AdicionarItemCmd { get; }
        public ICommand RemoverItemCmd { get; }
        public ICommand FinalizarCmd { get; }

        public PedidoEditorViewModel(Pessoa pessoa)
        {
            Pessoa = pessoa;
            AdicionarItemCmd = new RelayCommand(AdicionarItem, PodeAdicionar);
            RemoverItemCmd = new RelayCommand(o => RemoverItem((int)o));
            FinalizarCmd = new RelayCommand(Finalizar, PodeFinalizar);

            _ = LoadAsync();
            Itens.CollectionChanged += (s, e) => Raise(nameof(Total));
        }

        private async System.Threading.Tasks.Task LoadAsync()
        {
            foreach (var p in await _products.LoadAsync()) Produtos.Add(p);
        }

        private bool PodeAdicionar() => ProdutoSel != null && Quantidade > 0;
        private void AdicionarItem()
        {
            var existente = Itens.FirstOrDefault(i => i.ProdutoId == ProdutoSel.Id);
            if (existente == null)
                Itens.Add(new ItemPedido { ProdutoId = ProdutoSel.Id, ProdutoNome = ProdutoSel.Nome, ValorUnitario = ProdutoSel.Valor, Quantidade = Quantidade });
            else
                existente.Quantidade += Quantidade;
            Raise(nameof(Total));
        }

        private void RemoverItem(int produtoId)
        {
            var item = Itens.FirstOrDefault(i => i.ProdutoId == produtoId);
            if (item != null) Itens.Remove(item);
            Raise(nameof(Total));
        }

        private bool PodeFinalizar() => Pessoa != null && Itens.Count > 0;
        private async void Finalizar()
        {
            var todos = (await _orders.LoadAsync()).ToList();
            var novo = new Pedido
            {
                Id = _orders.NextId(todos),
                PessoaId = Pessoa.Id,
                PessoaNome = Pessoa.Nome,
                FormaPagamento = Forma,
                Itens = Itens.ToList()
            };
            todos.Add(novo);
            await _orders.SaveAsync(todos);
            Finalizado = true;
        }
    }
}
