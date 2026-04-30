# Added persistence and filtering activities

Added persistence for occurrences and support for filtering multiple activities.

- Introduced a database-backed progress tracking feature. This allows the program to be interrupted—due to internet or power outages—without restarting the entire process.

- The program now processes OFS files and stores them in a database, using it to build the processing queue. If interrupted, execution resumes from the last processed point, eliminating previous data loss issues.

- Improved activity retrieval by using the activity ID, ensuring more accurate and reliable results.

- **Breaking change:** plain text reports are no longer supported for data extraction.

- Extended database storage to include additional information such as:
  - inclusion date and time
  - service date and time
  - status code
  - observation message
