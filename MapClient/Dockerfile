FROM nginx:latest

COPY map.html /usr/share/nginx/html/
#COPY redirect.conf /etc/nginx/conf.d/

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]