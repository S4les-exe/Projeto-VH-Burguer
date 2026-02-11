using VHBurguerOFC.Contexts;
using VHBurguerOFC.Domains;
using VHBurguerOFC.Interfaces;

namespace VHBurguerOFC.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly VH_BurguerContext _context;

        public UsuarioRepository(VH_BurguerContext context)
        {
            _context = context;
        }

        public List<Usuario> Listar()
        {
            return _context.Usuario.ToList();
        }

        public Usuario? ObterPorId(int id)
        {
            // find performa melhor com chave primaria
            return _context.Usuario.Find(id);
        }

        public Usuario? ObterPorEmail(string email)
        {
            // FirstOrDefault -> retorna nosso usuario de banco
            return _context.Usuario.FirstOrDefault(usuario => usuario.Email == email);
        }

        public bool EmailExiste(string email)
        {
            // Any -> retorna um true ou false para validar se existe ALGUM usuario com esse email
            return _context.Usuario.Any(usuario => usuario.Email == email);
        }

        public void Adicionar(Usuario usuario)
        {
            _context.Usuario.Add(usuario);  
            _context.SaveChanges(); 
        }

        public void Atualizar(Usuario usuario)
        {
            Usuario? usuarioBanco = _context.Usuario.FirstOrDefault(usuarioAuxiliar => usuarioAuxiliar.UsuarioID == usuario.UsuarioID);

            if (usuarioBanco == null)
            {
                return;
            }

            usuarioBanco.Nome = usuario.Nome;
            usuarioBanco.Email = usuario.Email;
            usuarioBanco.Senha = usuario.Senha;

            _context.SaveChanges();
        }

        public void Remover(int Id)
        {
            Usuario? usuario = _context.Usuario.FirstOrDefault(usuarioAuxiliar => usuarioAuxiliar.UsuarioID == id);

            if(usuario == null)
            {
                return;
            }

            _context.Usuario.Remove(usuario);
            _context.SaveChanges();
        }
    }
}
