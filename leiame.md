# Funcionalidades do OFS_BOT

## Monitoramento de ofensores do IDG

_Desenvolvido e entregue desde 20/05/2024 ás 09:42_

O programa, durante o dia, extrai informações das rotas das equipes e verifica possíveis violações do IDG.

Entre as verificações e seus ofensores:

- GPS desligado ou sem registro (geral);
- Equipe ociosa ou sem nota (ocupação);
- Atraso ou antecipação de login (ocupação);
- Calendário extendido ou encurtado (ocupação);
- Deslogou antecipadamente (ocupação);
- Parada indevida em transito (eficiência);
- Deslocamento atrasado (eficiência);
- Deslocamento indevido (eficiência);
- Atendimento atrasado (eficiência);

> A comunicação dessas violações ocorrem por meio de canais do Telegram

## Relatório de resumo de rota

_Desenvolvido e entregue desde 11/06/2024 ás 17:03_

O programa, no final do dia, extrai as informações de rotas de forma resumida e baixa o relatório oficial do OFS.

Entre as informações que ele coleta:

- Horário de login e logout da equipe;
- Horário do calendário da equipe;
- Tempo de duração do checklist (inicio de turno);
- Tempo de duração do intervalo (almoço);
- Tempo total de indisponibilidade;
- Tempo total de jornada de trabalho;
- Tempo total de execução e rejeição;
- Tempo total de ocupação e ociosidade;
- Tempo total de produtividade;
- Proporção de ocupação;
- Proporção de produtividade;
- Proporção do IDG desconsiderando a eficiência.

## Relatório de informação em massa

_Desenvolvido e entregue desde 02/05/2025 às 10:56_

O programa, por uma lista de notas, extrai informações da nota que não são exportados pelo relatório oficial do OFS.

Entre as informações que ele coleta:

- Informações gerais;
- Códigos de finalização;
- Materiais empregados e retirados;
- Formulário digital de inspeção (TOI);
- Evidências gerais do serviço;
- Evidências de inspeções (TOI);

Futuramente:

- Formulário digital da APR;
- Exportação das assinaturas da APR;

> Para que as funcionalidades marcadas como "futuras" sejam implementadas, é necessário demanda e solicitação das mesmas.

## Sistema de coleta de informações distribuído

_Desenvolvido e entregue desde 15/04/2025 às 18:41_

O programa agora conta com os modos DELEGADOR e SERVIDOR, para centralizar a coleta massiva de informações.

Nesse modo, o programa atuará de forma distribuída, atuando em vários computadores ao mesmo tempo. O programa OFS_BOT será configurado como SERVIDOR e um conjunto de computadores e como DELEGADOR no computador que distribuirá e centralizará as informações coletadas.

Nesse modo, o programa coletará as notas para extração de informação diretamente do relatório oficial do OFS, realizando um filtro com os critérios abaixo:

- A coluna "Ordem de Serviço" não deve estar em branco;
- A coluna "Status da Atividade" deve estar como "concluído";
- A coluna "Cod. de Finalização" deve ter algum dos códigos de finalização de ramal e/ou medidor.

O computador DELEGADOR salvará as informações em um banco de dados SQLite3, que com o driver instalado, poderá ser consultado no Excel.

Essa funcionalidade pretende acabar com o trabalho manual de separar notas para distribuir para os computadores, agrupar os relatórios e ainda o problema de perdas de informações coletadas, pois elas estarão sendo salvas imediatamente.

> Os códigos que eu estarei considerando: 18.0, 6.11, 6.15, 6.16, 6.43, 7.10, 7.11, 7.12, 7.15, 7.16, 7.17.
