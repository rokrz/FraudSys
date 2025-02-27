using Amazon.DynamoDBv2.DataModel;
using FraudSys.Model;

namespace FraudSys.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly IDynamoDBContext context;

        public ClienteRepository(IDynamoDBContext context)
        {
            this.context = context;
        }

        public async Task Adicionar(Cliente cliente)
        {
            await context.SaveAsync(cliente);
        }

        public async Task<Cliente> Atualizar(Cliente cliente)
        {
            await context.SaveAsync(cliente);
            return cliente;
        }

        public async Task<Cliente> Buscar(string agencia, string cpf)
        {
            var lista = await context.QueryAsync<Cliente>(agencia, Amazon.DynamoDBv2.DocumentModel.QueryOperator.Equal, new object[] { cpf }).GetRemainingAsync();
            return lista.FirstOrDefault();
        }

        public async Task Deletar(string agencia, string cpf)
        {
            await context.DeleteAsync<Cliente>(agencia, cpf);
        }
    }
}
