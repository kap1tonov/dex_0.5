using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static testRefit.DiscordApiLibrary;
using Refit;
using Newtonsoft.Json;



namespace testRefit
{
    public class DiscordApiLibrary
    {
        public class Server
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public IEnumerable<Channel> Channels { get; set; }
        }

        public class Channel
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class CreateServerRequest
        {
            public string Name { get; set; }
        }

        public class CreateServerResponse
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class MessageCreateRequest
        {
            public string Content { get; set; }
        }

        public class MessageUpdateRequest
        {
            public string Content { get; set; }
        }

    }

    public interface IUserApi
    {
        [Get("/users/@me/guilds")]
        Task<IEnumerable<Server>> GetServersAsync([Header("Authorization")] string authToken);

        [Get("/guilds/{serverId}/channels")]
        Task<IEnumerable<DiscordApiLibrary.Channel>> GetChannelsAsync(string serverId, [Header("Authorization")] string authToken);

        [Get("/channels/{channelId}/messages")]
        Task<IEnumerable<Message>> GetMessagesAsync(string channelId, [Header("Authorization")] string authToken, [Query] int limit);

        [Get("/guilds/{serverId}")]
        Task<Server> GetServerByIdAsync(string serverId, [Header("Authorization")] string authToken);

        [Post("/guilds")]
        Task<CreateServerResponse> CreateServerAsync([Body] CreateServerRequest request, [Header("Authorization")] string authToken);

        [Delete("/guilds/{serverId}")]
        Task DeleteServerAsync(string serverId, [Header("Authorization")] string authToken);

        [Get("/channels/{channelId}/messages")]
        Task<IEnumerable<Message>> GetChannelMessagesAsync(string channelId, [Header("Authorization")] string authToken);

        [Post("/channels/{channelId}/messages")]
        Task<Message> CreateMessageAsync(string channelId, [Body] MessageCreateRequest request, [Header("Authorization")] string authToken);

        [Patch("/channels/{channelId}/messages/{messageId}")]
        Task<Message> UpdateMessageAsync(string channelId, string messageId, [Body] MessageUpdateRequest request, [Header("Authorization")] string authToken);

        [Delete("/channels/{channelId}/messages/{messageId}")]
        Task DeleteMessageAsync(string channelId, string messageId, [Header("Authorization")] string authToken);

        [Put("/channels/{channelId}/messages/{messageId}/reactions/{emoji}/@me")]
        Task CreateReactionAsync(string channelId, string messageId, string emoji, [Header("Authorization")] string authToken);

        [Patch("/guilds/{serverId}/members/@me/nick")]
        Task UpdateNicknameAsync(string serverId, [Body] UpdateNicknameRequest request, [Header("Authorization")] string authToken);

        /*
        [Patch("/users/@me")]
        Task UpdateUserAsync([Body] UpdateUserRequest request, [Header("Authorization")] string token);
        */

    }

    public interface IDiscordApi
    {
        [Get("/users/@me")]
        Task<UserData> GetUserDataAsync([Header("Authorization")] string authToken);
    }

    public class UserData
    {
        public string Username { get; set; }
    }

    public class Channel
    {
        public string Name { get; set; }
    }

    public class Message
    {
        public string Id { get; set; }
        public string Content { get; set; }
    }
    public class UpdateNicknameRequest
    {
        public string Nick { get; set; }
    }
    public class UpdateUserRequest
    {
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
    }

}


