using Microsoft.AspNetCore.Http.Connections;
using System.Security.Cryptography;
using System.Text;
using VHBurguerOFC.Domains;
using VHBurguerOFC.DTOs;
using VHBurguerOFC.Exceptions;
using VHBurguerOFC.Interfaces;

namespace VHBurguerOFC.Applications.Services
{
    //service concentra o "como fazer"
    public class UsuarioService
    {
        // _repository é o canal para acessar os dados
        private readonly IUsuarioRepository _repository;

        //injecao de dependencias 
        //implementamos o repositorio e o service so depende da interface
        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }
        
        //Por que private?
        //pq o metodo nao e regra de negocio e nao faz sentido existir fora do UsuarioService
        private static LerUsuarioDto LerDto(Usuario usuario) //pega a entidade e gera um DTO
        {
            LerUsuarioDto lerUsuario = new LerUsuarioDto
            {
                UsuarioID = usuario.UsuarioID,
                Nome = usuario.Nome,
                Email = usuario.Email,
                StatusUsuario = usuario.StatusUsuario ?? true // se nao tiver status no banco, deixa como true
            };
        }

        public List<LerUsuarioDto> Listar()
        {
            List<Usuario> usuarios = _repository.Listar();

            List<LerUsuarioDto> usuarioDto = usuarios
                    .Select(usuarioBanco => LerDto(usuario)) //SELECT que percorre cada Usuario e LerDto(usuario)
                    .ToList(); // ToList() -> devolve uma lista de DTOs
            return usuariosDto;
        }

        private static void ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            {
                throw new DomainException("Email invalido");           
            }
        }

        private static byte[] HashSenha(string senha)
        {
            if(string.IsNullOrWhiteSpace(senha)) // garante que a senha nao esta vazia
            {
                throw new DomainException("Senha é obrigatoria!");
            }

            using var sha256 = SHA256.Create(); // gera um hash SHA256 e devolve em byte[]
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
        }

        public LerUsuarioDto ObterPorId(int Id)
        {
            Usuario usuario = _repository.ObterPorId(Id);

            if(usuario == null)
            {
                throw new DomainException("Usuario não existe.");
            }

            return LerDto(usuario); // se existe usuario, converte para DTO e devolve usuario
        }

        public LerUsuarioDto ObterPorEmail(string email)
        {
            Usuario? usuario = _repository.ObterPorEmail(email);

            if (usuario == null)
            {
                throw new DomainException("Usuario não existe.");
            }

            return LerDto(usuario); // se existe usuario, converte para DTO e devolve usuario
        }
    }
}
