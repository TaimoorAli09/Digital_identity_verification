FROM nginx:alpine

# Remove default Nginx config
RUN rm /etc/nginx/conf.d/default.conf

# Copy your custom config
COPY default.conf /etc/nginx/conf.d/

# Copy all your frontend files (HTML, CSS, JS) to the target folder
COPY . /var/www/html/

EXPOSE 5500