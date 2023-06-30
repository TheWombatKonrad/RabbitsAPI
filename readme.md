Calls to the database are async, which doesn't do much right not because the database is
the bottleneck - it can only handle one request at a time. But if I switch to a different
database, e.g. Microsoft Azure SQL Database/NoSQL/etc, having the calls be async could speed
up the process.
