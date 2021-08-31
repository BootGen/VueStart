FROM nginx:latest
COPY ../ClientApp/dist /var/www/startvue
COPY startvue.nginx /etc/nginx/conf.d/default.conf
RUN mkdir /var/www/sites
COPY default /var/www/sites/default