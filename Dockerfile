FROM nginx:latest
RUN curl -fsSL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get update;  apt-get install -y nodejs
RUN curl -fsSL https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
RUN chmod +x dotnet-install.sh
RUN ./dotnet-install.sh -c 5.0
COPY nginx.conf /etc/nginx/conf.d/default.conf