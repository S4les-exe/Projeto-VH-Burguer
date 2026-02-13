using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VHBurguerOFC.Applications.Services;
using VHBurguerOFC.DTOs;
using VHBurguerOFC.Exceptions;

namespace VHBurguerOFC.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _service;

        public UsuarioController(UsuarioService service)
        {
            _service = service;
        }

        // GET -> Lista informações 
        [HttpGet]
        public ActionResult<List<LerUsuarioDto>> Listar()
        {
            List<LerUsuarioDto> usuarios = _service.Listar();
            // retorna a lista de usuarios, a partir da DTO de services 
            return Ok(usuarios); // OK - 200 - DEU CERTO
        }

        [HttpGet("{id}")]
        public ActionResult<LerUsuarioDto> ObterPorId(int id)
        {
            LerUsuarioDto usuario = _service.ObterPorId(id);
            if (usuario == null)
            {
                return NotFound(); // NAO ENCONTRADO - StatusCode 404
            }
            return Ok(usuario);
        }

        [HttpGet("email/{email}")] /* https://senai.com/email/arthur.sales@gmail.com (exenplo)*/
        
        public ActionResult<LerUsuarioDto> ObterPorEmail(string email) 
        {
            LerUsuarioDto usuario = _service.ObterPorEmail(email);

            if(usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        // POST - Envia Dados
        [HttpPost]
        public ActionResult<LerUsuarioDto> Adicionar(CriarUsuarioDto usuarioDto)
        {
            try
            {
                LerUsuarioDto usuarioCriado = _service.Adicionar(usuarioDto);
                return StatusCode(201, usuarioCriado); //retorna Status Code Criado
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message); 
            }
        }

        //Update dos dados
        [HttpPut("{id}")]
        public ActionResult<LerUsuarioDto> Atualizar(int id, CriarUsuarioDto usuarioDto)
        {
            try
            {
                LerUsuarioDto usuarioAtualizado = _service.Atualizar(id, usuarioDto);

                return StatusCode(200, usuarioAtualizado);
            }
            catch(DomainException ex)
            {
                return BadRequest(ex.Message); 
            }
        }

        //Remove os dados
        // no nosso banco o delete vai inativar o usuario por conta da trigger(processo chamado de soft delete)
        [HttpDelete("{id}")]
        public ActionResult Remover(int id)
        {
            try
            {
                _service.Remover(id);
                return NoContent();
            }
            catch(DomainException ex)
            {
                return BadRequest(ex.Message); 
            }
        }
    }
}
