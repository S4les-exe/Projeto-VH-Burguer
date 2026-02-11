using VHBurguerOFC.Domains;

namespace VHBurguerOFC.Interfaces
{
    public interface IUsuarioRepository
    {
        List<Usuario> Lista();

        //pode ser que nao venha nenhum usuario na busca, entao colocamos ?
        Usuario? ObterPorId(int Id);

        Usuario? ObterporEmail(string Email);

        bool EmailExiste(string Email);

        void Adicionar(Usuario usuario);

        void Atualizar(Usuario usuario);

        void Remover(int Id);



    }
}
