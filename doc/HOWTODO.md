# Classes dos elementos no OFS:

### Atributos dos elementos
* **par_pid**: identificador para elemento do mesmo recurso;
* **aid**: identificador para elementos da mesma atividade;



### Visualização GANTT
1. **toaGantt-timeChart:** classe com as definições de pixels por horário;
4. **toaGantt-tl**: classe do container das notas do recurso;
    4.1. **toaGantt-tl-gpsmark**: classe do container da rota do recurso
        4.1.1. **gps-status-normal**: Classe do elemento indica o recurso está dentro da rota planejada (em azul);
        4.1.2. **gps-status-idle**: classe do elemento que indica o recurso parado na rota sem motivo (em laranja);
        4.1.3. **gps-status-alert**: classe do elemento que indica o recurso fora da rota planejada (em vermelho);
    1. **toaGantt-tb-travel**: classe do elemento do deslocamento;
    2. **toaGantt-tb**: classe do elemento da ordem de serviço;
    3. **toaGantt-tb-name**: classe do identificador do recurso;
    6. **toaGantt-tl-shift**: classe do elemento que indica a jornada de trabalho;
