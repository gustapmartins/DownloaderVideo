# DownloaderVideo - .NET Project Template

Este repositório contém um template personalizado para projetos .NET, permitindo a geração rápida de uma estrutura padronizada com configurações pré-definidas.

## Requisitos

Antes de instalar e usar o template, certifique-se de que você tem os seguintes requisitos atendidos:

- .NET SDK 6.0 ou superior instalado
- Git (opcional, para clonar o repositório)

## Instalação do Template

Para instalar o template localmente, execute o seguinte comando no terminal:

```sh
dotnet new install C:\Github\DownloaderVideo\.template.config
```

Se você estiver usando um repositório Git, clone-o primeiro:

```sh
git clone https://github.com/seu-usuario/DownloaderVideo.git
cd DownloaderVideo
```

## Como Usar o Template

Depois de instalado, você pode listar os templates disponíveis com:

```sh
dotnet new list
```

Para exibir as opções do seu template, utilize:

```sh
dotnet new generateTemplate --help
```

### Criando um Novo Projeto

Para criar um novo projeto usando esse template, execute o seguinte comando:

```sh
dotnet new generateTemplate -n NomeDoProjeto -o Caminho/Destino
```

Se quiser testar sem criar arquivos, use:

```sh
dotnet new generateTemplate --dry-run
```

## Opções Disponíveis no Template

### Parâmetros Gerais

| Opção | Descrição |
|---------|-------------|
| `-n, --name` | Define o nome do projeto gerado. |
| `-o, --output` | Especifica o diretório de saída. |
| `--dry-run` | Simula a criação sem modificar arquivos. |
| `--force` | Sobrescreve arquivos existentes. |

### Configurações do Projeto

| Opção | Descrição |
|---------|-------------|
| `-F, --Framework <net6.0|net7.0|net8.0>` | Define a versão do .NET Framework. Padrão: `net8.0` |
| `-E, --EnableSwaggerSupport` | Ativa o suporte ao Swagger para documentação da API. Padrão: `true` |
| `-A, --AppSettingsDev` | Inclui um arquivo `appsettings.Development.json`. Padrão: `true` |
| `-D, --DockerCompose` | Adiciona um `docker-compose.yml`. Padrão: `true` |
| `-Au, --Authentication` | Adiciona arquivos relacionados à autenticação. Padrão: `false` |
| `-G, --GithubActions` | Inclui arquivos de configuração do GitHub Actions. Padrão: `false` |

### Exemplo de Uso Completo

Criando um projeto chamado `MinhaApi` com .NET 7.0, sem Swagger e com suporte a Docker:

```sh
dotnet new generateTemplate -n MinhaApi -F net7.0 -E false -D true
```

Isso criará um novo projeto na pasta atual com a estrutura padrão e os recursos especificados.

## Removendo o Template

Se precisar remover o template instalado, use:

```sh
dotnet new uninstall C:\Github\DownloaderVideo\.template.config
```

## Contribuição

Sinta-se à vontade para contribuir com melhorias! Basta abrir um Pull Request com suas sugestões.

## Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

