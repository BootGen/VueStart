#!/bin/bash
nginx
cd /code/ClientApp
npm run serve &
cd /code/DefaultSite
npm run serve -- --port 8090