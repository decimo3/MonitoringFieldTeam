# Manual de uso do programa MonitoringFieldTeam (OFS_BOT)

O programa MonitoringFieldTeam (OFS_BOT) é um robô de extração de informações do sistema Oracle Field Service.

O sistema pode não expor informações necessárias para a operação de forma direta, fazendo que o operador tenha que realizar manualmente. Esse programa é exatamente para resolver esse tipo de problema.

## Instalação

O programa é auto-contido, isso quer dizer que não é necessária nenhuma instalação para o programa funcionar, é só extrair em qualquer lugar, configurar e usar. Recomenda-se que extraia na área de trabalho para facilitar o acesso caso necessário.

## Configurações

As configurações do programa são definidas no arquivo [ofs.conf](src/ofs.conf), e a configurações delas dependem do [modo de operação do programa](#modos-de-operação). Cada modo tem suas necessidades específicas, abaixo listarei as necessidades em comum, com exceção ao modo DELEGADOR.

- GCHROME: caminho absoluto para o executável do navegador Google Chrome;
- DATAPATH: caminho da pasta utilizada para armazenar os downloads e relatórios;
- WEBSITE: URL do sistema OFS a ser automatizado;
- USUARIO: e-mail do usuário a ser usado na autenticação;
- PALAVRA: senha do usuário a ser usado na autenticação;
- RECURSO: lista de nomes de piscina/grupo/balde/recurso a ser monitorado;
- HORARIO: lista de abreviações de recursos e seus horários desejado de janela;
- BOT_TOKEN: token do chatbot no Telegram para envio dos comunicados e documentos;
- BOT_CHANNEL: lista de recursos e seus respectivos canais onde serão enviados os comunicados;
- RETRODAY: data limite para considerar relatórios retroativos;
- EXTRACAO: Lista de informações a serem extraídas da finalização do OFS;
- OPERACAO: definição do modo de operação do programa;

## Modos de operação

### RETRODAY

Modo de extração automática de relatório de final de rota e relatório oficial do OFS.

O programa verificará a partir da data especificada na configuração RETRODAY, se há relatórios de final de rota e relatório oficial do OFS na pasta definida na configuração DATAPATH para os baldes definidos na configuração RECURSO. Se não for encontrado o relatório, ele realizará a tentativa de coleta dos relatórios faltantes, salvando os mesmos na pasta definida na configuração DATAPATH.

> As informações do relatório retroativo estão definidos no arquivo [leiame.md](leiame.md#relatório-de-resumo-de-rota).

### MONITORA

Modo de monitoramento contínuo das informações de ofensa ao IDG do sistema OFS.

O programa verificará por violações no IDG de acordo com as informações coletadas no sistema OFS, verificando os baldes definidos na configuração RECURSO e reportando nos canais definidos na configuração BOT_CHANNEL. O programa também verifica o tamanho da janela de horário pela abreviação definida na configuração HORARIO.

> As informações do comunicado de infrações estão definidos no arquivo [leiame.md](leiame.md#monitoramento-de-ofensores-do-idg).

### EXTRACAO

Modo de coleta de informação em massa de forma isolada e a partir de uma lista de notas.

O programa procurará pelo arquivo `ofs.txt` na pasta definida na configuração DATAPATH. Encontrando o arquivo ele consultará nota por nota, coletando as informações definidas na configuração EXTRACAO. Após completar a coleta, o mesmo salvará em relatórios com as informações extraídas com o nome da informação no nome do arquivo.

### SERVIDOR

Modo de exposição de API para consulta de informações de notas de serviço no OFS.

O programa irá expor uma API mínima, com somente o endpoint GET raiz "/", que receberá um payload com informações de tipo de informação extrair e a nota a ser consultada. Exemplo:

```sh
curl --request 'GET' --data '{"info":["INF","COD"],"nota":1234567890}' http://localhost:7826/ --verbose
```

Isso permite o acesso a consulta de informações remotamente (de outros computadores), como com o [DELEGADOR](#delegador).

### DELEGADOR

Modo de operação do programa que centraliza a coleta de informações em massa e distribui para os computadores em modo [SERVIDOR](#servidor).

O programa procurará por arquivos de relatório oficial do OFS na pasta definida na configuração DATAPATH. Encontrando os arquivos, ele consultará nota por nota de cada arquivo, coletando as informações definidas na configuração EXTRACAO. A cada informação recebida, o mesmo salvará em um banco de dados SQLite3, que com o driver instalado, poderá ser consultado no Excel.

> Esse é o único modo de operação que não automatiza o Google Chrome, e consequentemente o sistema OFS.

## Solução de problemas
