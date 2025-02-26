using FraudSys.Model;
using FraudSys.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace FraudSys.Controllers;


[ApiController]
[Route("[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IClienteRepository _repository;

    public ClienteController(IClienteRepository repository)
    {
        _repository = repository;
    }

    //Metodo de criacao do usuario
    //Valida as informacoes do novo usuario(Valida CPF, numeros de conta e agencia e um limite de PIX maior que 0)
    [HttpPost("CriaCliente")]
    public async Task<IActionResult> CriaCliente(Cliente cliente)
    {
        if (cliente != null)
        {
            if (ValidaNovoUsuario(cliente))
            {
                await _repository.Adicionar(cliente);
                return Ok(cliente);
            }
            else
            {
                return BadRequest("Os campos não estão corretamente preenchidos");
            }
        }
        else
        {
            return BadRequest("Cliente invalido");
        }
    }

    [HttpGet("BuscaCliente/{agencia}/{conta}")]
    public async Task<IActionResult> BuscaCliente(string agencia, string conta)
    {
        if (!string.IsNullOrEmpty(agencia) && !string.IsNullOrEmpty(conta))
        {
            var cliente = await _repository.Buscar(agencia, conta);
            if (cliente != null)
            {
                return Ok(cliente);
            }
            else
            {
                return BadRequest("O cliente especificado não foi encontrado");
            }
        }
        else
        {
            return BadRequest("Os parametros de busca nao sao validos");
        }

    }

    //Verifica se existe um cliente com as informacoes de conta especificadas e atualiza o novo limite de pix
    [HttpPost("AtualizaLimite")]
    public async Task<IActionResult> AtualizaLimitePIX(ClienteUpdate cUpdate)
    {
        if (cUpdate != null)
        {
            Cliente cliente = await _repository.Buscar(cUpdate.NumeroAgencia, cUpdate.NumeroConta);
            if (cUpdate.NovoLimite >= 0)
            {
                if (cliente != null)
                {
                    cliente.LimitePIX = cUpdate.NovoLimite;
                    cliente.ResetLimitePIX();
                    await _repository.Atualizar(cliente);
                    return Ok(cliente);
                }
                else
                {
                    return BadRequest("O cliente especificado não foi encontrado.");
                }
            }
            else
            {
                return BadRequest("O limite informado não é permitido.");
            }
        }
        else
        {
            return BadRequest("Os parametros de atualizacao nao sao validos");
        }

    }

    //Metodo de atualziacao do cliente
    [HttpPut("AtualizaCliente")]
    public async Task<IActionResult> Update(Cliente cliente)
    {
        if (cliente != null)
        {
            await _repository.Atualizar(cliente);
            return Ok();
        }
        else
        {
            return BadRequest("Atualizacao de cliente invalida");
        }
    }

    //Metodo de deleçao do cliente
    [HttpDelete("DeletaCliente/{agencia}/{conta}")]
    public async Task<IActionResult> Delete(string agencia, string conta)
    {
        if (!String.IsNullOrEmpty(agencia) && !String.IsNullOrEmpty(conta))
        {
            Cliente c = await _repository.Buscar(agencia, conta);
            if (c != null)
            {
                await _repository.Deletar(agencia, conta);
                return Ok();
            }
            else
            {
                return BadRequest("Cliente nao encontrado");
            }
        }
        else
        {
            return BadRequest("Parametros invalidos");
        }
        
    }

    //Metodo de validacao das entradas
    private bool ValidaNovoUsuario(Cliente user)
    {
        bool isNewUserValid = true;

        //Valida o CPF por meio de um Regex
        if (string.IsNullOrEmpty(user.CPF) || !Regex.Match(user.CPF, @"^\d{3}\.\d{3}\.\d{3}-\d{2}$").Success)
        {
            isNewUserValid = false;
            return isNewUserValid;
        }
        //Verifica que o LimitePix é um numero positivo ou nulo
        if (user.LimitePIX < 0)
        {
            isNewUserValid = false;
            return isNewUserValid;
        }
        //Verirfica que os campos não são nulos
        if (string.IsNullOrEmpty(user.NumeroAgencia) || string.IsNullOrEmpty(user.NumeroConta))
        {
            isNewUserValid = false;
            return isNewUserValid;
        }

        return isNewUserValid;
    }


}
