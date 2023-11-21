## О чем проект?
Это ВК Бот, который оставляет роспись в комментарии под постом от пользователя. Пользователь репостит запись со своей страницы или скидывает ссылку на пост с сообщением /репост, а бот в ответ оставляет роспись.

## Особенности кода
* Парсинг айди поста из ссылки.
* Реализован паттерн инъекция зависимостей.
* Код асинхронный. Может обрабатывать несколько запросов одновременно.
* Настройка бота через файл конфигурации.
* Код поделен на методы, которые выполняют свою конкретную задачу.

## Dependencies
```powershell
# For both projects
dotnet add package Microsoft.Extensions.Hosting --version 7.0.1
dotnet add package Microsoft.Extensions.DependencyInjection --version 8.0.0
# Only for VkSignBot
dotnet add package VkNet --version 1.76.0
```

## Configuration
Inside the appsettings.json fill up these options.
```json
{
    "Vk": {
        "BotToken": "",
        "AppToken": "",
        "AppId": 0,
        "GroupId": 0,
        "Admins": []
    }
}
```
