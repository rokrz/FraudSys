using FraudSys.Model;

namespace FraudSys.Repositories
{
    public interface IClienteRepository
    {
        Task Adicionar(Cliente cliente);
        Task<Cliente> Atualizar(Cliente cliente);
        Task<Cliente> Buscar(string agencia, string conta);
        Task Deletar(string agencia, string conta);
    }
}
