# New Delegator Operation Mode for Distributed Work

The program in DELEGATOR mode will centralize and distribute information requests to computers running in SERVER mode.

The program will search for official OFS report files in the folder defined in the DATAPATH configuration. When it finds the files, it will go through each record in every file, collecting the information defined in the EXTRACAO configuration. Each piece of information retrieved will be saved in an SQLite3 database, which, with the appropriate driver installed, can be queried in Excel.

