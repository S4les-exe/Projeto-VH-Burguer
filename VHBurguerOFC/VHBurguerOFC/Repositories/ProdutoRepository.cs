using Microsoft.EntityFrameworkCore;
using VHBurguerOFC.Contexts;
using VHBurguerOFC.Domains;
using VHBurguerOFC.Interfaces;

namespace VHBurguerOFC.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly VH_BurguerContext _context;

        public ProdutoRepository(VH_BurguerContext context)
        {
            _context = context;
        }

        public List<Produto> Listar()
        {
            List<Produto> produtos = _context.Produto
                .Include(produto => produto.Categoria) //busca produtos e para cada produto tras as suas categorias
                .Include(produto => produto.Usuario) //busca produtos e para cada produto, traz seus usuarios
                .ToList();

            return produtos;
        }

        public Produto ObterPorId(int id)
        {
            Produto produtos = _context.Produto
                .Include(produtoDb => produtoDb.Categoria)
                .Include(produtoDb => produtoDb.Usuario)
                // Procura no banco (aux produtoDb) e verifica se o ID do produto no banco eh igual ao id passado como parametro no metodo ObterPorId
                .FirstOrDefault(produtoDb => produtoDb.ProdutoID  == id);
            
            return produto;
        }

        public bool NomeExiste(string nome, int? produtoIdAtual = null)
        {
            // AsQueryable() -> monta a consulta para executar passo a passo 
            // monta a consulta na tabela produto
            // nao executa nada no banco ainda
            var produtoConsultado = _context.Produto.AsQueryable();

            // Se o produto atual tiver valor, entao atualizaremos o produto

            if(produtoIdAtual.HasValue)
            {
                produtoConsultado = produtoConsultado.Where(produto => produto.ProdutoID != produtoIdAtual.Value);
            }

            return produtoConsultado.Any(produto => produto.Nome == nome);
        }

        public byte[] ObterImagem(int id)
        {
            var produto = _context.Produto
                .Where(produto => produto.ProdutoID == id)
                .Select(produto => produto.Imagem)
                .FirstOrDefault();

            return produto;
        }

        public void Adicionar(Produto produto, List<int> categoriaIds)
        {
            List<Categoria> categorias = _context.Categoria
                .Where(categoria => categoriaIds.Contains(categoria.CategoriaID))
                .ToList(); // Contains -> retorna true se houver o registro

            produto.Categoria = categorias; // adiciona as categorias incluidas ao produto 

            _context.Produto.Add(produto);
            _context.SaveChanges(); 
        }

        public void Atualizar (Produto produto, List<int> categoriaIds)
        {
            Produto? produtoBanco = _context.Produto
                .Include(produto => produto.Categoria)
                .FirstOrDefault(produtoAux => produtoAux.ProdutoID == produto.ProdutoID);

            if(produtoBanco == null)
            {
                return;
            }

            produtoBanco.Nome = produto.Nome;
            produtoBanco.Preco = produto.Preco;
            produtoBanco.Descricao = produto.Descricao;

            if (produto.Imagem != null && produto.Imagem.Length > 0)
            {
                //Se a imagem nao for nula e tiver tamanho maior que zero, o produto no banco vai receber a imagem que foi pássada
                produtoBanco.Imagem = produto.Imagem;
            }

            if(produto.StatusProduto.HasValue)
            {
                produtoBanco.StatusProduto = produto.StatusProduto;
            }

            // busca todas as categorias no banco com o id igual das categorias que vieram da requisicao/front
            var categoria = _context.Categoria
                .Where(categoria => categoriaIds.Contains(categoria.CategoriaID))
                .ToList();

            // Clear() -> Remove as ligacoes atuais entre o produto e as categorias 
            // ele nao apaga as categorias do banco, so remove o vinculo com a tabela ProdutoCategoria
            produtoBanco.Categoria.Clear();

            foreach(var categoria in categorias)
            {
                produtoBanco.Categoria.Add(categoria);
            }

            _context.SaveChanges();
        }

        public void Remove(int id)
        {
            Produto? produto = _context.Produto.FirstOrDefault(produto => produto.ProdutoID == id);

            if(produto == null)
            {
                return;
            }

            _context.Produto.Remove(produto);
            _context.SaveChanges();
        }
    }
}
