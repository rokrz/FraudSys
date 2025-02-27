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
            var clienteExistente = await _repository.Buscar(cliente.NumeroAgencia,cliente.CPF);
            if (clienteExistente!=null)
            {
                return BadRequest("O usuário já esta cadastrado");
            }
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

    [HttpGet("BuscaCliente/{agencia}/{cpf}")]
    public async Task<IActionResult> BuscaCliente(string agencia, string cpf)
    {
        if (!string.IsNullOrEmpty(agencia) && !string.IsNullOrEmpty(cpf))
        {
            var cliente = await _repository.Buscar(agencia, cpf);
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
            Cliente cliente = await _repository.Buscar(cUpdate.NumeroAgencia, cUpdate.CPF);
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
    [HttpDelete("DeletaCliente/{agencia}/{cpf}")]
    public async Task<IActionResult> Delete(string agencia, string cpf)
    {
        if (!String.IsNullOrEmpty(agencia) && !String.IsNullOrEmpty(cpf))
        {
            Cliente c = await _repository.Buscar(agencia, cpf);
            if (c != null)
            {
                await _repository.Deletar(agencia, cpf);
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

    private bool ValidaCPF(string cpf)
    {
        // Remove caracteres não numéricos
        cpf = new string(cpf.Where(char.IsDigit).ToArray());
        // Verifica se o CPF tem 11 dígitos
        if (cpf.Length != 11)
        {
            return false;
        }
        // Verifica se todos os dígitos são iguais (CPF inválido)
        if (cpf.Distinct().Count() == 1)
        {
            return false;
        }
        // Calcula o primeiro dígito verificador
        int soma = 0;
        for (int i = 0; i < 9; i++)
        {
            soma += int.Parse(cpf[i].ToString()) * (10 - i);
        }
        int primeiroDigito = 11 - (soma % 11);
        if (primeiroDigito >= 10)
        {
            primeiroDigito = 0;
        }
        // Verifica o primeiro dígito verificador
        if (primeiroDigito != int.Parse(cpf[9].ToString()))
        {
            return false;
        }
        // Calcula o segundo dígito verificador
        soma = 0;
        for (int i = 0; i < 10; i++)
        {
            soma += int.Parse(cpf[i].ToString()) * (11 - i);
        }
        int segundoDigito = 11 - (soma % 11);
        if (segundoDigito >= 10)
        {
            segundoDigito = 0;
        }
        // Verifica o segundo dígito verificador
        if (segundoDigito != int.Parse(cpf[10].ToString()))
        {
            return false;
        }
        return true;
    }

    //Metodo de validacao das entradas
    private bool ValidaNovoUsuario(Cliente user)
    {
        //Valida o CPF por meio de um Regex
        if (string.IsNullOrEmpty(user.CPF) || !ValidaCPF(user.CPF))
        {
            return false;
        }
        //Verifica que o LimitePix é um numero positivo ou nulo
        if (user.LimitePIX < 0)
        {
            return false;
        }
        //Verirfica que os campos não são nulos
        if (string.IsNullOrEmpty(user.NumeroAgencia) || string.IsNullOrEmpty(user.NumeroConta))
        {
            return false;
        }

        return true;
    }


}
