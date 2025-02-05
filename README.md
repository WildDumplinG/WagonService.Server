# WagonService.Server

## Описание

## Требования
.NET Core 9.0

## Установка
Для установки проекта выполните следующие шаги:
1. Клонируйте репозиторий:
```
   git clone [https://github.com/WildDumplinG/WagonService.Server.git](https://github.com/WildDumplinG/WagonService.Server)
```
2. Перейдите в каталог скачанного проекта
```bash
  cd WagonService.Server
```
3. Установите необходимые зависимости:
```bash
   dotnet restore
```
4. Для подключения к БД изменить файл проекта Server (WagonService.Server/Server) appsettings.json поле Connection класса WagonServiceImplSettings:
```bash
"WagonServiceImplSettings": {
  "Connection": "Host=localhost;Port=5432;Username=Admin;Password=Admin;Database=wagon"
},
```
# Запуск сервера
dotnet run --project Server

# Запуск через Docker-Compose
1. Настройка docker-compose.yml
```bash
ports:
  - "5000:5000"
  - "5001:5001"
Указываем порты согласно appsettings
 POSTGRES_USER: ${POSTGRES_USER:-postgres}
 POSTGRES_PASSWORD: ${POSTGRES_PASSWORD:-postgres}
Настройка дефолтного пользователя БД
- /d/Works/Interviews/Company/NIIAS/Tests/scheduler.backup:/docker-entrypoint-initdb.d/scheduler.backup
Путь до бэкапа
```
2. Создание бд
```bash
Запуск аркестратора из корня проекта
docker-compose up -d
Подключение к серверу postgreSQL
docker exec -it <ключ контейнера> psql -U <имя пользователя>
CREATE DATABASE <имя БД>;
\q
```
3. Подгрузка бэкапа
docker exec <ключ контейнера> pg_restore -U <имя пользователя> -d <имя бд> /docker-entrypoint-initdb.d/scheduler.backup

Релизы можно скачать по пути
[https://github.com/WildDumplinG/WagonService.Server/releases]
