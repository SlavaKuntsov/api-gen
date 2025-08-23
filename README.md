# ApiGen

Простая v1 реализация генератора API на ASP.NET Core 9.

## Возможности
- Сканирование контроллеров и действий в рабочем каталоге
- Предпросмотр diff перед записью файлов
- REST-эндпоинты `/gen/routes`, `/gen/preview`, `/gen/apply`

## Сборка

```bash
dotnet build -v minimal
```

## Запуск

```bash
dotnet run --project src/ApiGen.Web
```

## Тесты

```bash
dotnet test -v minimal
```
