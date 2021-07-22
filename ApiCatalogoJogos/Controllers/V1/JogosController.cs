using ApiCatalogoJogos.Exceptions;
using ApiCatalogoJogos.InputModel;
using ApiCatalogoJogos.Services;
using ApiCatalogoJogos.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogoJogos.Controllers.V1
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class JogosController : ControllerBase
    {
        private readonly IJogoservice _jogoService;

        public JogosController(IJogoservice jogoService)
        {
            _jogoService = jogoService;
        }

        /// <summary>
        /// Buscar todos os jogos de forma paginada
        /// </summary>
        /// <remarks>
        /// Não é possivel retornar os jogos sem paginação
        /// </remarks>
        /// <param name="pagina">Indica qual página está sendo consultada</param>
        /// <param name="quantidade">Indica a quantidade de resgistros por página. Min 1 e Max 50</param>
        /// <response code="200">Retorna a lista de jogos</response>
        /// <response code="204">Caso não haja jogos</response>
        [HttpGet] //retorna dados
        public async Task<ActionResult<IEnumerable<JogoViewModel>>> Obter([FromQuery, Range(1, int.MaxValue)] int pagina = 1, [FromQuery, Range(1, 50)] int quantidade = 5)
        {
            var jogos = await _jogoService.Obter(pagina, quantidade);

            if (jogos.Count() == 0)
                return NoContent();
            return Ok(jogos);
        }

        [HttpGet("{idJogo:guid}")] 
        public async Task<ActionResult<JogoViewModel>> Obter([FromRoute] Guid idJogo)
        {
            var jogo = await _jogoService.Obter(idJogo);

            if (jogo == null)
                return NoContent();
            return Ok(jogo);  
        }

        [HttpPost] //cria dados
        public async Task<ActionResult<JogoViewModel>> InserirJogo([FromBody] JogoInputModel jogoInputModel)
        {
            try
            {
                var jogo = await _jogoService.Inserir(jogoInputModel);
                return Ok(jogo);
            }
            //catch (JogoJaCadastradoException ex)
            catch(JogoJaCadastradoException ex)
            {
                return UnprocessableEntity("Já existe um jogo com este nome e produtora");
            }
        }
        
        [HttpPut("{idJogo:guid}")] //atualiza dados completo
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid idJogo, [FromBody] JogoInputModel jogoInputModel)
        {
            try
            {
                await _jogoService.Atualizar(idJogo, jogoInputModel);
                return Ok();
            }
            catch(JogoNaoCadastradoException ex)
            {
                return NotFound("Não existe este jogo.");
            }
        }

        [HttpPatch("{idJogo:guid}/preco/{preco:double}")] //atualiza uma parte dos dados
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid idJogo, [FromRoute] double preco)
        {
            try
            {
                await _jogoService.Atualizar(idJogo, preco);
                return Ok();
            }
            catch(JogoNaoCadastradoException ex)
            {
                return NotFound("Jogo não encontrado.");
            }
        }

        [HttpDelete("{idJogo:guid}")] //deleta dados completo
        public async Task<ActionResult> ApagarJogo([FromRoute] Guid idJogo)
        {
            try
            {
                await _jogoService.Remover(idJogo);
                return Ok();
            }
            catch(JogoNaoCadastradoException ex)
            {
                return NotFound("Jogo não encontrado");
            }
        }

    }
}
