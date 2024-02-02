
```mermaid
graph TD;
toaGantt --> toaGantt-head
toaGantt --> toaGantt-body
toaGantt --> toaGantt-bottom
toaGantt-body --> toaGantt-provTree
toaGantt-body --> toaGantt-timeChart
toaGantt-timeChart --> toaGantt-tl
toaGantt-tl --> toaGantt-tl-shift
toaGantt-tl --> toaGantt-tl-gpsmark
toaGantt-tl-gpsmark --> gps-status-normal
toaGantt-tl-gpsmark --> gps-status-idle
toaGantt-tl-gpsmark --> gps-status-alert
toaGantt-body --> toaGantt-notOrdered
toaGantt-body --> toaGantt-withoutDate
toaGantt-body --> toaGantt-vScroll
```