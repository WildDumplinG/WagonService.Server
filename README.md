# WagonService.Server

## Описание

## Требования
.NET Core 9.0

## Установка
Для установки проекта выполните следующие шаги:
1. Клонируйте репозиторий:
```bash
   git clone [https://github.com/ваш_пользователь/WagonService.Server.git](https://github.com/WildDumplinG/WagonService.Server)
2. Перейдите в каталог скачанного проекта
```bash
  cd WagonService.Server
3. Установите необходимые зависимости:
```bash
   dotnet restore
4. Для подключения к БД изменить файл проекта appsettings.json поле Connection класса WagonServiceImplSettings:
```bash
"WagonServiceImplSettings": {
  "Connection": "Host=localhost;Port=5432;Username=Admin;Password=Admin;Database=wagon"
},

# Запуск сервера
dotnet run
