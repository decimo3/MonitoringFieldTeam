# Corrigido erro de verificação de página

O método de verificação de página checava se o elemento SEARCHBAR_LOGO estava presente para verificar se o gráfico de GANNT estava carregado.

Porém esse elemento também está presente na página "Detalhes da Atividade", fazendo as operações de coleta falharem.

Foi corrigida a lógica de checagem, realizando através do título da página, como sugerido pelo programador [ventacraft](https://github.com/ventacraft).
