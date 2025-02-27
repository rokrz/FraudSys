using FraudSys.Model;
using FraudSys.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace FraudSys.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransacaoController : ControllerBase
    {
        private readonly IClienteRepository _repository;

        public TransacaoController(IClienteRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> ValidaTransacao(TransacaoModel transacao)
        {
            if (transacao != null)
            {
                if (transacao.ValidaDadosTransacao())
                {
                    var cliente = await _repository.Buscar(transacao.NumeroAgenciaOrigem, transacao.CPFOrigem);
                    if (cliente!=null)
                    {
                        if (cliente.LimitePIXAtual >= transacao.ValorTransacao)
                        {
                            cliente.LimitePIXAtual -= transacao.ValorTransacao;
                            await _repository.Atualizar(cliente);
                            return Ok("Transação aprovada. Limite Atual: "+cliente.LimitePIXAtual);
                        }
                        else
                        {
                            return Ok("Limite insuficiente para transacao, operacao abortada");
                        }
                    }
                    else
                    {
                        return BadRequest("O cliente especificado nao foi encontrado");
                    }
                }
                else
                {
                    return BadRequest("Os dados informados estao incorretos");
                }
            }
            else
            {
                return BadRequest("O parametro esta invalido");
            }
        }

        
    }
}
