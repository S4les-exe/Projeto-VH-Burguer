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
            return lerUsuario;
        }

        public List<LerUsuarioDto> Listar()
        {
            List<Usuario> usuarios = _repository.Listar();

            List<LerUsuarioDto> usuariosDto = usuarios
                    .Select(usuarioBanco => LerDto(usuarioBanco)) //SELECT que percorre cada Usuario e LerDto(usuario)
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
            if (string.IsNullOrWhiteSpace(senha)) // garante que a senha nao esta vazia
            {
                throw new DomainException("Senha é obrigatoria!");
            }

            using var sha256 = SHA256.Create(); // gera um hash SHA256 e devolve em byte[]
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(senha));
        }

        public LerUsuarioDto ObterPorId(int Id)
        {
            Usuario? usuario = _repository.ObterPorId(Id);

            if (usuario == null)
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

        public LerUsuarioDto Adicionar(CriarUsuarioDto usuarioDto)
        {
            ValidarEmail(usuarioDto.Email);

            if (_repository.EmailExiste(usuarioDto.Email))
            {
                throw new DomainException("Ja existe um usuario com este e-mail");
            }

            Usuario usuario = new Usuario // criando entidade usuario
            {
                Nome = usuarioDto.Nome,
                Email = usuarioDto.Email,
                Senha = HashSenha(usuarioDto.Senha),
                StatusUsuario = true
            };
            _repository.Adicionar(usuario);

            return LerDto(usuario); // retorna a LerDto para não retornar objeto com a senha.
        }

        public LerUsuarioDto Atualizar(int id, CriarUsuarioDto usuarioDto)
        {
            ValidarEmail(usuarioDto.Email);

            Usuario? usuarioBanco = _repository.ObterPorId(id);

            if (usuarioBanco == null)
            {
                throw new DomainException("Usuario nao encontrado.");
            }

            ValidarEmail(usuarioDto.Email);

            Usuario? usuarioComMesmoEmail = _repository.ObterPorEmail(usuarioDto.Email);

            if (usuarioComMesmoEmail != null && usuarioComMesmoEmail.UsuarioID != id)
            {
                throw new DomainException("Ja existe um usuario com este email!");
            }

            //Substitui as informações do banco(usuarioBanco)
            //Inserindo as alteraçoes que estao vindo de usuarioDto
            usuarioBanco.Nome = usuarioDto.Nome;
            usuarioBanco.Email = usuarioDto.Email;
            usuarioBanco.Senha = HashSenha(usuarioDto.Senha);

            _repository.Atualizar(usuarioBanco);

            return LerDto(usuarioBanco);
        }

        public void Remover(int id)
        {
            Usuario? usuario = _repository.ObterPorId(id);

            if (usuario == null)
            {
                throw new DomainException("Usuario nao encontrado");
            }

            _repository.Remover(id);
        }
    }
}
