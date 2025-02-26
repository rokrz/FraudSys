namespace FraudSys.Model
{
    public class TransacaoModel
    {
        public string NumeroAgenciaOrigem { get; set; }
        public string NumeroContaOrigem { get; set; }
        public float ValorTransacao { get; set; }
        public string NumeroAgenciaDestino { get; set; }
        public string NumeroContaDestino { get; set; }

        public bool ValidaDadosTransacao()
        {
            if (string.IsNullOrEmpty(NumeroAgenciaOrigem))
            {
                return false;
            }
            if (string.IsNullOrEmpty(NumeroContaOrigem))
            {
                return false;
            }
            if (ValorTransacao < 0)
            {
                return false;
            }
            return true;
        }
    }
}
