FROM mcr.microsoft.com/mssql/server:latest

COPY mydb.mdf /var/opt/mssql/data/mydb.mdf
COPY mydb_log.ldf /var/opt/mssql/data/mydb_log.ldf

USER root
RUN chown -R mssql:root /var/opt/mssql
USER mssql