using Amazon.DynamoDBv2.DataModel;

namespace FraudSys.Model
{
    [DynamoDBTable("Banco_KRT")]
    public class Cliente
    {
        [DynamoDBProperty]
        public string CPF { get; set; }

        [DynamoDBHashKey("NumeroAgencia")]
        public string NumeroAgencia { get; set; }

        [DynamoDBRangeKey("NumeroConta")]
        public string NumeroConta { get; set; }

        [DynamoDBProperty]
        public float LimitePIX { get; set; }

        [DynamoDBProperty]
        public float LimitePIXAtual { get; set; }

        public void ResetLimitePIX()
        {
            this.LimitePIXAtual = LimitePIX;
        }

    }
}
