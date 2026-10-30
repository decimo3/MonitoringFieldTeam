# Corrigido problema de navegação de páginas

O programa não era capaz de alternar entre os paineis "Console de Alocação" e a "Detalhes da Atividade"

Isso causava erro nos modos RETRODAY, MONITORA, e AUTOMATO, devido a coleta das informações de rota e relatórios

Os modos de operação RETRODAY e MONITORA, não eram tão impactados, pois os mesmos não navegam para outro painel

Porém, o modo AUTOMATO, alterna entre ambos painéis para completar o ciclo de coleta de dados, causando erro na coleta

Para solucionar esse problema, foi pesquisado e implementado a navegação para o "Console de Alocação" quando necessário
