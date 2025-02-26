using FraudSys.Controllers;
using FraudSys.Model;
using FraudSys.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FraudSys_Tests
{
    public class FraudSysTransacaoControllerTests
    {
        private readonly Mock<IClienteRepository> _mockClienteRepo;
        private readonly TransacaoController _controller;

        private readonly Cliente clienteBusca = new Cliente
        {
            CPF = "484.312.518-08",
            NumeroAgencia = "agencia2",
            NumeroConta = "conta2",
            LimitePIX = 200,
            LimitePIXAtual = 200
        };

        private readonly TransacaoModel transacaoExemplo = new TransacaoModel
        {
            NumeroAgenciaOrigem = "agencia2",
            NumeroContaOrigem = "conta2",
            NumeroAgenciaDestino = "agencia2",
            NumeroContaDestino = "conta2",
            ValorTransacao = 100
        };

        public FraudSysTransacaoControllerTests()
        {
            _mockClienteRepo = new Mock<IClienteRepository>();
            _mockClienteRepo.Setup(repo => repo.Adicionar(It.IsAny<Cliente>()));
            _mockClienteRepo.Setup(repo => repo.Buscar(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta)).ReturnsAsync(clienteBusca);
            _mockClienteRepo.Setup(repo => repo.Atualizar(It.IsAny<Cliente>())).ReturnsAsync((Cliente clienteAtualizado) => clienteAtualizado);
            _mockClienteRepo.Setup(repo => repo.Deletar(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta));

            _controller = new TransacaoController(_mockClienteRepo.Object);
        }

        [Fact]
        public async Task TestaTransacaoValida()
        {
            var result = await _controller.ValidaTransacao(transacaoExemplo);
            var resultSucesso = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Transação aprovada. Limite Atual: 100", resultSucesso.Value);
        }

        [Fact]
        public async Task TestaTransacaoValidaLimiteInsuficiente()
        {
            TransacaoModel tm = new TransacaoModel
            {
                NumeroAgenciaOrigem = "agencia2",
                NumeroContaOrigem = "conta2",
                NumeroAgenciaDestino = "agencia2",
                NumeroContaDestino = "conta2",
                ValorTransacao = 400
            };
            var result = await _controller.ValidaTransacao(tm);
            var resultSucesso = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Limite insuficiente para transacao, operacao abortada", resultSucesso.Value);
        }


        [Fact]
        public async Task TestaTransacaoInvalidaClienteInexistente()
        {
            TransacaoModel tm = new TransacaoModel
            {
                NumeroAgenciaOrigem = "agencia 4",
                NumeroContaOrigem = "conta 5",
                NumeroAgenciaDestino = "agencia2",
                NumeroContaDestino = "conta2",
                ValorTransacao = 400
            };
            var result = await _controller.ValidaTransacao(tm);
            var resultFalho = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("O cliente especificado nao foi encontrado", resultFalho.Value);
        }

        [Fact]
        public async Task TestaTransacaoInvalidaDadosInvalidosValorTransacao()
        {
            TransacaoModel tm = new TransacaoModel
            {
                NumeroAgenciaOrigem = "agencia2",
                NumeroContaOrigem = "conta2",
                NumeroAgenciaDestino = "agencia2",
                NumeroContaDestino = "conta2",
                ValorTransacao = -1400
            };
            var result = await _controller.ValidaTransacao(tm);
            var resultFalho = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Os dados informados estao incorretos", resultFalho.Value);
        }

        [Fact]
        public async Task TestaTransacaoInvalidaDadosInvalidos()
        {
            TransacaoModel tm = new TransacaoModel
            {
                NumeroAgenciaOrigem = null,
                NumeroContaOrigem = null,
                NumeroAgenciaDestino = "agencia2",
                NumeroContaDestino = "conta2",
                ValorTransacao = 100
            };
            var result = await _controller.ValidaTransacao(tm);
            var resultFalho = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Os dados informados estao incorretos", resultFalho.Value);
        }

        [Fact]
        public async Task TestaTransacaoInvalidaNulo()
        {
            var result = await _controller.ValidaTransacao(null);
            var resultFalho = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("O parametro esta invalido", resultFalho.Value);
        }

    }
}
