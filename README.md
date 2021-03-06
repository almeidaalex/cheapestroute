# Rota mais barata para o viajante

Para executar essa aplicação, você precisará do [SDK do dotnet versão 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) de acordo com o SO que irá rodar a aplicação.

## Compilando os projetos...

### Compilando CLI
Para compilar o CLI seguir as instruções, na pasta raiz digitar:
```
dotnet publish .\CLI\CLI.csproj -o ./dist/cli
```

E para executar aplicação, executar as seguintes instruções
```
cd dist/cli
cheap_route.exe <arquivo>
```

### Compilando Api
Para compilar o Api seguir as instruções, na pasta raiz digitar:
```
dotnet publish .\API\API.csproj -o ./dist/api
```

E para executar aplicação, executar as seguintes instruções
```
cd dist/api
dotnet API.dll
```


## Estrutura da aplicacão

```
 CheapestTravel
    CLI
    Core
    UnitTest
    API
```

## Design da solução
A solução possui quatro projetos, embora sejam pequenos eles tem suas responsabilidades definidas. O principal projeto é o Core, é nele que a funcionalidade principal está escrita, não tem dependencias com as camadas de apresentação. Os testes unitários foram basicamente construídos para validar as funcionalidades do core.

Os projetos CLI e API são respectivamente a aplicação de linha de comando e o serviço Rest. É a forma com que aplicação chega ao seu usuário final, não possui nenhuma regra sobre as rotas, apenas possui validações de entrada e mostra as saídas de sucesso ou falha.

O único pacote que não faz parte dos framework do dotnet foi o pacotes NUnit para fazer os testes unitários, o restante foi feito com o que a plataforma oferece.

### API
A api possui apenas dois métodos, para incluir novas rotas e para consultar o menor caminho.
Para incluir, deve-se usar o método POST da api:
```
curl -X POST http://localhost:5000/api/route -d "GRU,YVR,40"
```
Temos 3 códigos possíveis:

- 201: recurso criado com sucesso.
- 400: os dados informados estão com alguma inconsistência, observar a mensagem de retorno
- 500: algum erro não esperado.

Para obter a rota mais econômica, deve-se usar o método GET, informando origem e destino.
```
curl -X GET http://localhost:5000/api/route/<origem>/<destino>
```

Temos 4 códigos possíveis:

- 200: foi encontrada a rota mais barata.
- 404: origem ou destino não foram encontrados.
- 400: alguma inconsistência nos dados informados.
- 500: algum erro não esperado.


