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

Перед запуском убедитесь, что каталог, указанный в `Workspace:Root` (по умолчанию `/workspace/target`), существует.
Можно создать его командой:

```bash
mkdir -p /workspace/target
```

## Тесты

```bash
dotnet test -v minimal
```
