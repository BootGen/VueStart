FROM nginx:latest
RUN curl -fsSL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get update;  apt-get install -y nodejs
COPY nginx.conf /etc/nginx/conf.d/default.conf