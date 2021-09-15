#!/bin/bash
nginx
cd /code/ClientApp
npm run serve --  --mode production &
cd /code/DefaultSite
npm run serve -- --port 8090  --mode production &
cd /code/Server
/root/.dotnet/dotnet run