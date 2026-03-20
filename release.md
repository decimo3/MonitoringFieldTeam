# Improved report details and reimplemented navigation tree

Now, the retroachive report have three more columns:

* saida_canteiro: the time the team leaves the site, calculated from the end time of the `Início de Turno` or the end of the subsequent `Indisponibilidade` period after the `Início de Turno`.
* atraso_startup: total time elapsed between the team's login window or login time in OFS and the time of departure from the site.
* final_deslocando: total time elapsed between the end time of the team's last note and the end time of the shift in OFS.

> These columns are to help analyze wasted time and assess the efficiency of supervision during team departures.

In addition to this change, the resource abbreviation instruction was removed to facilitate comparison with the resource name in team composition.

The bucket hierarchy navigation system was also reimplemented.
