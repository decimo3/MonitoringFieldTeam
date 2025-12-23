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
