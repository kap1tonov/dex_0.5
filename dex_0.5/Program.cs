using Refit;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Channels;
using testRefit;



using static testRefit.DiscordApiLibrary;

public class DiscordApiClient
{
    private static readonly HttpClient Client = new HttpClient { BaseAddress = new Uri("https://discord.com/api/") };

    public static async Task ManageChannel(IUserApi userApi, string token, string channelId)
    {
        Console.WriteLine("Введите номер действия:");
        Console.WriteLine("1. Написать сообщение");
        Console.WriteLine("2. Обновить сообщение");
        Console.WriteLine("3. Удалить сообщение");
        Console.WriteLine("4. Поставить реакцию");

        if (int.TryParse(Console.ReadLine(), out int action) && action > 0 && action <= 4)
        {
            Console.WriteLine("Список сообщений в канале: ");
            var messages = await userApi.GetChannelMessagesAsync(channelId, token);
            int index = 1;
            var messageList = messages.ToList();
            foreach (var message in messageList)
            {
                Console.WriteLine($"{index++}. {message.Content} (ID: {message.Id})");
            }
            Console.WriteLine("");

            switch (action)
            {
                case 1:
                    Console.WriteLine("Введите текст сообщения:");
                    var messageContent = Console.ReadLine();
                    var messageRequest = new MessageCreateRequest { Content = messageContent };
                    var createdMessage = await userApi.CreateMessageAsync(channelId, messageRequest, token);
                    Console.WriteLine("Сообщение создано успешно");
                    break;
                case 2:
                    Console.WriteLine("Введите номер сообщения для обновления:");
                    if (int.TryParse(Console.ReadLine(), out int messageId) && messageId > 0 && messageId <= messageList.Count)
                    {
                        var messageToUpdate = messageList[messageId - 1];
                        Console.WriteLine("Введите новый текст сообщения:");
                        var newMessageContent = Console.ReadLine();
                        var updateRequest = new MessageUpdateRequest { Content = newMessageContent };
                        await userApi.UpdateMessageAsync(channelId, messageToUpdate.Id, updateRequest, token);
                        Console.WriteLine("Сообщение обновлено успешно");
                    }
                    break;
                case 3:
                    Console.WriteLine("Введите номер сообщения для удаления:");
                    if (int.TryParse(Console.ReadLine(), out int deleteMessageId) && deleteMessageId > 0 && deleteMessageId <= messageList.Count)
                    {
                        var messageToDelete = messageList[deleteMessageId - 1];
                        await userApi.DeleteMessageAsync(channelId, messageToDelete.Id, token);
                        Console.WriteLine("Сообщение удалено успешно");
                    }
                    break;
                case 4:
                    Console.WriteLine("Введите номер сообщения для добавления реакции:");
                    if (int.TryParse(Console.ReadLine(), out int reactionMessageId) && reactionMessageId > 0 && reactionMessageId <= messageList.Count)
                    {
                        var messageToReact = messageList[reactionMessageId - 1];
                        var emoji = "👍";
                        await userApi.CreateReactionAsync(channelId, messageToReact.Id, emoji, token);
                        Console.WriteLine("Реакция добавлена успешно");
                    }
                    break;
            }
        }
    }

    public static async Task ManageServer(IUserApi userApi, string token)
    {
        Console.WriteLine("Введите номер действия:");
        Console.WriteLine("1. Посмотреть сервера");
        Console.WriteLine("2. Получить сервер по Id");
        Console.WriteLine("3. Удалить сервер");
        Console.WriteLine("4. Изменить никнейм на сервере");

        if (int.TryParse(Console.ReadLine(), out int action) && action > 0 && action <= 4)
        {
            switch (action)
            {
                case 1:
                    Console.WriteLine("Список серверов пользователя:");
                    var servers = await userApi.GetServersAsync(token);
                    int index = 1;
                    foreach (var server in servers)
                    {
                        Console.WriteLine($"{index++}. {server.Name} (ID: {server.Id})");
                    }
                    break;
                case 2:
                    Console.WriteLine("Введите Id сервера:");
                    var serverId = Console.ReadLine();
                    var serverById = await userApi.GetServerByIdAsync(serverId, token);
                    Console.WriteLine($"Название сервера: {serverById.Name}");
                    Console.WriteLine("Список каналов на сервере:");
                    var channels = await userApi.GetChannelsAsync(serverId, token);
                    foreach (var channel in channels)
                    {
                        Console.WriteLine($"- {channel.Name} (ID: {channel.Id})");
                    }
                    break;
                case 3:
                    Console.WriteLine("Введите Id сервера для удаления:");
                    var deleteServerId = Console.ReadLine();
                    await userApi.DeleteServerAsync(deleteServerId, token);
                    Console.WriteLine("Сервер удален успешно");
                    break;
            }
        }
    }
    public static async Task ManageUser(IUserApi userApi, string token)
    {
        Console.WriteLine("Введите номер действия:");
        Console.WriteLine("1. Изменить имя пользователя");
        Console.WriteLine("2. Изменить аватар пользователя");

        if (int.TryParse(Console.ReadLine(), out int action) && action > 0 && action <= 2)
        {
            switch (action)
            {
                case 1:
                    Console.WriteLine("Введите новый никнейм:");
                    var newNickname = Console.ReadLine();
                    Console.WriteLine("Введите Id сервера:");
                    var serverId = Console.ReadLine();
                    var updateNicknameRequest = new UpdateNicknameRequest { Nick = newNickname };
                    await userApi.UpdateNicknameAsync(serverId, updateNicknameRequest, token);
                    Console.WriteLine("Никнейм изменен успешно");
                    break;

                    /* case 2:
                         Console.WriteLine("Введите путь к новому аватару:");
                         var avatarPath = Console.ReadLine();
                         var base64Avatar = Convert.ToBase64String(File.ReadAllBytes(avatarPath));
                         var updateUserRequest = new UpdateUserRequest { Avatar = $"data:image/png;base64,{base64Avatar}" };
                         await userApi.UpdateUserAsync(updateUserRequest, token);
                         Console.WriteLine("Аватар изменен успешно");
                         break;
                    */
            }
        }
    }


    public static async Task Main(string[] args)
    {
        string token = "token";
        var api = RestService.For<IUserApi>("https://discord.com/api/v9");
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var discordApi = RestService.For<IDiscordApi>(Client);
        var userApi = RestService.For<IUserApi>(Client);

        Console.WriteLine("Выберите режим:");
        Console.WriteLine("1. Управление каналом");
        Console.WriteLine("2. Управление серверами");
        Console.WriteLine("3. Управление пользователем");

        if (int.TryParse(Console.ReadLine(), out int mode) && mode > 0 && mode <= 3)
        {
            var channelId = "channelId";

            if (mode == 1)
            {
                await ManageChannel(userApi, token, channelId);
            }
            else if (mode == 2)
            {
                await ManageServer(userApi, token);
            }
            else if (mode == 3)
            {
                await ManageUser(userApi, token);
            }
        }
    }
}
