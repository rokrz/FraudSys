using Microsoft.AspNetCore.Mvc.Testing;
using FraudSys;
using Moq;
using FraudSys.Controllers;
using FraudSys.Repositories;
using FraudSys.Model;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
namespace FraudSys_Tests
{
    public class FraudSysTests
    {
        private readonly Mock<IClienteRepository> _mockClienteRepo;
        private readonly ClienteController _controller;
        private readonly Cliente clienteBusca = new Cliente
        {
            CPF = "484.312.518-08",
            NumeroAgencia = "agencia2",
            NumeroConta = "conta2",
            LimitePIX = 200
        };

        public FraudSysTests()
        {
            _mockClienteRepo = new Mock<IClienteRepository>();
            _controller = new ClienteController(_mockClienteRepo.Object);
        }

        [Fact]
        public async Task TesteCriacaoClienteCorreto()
        {
            var clienteTeste = new Cliente
            {
                CPF = "123.123.123-01",
                NumeroAgencia = "123456",
                NumeroConta = "09201",
                LimitePIX = 100
            };

            var result = await _controller.CriaCliente(clienteTeste);//Setup(service => service.Adicionar(It.IsAny<Cliente>())).Returns(clienteTeste);
            var createdResult = Assert.IsType<OkObjectResult>(result);
            var returnCliente = Assert.IsType<Cliente>(createdResult.Value);
            Assert.Equal(clienteTeste.CPF, returnCliente.CPF);
            Assert.Equal(clienteTeste.NumeroAgencia, returnCliente.NumeroAgencia);
            Assert.Equal(clienteTeste.NumeroConta, returnCliente.NumeroConta);
            Assert.Equal(clienteTeste.LimitePIX, returnCliente.LimitePIX);
        }

        [Fact]
        public async Task TesteCriacaoClienteNulo()
        {
            var result = await _controller.CriaCliente(null);//Setup(service => service.Adicionar(It.IsAny<Cliente>())).Returns(clienteTeste);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cliente invalido", badRequestResult.Value);
        }

        [Fact]
        public async Task TesteCriacaoClienteIncorreto()
        {
            var clienteTeste = new Cliente
            {
                CPF = "12312312301",
                NumeroAgencia = "123456",
                NumeroConta = "09201",
                LimitePIX = 100
            };

            var result = await _controller.CriaCliente(clienteTeste);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Os campos não estão corretamente preenchidos", badRequestResult.Value);
        }

        [Fact]
        public async Task BuscaClienteCorreto()
        {
            _mockClienteRepo.Setup(repo => repo.Buscar(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta)).ReturnsAsync(clienteBusca);
            
            var controllerBusca = new ClienteController(_mockClienteRepo.Object);
            var result = await controllerBusca.BuscaCliente(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta);
            //Debug.WriteLine(((BadRequestObjectResult)result).Value);
            var searchResult = Assert.IsType<OkObjectResult>(result);
            var returnCliente = Assert.IsType<Cliente>(searchResult.Value);
            Assert.Equal(clienteBusca.CPF, returnCliente.CPF);
            Assert.Equal(clienteBusca.NumeroAgencia, returnCliente.NumeroAgencia);
            Assert.Equal(clienteBusca.NumeroConta, returnCliente.NumeroConta);
            Assert.Equal(clienteBusca.LimitePIX, returnCliente.LimitePIX);
        }

        [Fact]
        public async Task BuscaClienteIncorreto()
        {
            _mockClienteRepo.Setup(repo => repo.Buscar(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta)).ReturnsAsync(clienteBusca);

            var controllerBusca = new ClienteController(_mockClienteRepo.Object);
            var result = await controllerBusca.BuscaCliente("teste","teste");
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("O cliente especificado não foi encontrado", badRequestResult.Value);
        }

        [Fact]
        public async Task BuscaClienteNulo()
        {
            _mockClienteRepo.Setup(repo => repo.Buscar(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta)).ReturnsAsync(clienteBusca);

            var controllerBusca = new ClienteController(_mockClienteRepo.Object);
            var result = await controllerBusca.BuscaCliente(null, null);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Os parametros de busca nao sao validos", badRequestResult.Value);
        }

    }
}