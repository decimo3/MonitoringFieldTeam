# Resolvido problema com variação da estrutura da tabela de materiais

O programa estava dando erro de índice, ao tentar acessar tabelas com menos de 5 colunas, causando erro no coletor de materiais SERVIDOR.

O acúmulo de mais de 4 notas finalizadas como concluídas dessa forma, ou seja, sem medidor na instalação. Causou a impossibilidade de operação do modo AUTOMATO

Para esses casos, o OFS renderiza uma tabela incompleta, e para resolver o problema, foi implementada coleta para tabela com padrão de 3 e 4 colunas (além das coletas existentes para 5 e 6 colunas), preenchendo os demais valores com texto vazio para possibilitar a inserção no banco de dados.
