#!/bin/bash
nginx
cd /code/ClientApp
npm run serve --  --mode production &
cd /code/Server
/root/.dotnet/dotnet run