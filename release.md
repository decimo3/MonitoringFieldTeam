# Corrigida a lógica de verificação de relatórios importados

O programa anteriormente verificava se o relatório havia sido importado a pelo menos seis horas, o que abre margem para duplicação de importação de relatório.

Agora o programa verifica se já foi importado relatório no mesmo dia, então mesmo que ele seja executado com mais de 6 horas de diferença, não duplicará a importação.
