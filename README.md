Visão Geral
O WebScrapper é um sistema de web crawling desenvolvido segundo os princípios do Domain-Driven Design (DDD) e SOLID. A solução foi construída para atender aos seguintes requisitos:

2.1.1. Acessar o site https://proxyservers.pro/proxy/list/order/updated/order_dir/desc.
2.1.2. Extrair os campos IP Address, Port, Country e Protocol de todas as linhas, em todas as páginas disponíveis.
2.1.3. Salvar o resultado da extração em um arquivo JSON, que deve ser salvo na máquina.
2.1.4. Persistir em banco de dados as métricas de execução: data de início, data de término, quantidade de páginas processadas, quantidade de linhas extraídas e o conteúdo do JSON gerado.
2.1.5. Salvar uma cópia (print) em formato HTML de cada página acessada.
2.1.6. Implementar o webcrawler de forma multithread, com no máximo 3 execuções simultâneas.
Arquitetura e Organização
A solução está dividida em quatro camadas principais:

Domain:
Contém os modelos de domínio que representam as entidades centrais do sistema:

ProxyDataModel: Representa os dados do proxy (IP, Port, Country, Protocol, etc.).
ScrapeResult: Agrega a lista de proxies extraídos e a URL da próxima página.
ScrapingMetrics: Armazena as métricas da execução do scraping (datas, número de páginas, total de linhas, JSON gerado).
Application:
Responsável pela orquestração do processo de scraping. Nesta camada estão:

IScraperService / WebScraperService: Responsável pela extração dos dados e pela gravação dos arquivos HTML.
ScraperManager: Gerencia a execução multithread, limitando a 3 tarefas simultâneas.
IScrapingOrchestrator / ScrapingOrchestrator: Orquestra o fluxo completo (chamada do scraping, serialização para JSON, gravação de arquivos e persistência das métricas no banco).
Infrastructure:
Contém a implementação do acesso a dados com Entity Framework Core:

ScrapingContext: O DbContext que gerencia a persistência dos dados (métricas).
InfrastructureServiceCollectionExtensions: Método de extensão para registrar o DbContext com a connection string.
ScrapingContextFactory: Fábrica para criação do DbContext em tempo de design (para migrações).
API:
Uma solução ASP.NET Core Web API que expõe endpoints para acionar o scraping.
O controller (por exemplo, ScrapingController) injeta o orquestrador e, ao ser acionado, dispara todo o fluxo de scraping.

Princípios SOLID e DDD
Single Responsibility: Cada classe possui uma responsabilidade única; por exemplo, o WebScraperService se encarrega exclusivamente da extração e do salvamento do HTML.
Dependency Inversion: São utilizadas interfaces para abstrair as implementações (por exemplo, IScraperService e IScrapingOrchestrator), permitindo a injeção de dependências e facilitando testes e manutenção.
Open/Closed: A arquitetura permite a extensão das funcionalidades sem alterar as classes existentes.
Separation of Concerns (Separação de Responsabilidades): A divisão da solução em Domain, Application, Infrastructure e API permite isolar regras de negócio, lógica de aplicação e detalhes técnicos.
Orquestração de Processos: A criação de um orquestrador (ScrapingOrchestrator) mantém o controller simples e delega toda a lógica de negócio, em linha com os princípios de Clean Architecture.
Requisitos Técnicos Atendidos
Acesso ao Site e Extração de Dados:
A URL de origem é acessada, e os campos "IP Address", "Port", "Country" e "Protocol" são extraídos de cada linha da tabela em todas as páginas disponíveis.

Persistência do Resultado:
Os dados extraídos são serializados em um arquivo JSON (proxies.json), armazenado no diretório atual. Além disso, os dados de execução (datas, páginas processadas, total de linhas e JSON) são persistidos no banco de dados SQL Server.

Captura do HTML:
Cada página é salva como um arquivo HTML na pasta html_pages, garantindo que o print (cópia do HTML) seja arquivado.

Execução Multithread:
A implementação utiliza SemaphoreSlim para limitar a execução a três tarefas simultâneas, garantindo o comportamento multithread.

Setup e Execução
Pré-requisitos
.NET 6/7/8 SDK
Docker e Docker Compose
Ferramenta de gerenciamento de SQL (opcional)
Passo a Passo
Clonar o Repositório

bash
Copiar
git clone <repository-url>
cd WebScrapper
Iniciar o SQL Server com Docker Compose
Certifique-se de que o Docker esteja instalado e execute:

bash
Copiar
docker-compose up -d
Isso iniciará um container SQL Server com a senha ELawScrapper e mapeará a porta 1433 para localhost.

Configurar a Connection String
Em appsettings.json (ou no design-time DbContext factory), a connection string deve ser:

arduino
Copiar
"Server=localhost,1433;Database=ScrapingDb;User Id=sa;Password=ELawScrapper;"
Executar as Migrações do EF Core Navegue até o projeto que contém o ScrapingContext e execute:

bash
Copiar
dotnet ef migrations add Initial
dotnet ef database update
Executar a Aplicação

Console Application:
bash
Copiar
dotnet run --project WebScrapper.Application
Web API:
bash
Copiar
dotnet run --project WebScraper.API
A API expõe o endpoint:
bash
Copiar
GET https://localhost:<porta>/api/scraping/run
Verificação dos Resultados

O arquivo proxies.json será gerado na pasta atual.
Os prints (HTML) das páginas serão salvos na pasta html_pages.
As métricas de execução serão persistidas na tabela ScrapingMetrics do banco de dados.
Uso e Endpoints
Console Application
O aplicativo console executa o processo de scraping automaticamente e imprime os resultados no console.

Web API
A API possui o endpoint:

GET /api/scraping/run
Ao chamar este endpoint, o sistema inicia o processo de scraping, salva os arquivos e as métricas, e retorna os proxies extraídos.
