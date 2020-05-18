using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Usenet.Nntp.Models;
using Usenet.Nntp.Parsers;
using Usenet.Nntp.Responses;

namespace Usenet.Nntp
{
    public class AsyncNntpClient : IAsyncRfc3977, IAsyncRfc4643
    {
        private readonly IAsyncNntpConnection connection;

        public AsyncNntpClient(IAsyncNntpConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Attempts to establish a <a href="https://tools.ietf.org/html/rfc3977#section-5.1">connection</a> with a usenet server.
        /// </summary>
        /// <param name="hostname">The hostname of the usenet server.</param>
        /// <param name="port">The port to use.</param>
        /// <param name="useSsl">A value to indicate whether or not to use SSL encryption.</param>
        /// <returns>true if a connection was made; otherwise false</returns>
        public async Task<bool> ConnectAsync(string hostname, int port, bool useSsl)
        {
            if (string.IsNullOrWhiteSpace(hostname))
                throw new ArgumentNullException(nameof(hostname));

            NntpResponse response = await connection.ConnectAsync(hostname, port, useSsl, new ResponseParser(200, 201));
            return response.Success;
        }

        /// <summary>
        /// The <a href="https://tools.ietf.org/html/rfc4643#section-2.3">AUTHINFO USER and AUTHINFO PASS</a> 
        /// (<a href="https://tools.ietf.org/html/rfc2980#section-3.1.1">ad 1</a>)
        /// commands are used to present clear text credentials to the server.
        /// </summary>
        /// <param name="username">The username to use.</param>
        /// <param name="password">The password to use.</param>
        /// <returns>true if the user was authenticated successfully; otherwise false.</returns>
        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));

            NntpResponse userResponse = await connection.CommandAsync($"AUTHINFO USER {username}", new ResponseParser(281));
            if (userResponse.Success)
            {
                return true;
            }
            if (userResponse.Code != 381 || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }
            NntpResponse passResponse = await connection.CommandAsync($"AUTHINFO PASS {password}", new ResponseParser(281));
            return passResponse.Success;
        }

        /// <summary>
        /// The <a href="https://tools.ietf.org/html/rfc3977#section-6.2.1">ARTICLE</a> command 
        /// selects an article according to the arguments and
        /// presents the entire article (that is, the headers, an empty line, and
        /// the body, in that order) to the client.
        /// </summary>
        /// <param name="messageId">The message-id of the article to received from the server.</param>
        /// <returns>An article response object.</returns>
        public Task<NntpArticleResponse> ArticleAsync(NntpMessageId messageId)
        {
            if (string.IsNullOrWhiteSpace(messageId))
                throw new ArgumentNullException(nameof(messageId));

            return connection.MultiLineCommandAsync(
                $"ARTICLE {messageId}",
                new ArticleResponseParser(ArticleRequestType.Article));
        }
            
    }
}
