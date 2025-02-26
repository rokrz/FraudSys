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

        public async Task Atualizar(Cliente cliente)
        {
            await context.SaveAsync(cliente);
        }

        public async Task<Cliente> Buscar(string agencia, string conta)
        {
            var lista = await context.QueryAsync<Cliente>(agencia, Amazon.DynamoDBv2.DocumentModel.QueryOperator.Equal, new object[] { conta }).GetRemainingAsync();
            return lista.FirstOrDefault();
        }

        public async Task Deletar(string agencia, string conta)
        {
            await context.DeleteAsync<Cliente>(agencia, conta);
        }
    }
}
