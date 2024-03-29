# Winestro-A
Это проект на WinUI 3 с использованием C#/XAML для удобного управления дискорд ботом

## Скриншоты
![image](https://github.com/aexra/Winestro-A/assets/121866384/6ece6367-0adc-4816-8210-9a97c239b2e1)
![image](https://github.com/aexra/Winestro-A/assets/121866384/b9f2eb9e-6e6e-4011-b41c-995ea00b2197)

## Возможности
Бот предлагает возможности использования на серверах комманд для воспроизведения музыки из Youtube по переданной ссылке или названию видео и управления этим воспроизведением.<br>
Также реализована очередь и воспроизведение плейлистов.<br>
На главной странице бота вы можете отслеживать информацию о нем, а также просматривать системные логи и пользоваться консолью для использования различных операций, которые вы также можете сами определять (см. Использование)

## Использование
### База
Если Вы хотите использовать этот проект для своего бота, Вам придется выполнить несколько следующих простых шагов:
1. Клонировать этот репозиторий и сбилдить в Visual Studio 2022+
2. В открывшемся приложении, во вкладке Config добавить два ключ-значения:
   - Токен вашего бота
   - ID вашего тестового сервера

![image](https://github.com/aexra/Winestro-A/assets/121866384/17b8c499-da7f-48c8-90c5-43689419291c)

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
А также объявить команду (зарегистрировать в дискорде и написать у себя не одно и то же)
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
Для более детальной информации о создании модулей [Interaction Framework](https://docs.discordnet.dev/guides/int_framework) можно прочитать [тут](https://docs.discordnet.dev/guides/int_framework/intro.html#interaction-context)
