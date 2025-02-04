FROM node:18-alpine

WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .

# Установите serve для обслуживания статики
RUN npm install -g serve

# Соберите проект
RUN npm run build

EXPOSE 3000
CMD ["serve", "-s", "dist", "-l", "3000"]