using VHBurguerOFC.Domains;

namespace VHBurguerOFC.Interfaces
{
    public interface IUsuarioRepository
    {
        List<Usuario> Listar();

        //pode ser que nao venha nenhum usuario na busca, entao colocamos ?
        Usuario? ObterPorId(int Id);

        Usuario? ObterPorEmail(string Email);

        bool EmailExiste(string Email);

        void Adicionar(Usuario usuario);

        void Atualizar(Usuario usuario);

        void Remover(int Id);



    }
}
