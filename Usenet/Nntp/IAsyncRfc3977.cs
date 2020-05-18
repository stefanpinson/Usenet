using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Usenet.Nntp.Models;
using Usenet.Nntp.Responses;

namespace Usenet.Nntp

{
    public interface IAsyncRfc3977
    {
        /// <summary>
        /// Attempts to establish a <a href="https://tools.ietf.org/html/rfc3977#section-5.1">connection</a> with a usenet server.
        /// </summary>
        /// <param name="hostname">The hostname of the usenet server.</param>
        /// <param name="port">The port to use.</param>
        /// <param name="useSsl">A value to indicate whether or not to use SSL encryption.</param>
        /// <returns>true if a connection was made; otherwise false</returns>
        Task<bool> ConnectAsync(string hostname, int port, bool useSsl);        

        /// <summary>
        /// The <a href="https://tools.ietf.org/html/rfc3977#section-6.2.1">ARTICLE</a> command 
        /// selects an article according to the arguments and
        /// presents the entire article (that is, the headers, an empty line, and
        /// the body, in that order) to the client.
        /// </summary>
        /// <param name="messageId">The message-id of the article to received from the server.</param>
        /// <returns>An article response object.</returns>
        Task<NntpArticleResponse> ArticleAsync(NntpMessageId messageId);
    }
}
