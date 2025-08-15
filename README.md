# WishAWish (WPF .NET Framework 4.6.2)

App de cadastro de Pessoas, Produtos e Pedidos.
- Persistência em JSON (pasta /Data)
- LINQ para filtros e cálculos
- MVVM simples (ObservableCollection + ICollectionView)

## Requisitos
- Visual Studio 2022 com workload "Desktop development with .NET"
- .NET Framework 4.6.2 targeting pack

## Rodar
1. Abra a solução.
2. Build e Start (F5).
3. Cadastre Pessoas e Produtos.
4. Em Pessoas > "Incluir Pedido" para abrir o editor; adicione itens, escolha forma de pagamento e finalize.

## Estrutura
- Models: Pessoa, Produto, Pedido, ItemPedido, Enums
- Services: JsonRepository<T>, PersonService, ProductService, OrderService
- ViewModels: PessoasViewModel, ProdutosViewModel, PedidosViewModel, PedidoEditorViewModel
- Views: PessoasView, ProdutosView, PedidosView, PedidoEditorWindow
