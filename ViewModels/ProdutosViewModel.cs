using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using WishAWish.Models;
using WishAWish.Services;

namespace WishAWish.ViewModels
{
    public class ProdutosViewModel : BaseViewModel
    {
        private readonly ProductService _service = new ProductService();

        public ObservableCollection<Produto> Produtos { get; } = new ObservableCollection<Produto>();
        public ICollectionView ProdutosView { get; }

        private Produto _selecionado;
        public Produto Selecionado { get => _selecionado; set => Set(ref _selecionado, value); }

        public string FiltroNome { get => _filtroNome; set { if (Set(ref _filtroNome, value)) ProdutosView.Refresh(); } }
        private string _filtroNome;
        public string FiltroCodigo { get => _filtroCodigo; set { if (Set(ref _filtroCodigo, value)) ProdutosView.Refresh(); } }
        private string _filtroCodigo;
        public decimal? ValorMin { get => _valorMin; set { if (Set(ref _valorMin, value)) ProdutosView.Refresh(); } }
        private decimal? _valorMin;
        public decimal? ValorMax { get => _valorMax; set { if (Set(ref _valorMax, value)) ProdutosView.Refresh(); } }
        private decimal? _valorMax;

        public RelayCommand NovoCmd { get; }
        public RelayCommand SalvarCmd { get; }
        public RelayCommand ExcluirCmd { get; }

        public ProdutosViewModel()
        {
            ProdutosView = CollectionViewSource.GetDefaultView(Produtos);
            ProdutosView.Filter = Filter;

            NovoCmd = new RelayCommand(Novo);
            SalvarCmd = new RelayCommand(Salvar, () => Selecionado != null && Valido(Selecionado));
            ExcluirCmd = new RelayCommand(Excluir, () => Selecionado != null);

            _ = LoadAsync();
        }

        private bool Filter(object o)
        {
            var p = o as Produto;
            if (p == null) return false;
            bool ok = true;
            if (!string.IsNullOrWhiteSpace(FiltroNome)) ok &= p.Nome != null && p.Nome.ToLower().Contains(FiltroNome.ToLower());
            if (!string.IsNullOrWhiteSpace(FiltroCodigo)) ok &= p.Codigo != null && p.Codigo.ToLower().Contains(FiltroCodigo.ToLower());
            if (ValorMin.HasValue) ok &= p.Valor >= ValorMin.Value;
            if (ValorMax.HasValue) ok &= p.Valor <= ValorMax.Value;
            return ok;
        }

        private async System.Threading.Tasks.Task LoadAsync()
        {
            Produtos.Clear();
            foreach (var p in await _service.LoadAsync()) Produtos.Add(p);
            if (Produtos.Count == 0) Novo();
        }

        private void Novo() => Selecionado = new Produto();
        private bool Valido(Produto p) => !string.IsNullOrWhiteSpace(p.Nome) && !string.IsNullOrWhiteSpace(p.Codigo) && p.Valor > 0;

        private async void Salvar()
        {
            var list = Produtos.ToList();
            if (Selecionado.Id == 0)
            {
                Selecionado.Id = _service.NextId(list);
                Produtos.Add(Selecionado);
            }
            var todos = Produtos.ToList();
            await _service.SaveAsync(todos);
            ProdutosView.Refresh();
        }

        private async void Excluir()
        {
            if (Selecionado == null) return;
            Produtos.Remove(Selecionado);
            await _service.SaveAsync(Produtos.ToList());
            Selecionado = null;
        }
    }
}
