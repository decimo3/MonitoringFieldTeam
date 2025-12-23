# Reimplemented system for downloading evidence attached to OFS

The program has been updated to download, rename, and move each piece of evidence to its corresponding folder, keeping downloads organized for later consultation.

Evidence extraction is performed using the same method as material extraction: a list of notes must be provided for consultation, one by one. The `EXTRACAO` setting in the configuration file must contain the terms `JPG` and/or `EVD` depending on the extraction source.

Notes:

The program will create folders and rename the files with the service number; however, the files will have the download order number added, with the addition of ten per type: ten for common evidence (JPG); and two for inspection evidence (EVD).

One change made is that the file with the list of notes must be created in the same folder defined in the `DATAPATH` setting. This is the same location where files and reports will be saved.
