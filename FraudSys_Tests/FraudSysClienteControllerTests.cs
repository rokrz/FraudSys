using Microsoft.AspNetCore.Mvc.Testing;
using FraudSys;
using Moq;
using FraudSys.Controllers;
using FraudSys.Repositories;
using FraudSys.Model;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.WebSockets;
namespace FraudSys_Tests
{
    public class FraudSysClienteControllerTests
    {
        private readonly Mock<IClienteRepository> _mockClienteRepo;
        private readonly ClienteController _controller;

        private readonly Cliente clienteBusca = new Cliente
        {
            CPF = "368.914.490-65",
            NumeroAgencia = "agencia2",
            NumeroConta = "conta2",
            LimitePIX = 200
        };

        public FraudSysClienteControllerTests()
        {
            _mockClienteRepo = new Mock<IClienteRepository>();
            _mockClienteRepo.Setup(repo => repo.Adicionar(It.IsAny<Cliente>()));
            _mockClienteRepo.Setup(repo => repo.Buscar(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta)).ReturnsAsync(clienteBusca);
            _mockClienteRepo.Setup(repo => repo.Atualizar(It.IsAny<Cliente>())).ReturnsAsync((Cliente clienteAtualizado) => clienteAtualizado);
            _mockClienteRepo.Setup(repo => repo.Deletar(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta));

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

            var result = await _controller.CriaCliente(clienteTeste);
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
           // _mockClienteRepo.Setup(repo => repo.Buscar(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta)).ReturnsAsync(clienteBusca);
            
            //var controllerBusca = new ClienteController(_mockClienteRepo.Object);
            var result = await _controller.BuscaCliente(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta);
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
            //_mockClienteRepo.Setup(repo => repo.Buscar(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta)).ReturnsAsync(clienteBusca);

            //var controllerBusca = new ClienteController(_mockClienteRepo.Object);
            var result = await _controller.BuscaCliente("teste","teste");
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("O cliente especificado não foi encontrado", badRequestResult.Value);
        }

        [Fact]
        public async Task BuscaClienteNulo()
        {
            //_mockClienteRepo.Setup(repo => repo.Buscar(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta)).ReturnsAsync(clienteBusca);

            //var controllerBusca = new ClienteController(_mockClienteRepo.Object);
            var result = await _controller.BuscaCliente(null, null);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Os parametros de busca nao sao validos", badRequestResult.Value);
        }

        [Fact]
        public async Task Update()
        {
            //_mockClienteRepo.Setup(repo => repo.Atualizar(clienteBusca)).ReturnsAsync((Cliente clienteAtualizado) => clienteAtualizado);
            //var controllerUpdate = new ClienteController(_mockClienteRepo.Object);
            var result = await _controller.Update(clienteBusca);
            var updateResult = Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task UpdateNulo()
        {
            //_mockClienteRepo.Setup(repo => repo.Atualizar(clienteBusca)).ReturnsAsync((Cliente clienteAtualizado) => clienteAtualizado);
            //var controllerUpdate = new ClienteController(_mockClienteRepo.Object);
            var result = await _controller.Update(null);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Atualizacao de cliente invalida", badRequestResult.Value);
        }


        [Fact]
        public async Task AtualizaLimitePIXValido()
        {
            //_mockClienteRepo.Setup(repo => repo.Buscar(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta)).ReturnsAsync(clienteBusca);
            //_mockClienteRepo.Setup(repo => repo.Atualizar(clienteBusca)).ReturnsAsync((Cliente clienteAtualizado) => clienteAtualizado);
            //var controllerAtualizacao = new ClienteController(_mockClienteRepo.Object);
            ClienteUpdate cUpdate = new ClienteUpdate
            {
                NumeroAgencia = clienteBusca.NumeroAgencia,
                CPF = clienteBusca.NumeroConta,
                NovoLimite = 500
            };

            var result = await _controller.AtualizaLimitePIX(cUpdate);
            var updateResult = Assert.IsType<OkObjectResult>(result);
            var returnCliente = Assert.IsType<Cliente>(updateResult.Value);
            Assert.Equal(clienteBusca.CPF, returnCliente.CPF);
            Assert.Equal(clienteBusca.NumeroAgencia, returnCliente.NumeroAgencia);
            Assert.Equal(clienteBusca.NumeroConta, returnCliente.NumeroConta);
            Assert.Equal(cUpdate.NovoLimite, returnCliente.LimitePIX);
        }

        [Fact]
        public async Task AtualizaLimitePIXClienteInvalido()
        {
            //_mockClienteRepo.Setup(repo => repo.Buscar(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta)).ReturnsAsync(clienteBusca);
            //_mockClienteRepo.Setup(repo => repo.Atualizar(clienteBusca)).ReturnsAsync((Cliente clienteAtualizado) => clienteAtualizado);
            //var controllerAtualizacao = new ClienteController(_mockClienteRepo.Object);
            ClienteUpdate cUpdate = new ClienteUpdate
            {
                NumeroAgencia = "agencia1",
                CPF = clienteBusca.NumeroConta,
                NovoLimite = 500
            };

            var result = await _controller.AtualizaLimitePIX(cUpdate);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("O cliente especificado não foi encontrado.", badRequestResult.Value);
        }

        [Fact]
        public async Task AtualizaLimitePIXValorInvalido()
        {
            //_mockClienteRepo.Setup(repo => repo.Buscar(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta)).ReturnsAsync(clienteBusca);
            //_mockClienteRepo.Setup(repo => repo.Atualizar(clienteBusca)).ReturnsAsync((Cliente clienteAtualizado) => clienteAtualizado);
            //var controllerAtualizacao = new ClienteController(_mockClienteRepo.Object);
            ClienteUpdate cUpdate = new ClienteUpdate
            {
                NumeroAgencia = clienteBusca.NumeroAgencia,
                CPF = clienteBusca.NumeroConta,
                NovoLimite = -120
            };

            var result = await _controller.AtualizaLimitePIX(cUpdate);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("O limite informado não é permitido.", badRequestResult.Value);
        }

        [Fact]
        public async Task AtualizaLimitePIXNulo()
        {
            //_mockClienteRepo.Setup(repo => repo.Buscar(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta)).ReturnsAsync(clienteBusca);
            //_mockClienteRepo.Setup(repo => repo.Atualizar(clienteBusca)).ReturnsAsync((Cliente clienteAtualizado) => clienteAtualizado);
            //var controllerAtualizacao = new ClienteController(_mockClienteRepo.Object);
            var result = await _controller.AtualizaLimitePIX(null);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Os parametros de atualizacao nao sao validos", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteCorreto()
        {
            //_mockClienteRepo.Setup(repo => repo.Buscar(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta)).ReturnsAsync(clienteBusca);

            //var controllerDelecao = new ClienteController(_mockClienteRepo.Object);
            var result = await _controller.Delete(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta);
            var OkResult = Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteIncorreto()
        {
            //_mockClienteRepo.Setup(repo => repo.Buscar(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta)).ReturnsAsync(clienteBusca);
            //_mockClienteRepo.Setup(repo => repo.Deletar(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta));

            //var controllerDelecao = new ClienteController(_mockClienteRepo.Object);
            var result = await _controller.Delete("aleatorio", clienteBusca.NumeroConta);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cliente nao encontrado", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteNulo()
        {
            //_mockClienteRepo.Setup(repo => repo.Buscar(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta)).ReturnsAsync(clienteBusca);
            //_mockClienteRepo.Setup(repo => repo.Deletar(clienteBusca.NumeroAgencia, clienteBusca.NumeroConta));

            //var controllerDelecao = new ClienteController(_mockClienteRepo.Object);
            var result = await _controller.Delete(null, null);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Parametros invalidos", badRequestResult.Value);
        }

    }
}