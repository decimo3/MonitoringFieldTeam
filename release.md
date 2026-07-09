# Implementado novo módulo "AUTONOMO"

O programa agora trabalhará de forma autônoma para coleta de informações do OFS. Ele fará automaticamente:

1. O download do relatório oficial do OFS em D+1;
2. Instanciação de um servidor de coleta local;
3. Importação da lista de notas "concluídas";
4. Coleta e persistência no banco de dados.

Essas operações serão realizadas assim que o programa iniciar, mantendo a coleta de dados independente de qualquer interação humana.

Além dessa nova funcionalidade, foram realizadas as melhorias e correções:

- Implementada dupla checagem do número da atividade para zerar as coletas erradas;
- Adicionada instrução para reiniciar a coleta se ainda houver erros após o mesmo;
- Implementado o sistema de verificação de falhas consecutivas no servidor local;
    > Isso é, se der um problema, depois de 3 falhas, o programa reiniciará.
- Alterado o banco de dados de LOCAL (SQLite3) para CENTRAL (PostgreSQL 16);
- Corrigido problema de download do ChromeDriver em versão superior ao GoogleChrome.
