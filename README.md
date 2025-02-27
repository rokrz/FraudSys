# **README**

## **Configuração do sistema:**
1- Baixar e instalar o AWS Cli (link: https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html)

2- Configurar o usuário AWS através do comando aws configure

3-  Ao executar o programa pela primeira vez, ele validará se existe uma tabela “Banco_KRT” no DynamoDB da conta configurada. Caso não haja, ele criará uma tabela com as seguintes chaves: 
	-pk: NumeroAgencia (string)
	-sk:CPF (string)

4- O sistema já pode ser testado através dos comandos no documento requests.http ou outros métodos (Postman ou API do Swagger)

## **Justificativas**
Na criação da estrutura do banco, optei por usar o conjunto de agência e cpf como chaves de usuários. Ao considerar usar somente o cpf de chave, acreditei que seria um limitante do ponto de vista do usuário, só poder ter uma conta com seu cpf, e possivelmente oneroso do ponto de vista do banco, dado que o cpf seria usado como partition key, o que abriria mais opções de particionamento. Decidi no primeiro momento agrupar por agência e fazer a chave secundária o número da conta, mas por se tratar de uma aplicação PIX, optei por substituí-la pelo CPF.

Usando agora o CPF como uma das chaves do banco, aumentei as validações em cima do CPF. Logo, para testes de operação, é necessário usar um gerador de CPF.

Para a operação de validar transações, eu optei por criar uma nova controller, separando os diversos domínios da aplicação. Os modelos de TransacaoModel e ClienteUpdate são estruturas para fazer a tradução dos dados do body para a aplicação.
