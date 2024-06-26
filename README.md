# Winestro-A
Это проект на WinUI 3 с использованием C#/XAML для удобного управления дискорд ботом

- [Скриншоты](#Скриншоты)
- [Возможности](#Возможности)
- [Использование](#Использование)
  - [База](#База)
  - [Кастомизация команд дискорда](#Кастомизация-команд-дискорда)
  - [Кастомизация команд консоли](#Кастомизация-команд-консоли)

## Скриншоты
![image](https://github.com/aexra/Winestro-A/assets/121866384/6ece6367-0adc-4816-8210-9a97c239b2e1)
![image](https://github.com/aexra/Winestro-A/assets/121866384/b9f2eb9e-6e6e-4011-b41c-995ea00b2197)

## Возможности
Бот предлагает возможности использования на серверах комманд для воспроизведения музыки из Youtube по переданной ссылке или названию видео и управления этим воспроизведением.<br>
Также реализована очередь и воспроизведение плейлистов.<br>
На главной странице бота вы можете отслеживать информацию о нем, а также просматривать системные логи и пользоваться консолью для использования различных операций, которые вы также можете сами определять - [Кастомизация команд консоли](#Кастомизация-команд-консоли)

Страница ```Modules``` предназначена для управления модулями проекта, но пока не имеет программной реализации. Предполагается создать подстраницу для каждого модуля

Страница ```Guilds``` позволяет взаимодействовать с подключенными к боту серверами: отправлять в достпуные текстовые каналы сообщения, просматривать последние 100 сообщений истории в них (отправлять пока только текст, просматривать текст и изоражения)

Страница ```Debug``` - это пустая страница, необходимая для проверки работоспособности тех или иных будущих фич (на самом деле удобно использовать для этого консоль и логгер на главной странице для ввода и вывода, но может пригодится во время разработки кастомных элементов управления)

![image](https://github.com/aexra/Winestro-A/assets/121866384/4ac367cb-0138-4cd0-812a-9d6c00c3c9b4)

## Использование
### База
Если Вы хотите использовать этот проект для своего бота, Вам придется выполнить несколько следующих простых шагов:
1. Клонировать этот репозиторий и сбилдить в Visual Studio 2022+
2. В открывшемся приложении, во вкладке Config добавить два ключ-значения:
   - Токен вашего бота ```DiscordApiKey```
   - ID вашего тестового сервера ```DiscordTestGuildID```
3. Для применения комманд к тестовому серверу, необходимо один раз в консоли прописать ```discord commands reg test``` (при условии что бот запущен)

![image](https://github.com/aexra/Winestro-A/assets/121866384/194aabff-9f6f-4cf8-9cd5-47b44b011237)
![image](https://github.com/aexra/Winestro-A/assets/121866384/ca0202cf-0754-4295-9691-bdc8df0f40db)

### Кастомизация команд дискорда
Все команды бота реализованы в модулях по директории: ```Winestro_A/Discord/CommandModules/```<br>
Например: [MusicSlashModule.cs](Winestro-A/Discord/CommandModules/MusicSlashModule.cs)

Вы можете создать новый модуль в этой же папке (вообще в любой, но не зря же я ее создавал?) следующим образом:
```cs
namespace Winestro_A.Discord;
public class SampleSlashModule : InteractionModuleBase<SocketInteractionContext>
{

}
```
А также объявить команду <br> 
> P.S. зарегистрировать в дискорде и написать у себя - не одно и то же
```cs
namespace Winestro_A.Discord;
public class SampleSlashModule : InteractionModuleBase<SocketInteractionContext>
{
   [SlashCommand("test", "My sample command", runMode: RunMode.Sync)]
   public async Task MethodName(...) {
      // ...
      await RespondAsync("Hello, world!");
      // ...   
   }
}
```
Подробнее о создании модулей [Interaction Framework](https://docs.discordnet.dev/guides/int_framework) можно прочитать [тут](https://docs.discordnet.dev/guides/int_framework/intro.html#interaction-context)

### Кастомизация команд консоли
На момент записи README моя консоль поддерживает только определенные в ее же классе команды, поэтому все новые команды должны быть определены в ```Winestro-A/Services/IntegratedConsoleService.cs```, т.е. [тут](Winestro-A/Services/IntegratedConsoleService.cs)

Так выглядит команда с 1 обязательным позиционным аргументом и одним key-word аргументом:
```cs
[ConsoleCommand("sample", Description = "I hate Java", RequiredArgs = 1, KwargsKeys = new string[]{ "keyword_key" })]
private static async Task<ConsoleCommandResult> Sample(ConsoleCommandContext ctx)
{
   return new ConsoleCommandResult($"I hate {ctx.Args.First()}");
}
```

Как это выглядит в консоли:<br>

![image](https://github.com/aexra/Winestro-A/assets/121866384/229707d2-604e-4888-b885-94889efd12f3)
