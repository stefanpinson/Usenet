using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Usenet.Nntp.Parsers;

namespace Usenet.Nntp
{
    public interface IAsyncNntpConnection
    {
        /// <summary>
        /// Attempts to establish a connection with a usenet server.
        /// </summary>
        /// <typeparam name="TResponse">The type of the parsed response.</typeparam>
        /// <param name="hostname">The hostname of the usenet server.</param>
        /// <param name="port">The port to use.</param>
        /// <param name="useSsl">A value to indicate whether or not to use SSL encryption.</param>
        /// <param name="parser">The response parser to use.</param>
        /// <returns>A response object of type <typeparamref name="TResponse"/>.</returns>
        Task<TResponse> ConnectAsync<TResponse>(string hostname, int port, bool useSsl, IResponseParser<TResponse> parser);

        /// <summary>
        /// Sends a command to the usenet server. The response is expected to be a single line.
        /// </summary>
        /// <typeparam name="TResponse">The type of the parsed response.</typeparam>
        /// <param name="command">The command to send to the server.</param>
        /// <param name="parser">The response parser to use.</param>
        /// <returns>A response object of type <typeparamref name="TResponse"/>.</returns>
        Task<TResponse> CommandAsync<TResponse>(string command, IResponseParser<TResponse> parser);

        /// <summary/// <summary>
        /// Sends a command to the usenet server. The response is expected to be multiple lines.
        /// </summary>
        /// <typeparam name="TResponse">The type of the parsed response.</typeparam>
        /// <param name="command">The command to send to the server.</param>
        /// <param name="parser">The multi-line response parser to use.</param>
        /// <returns>A response object of type <typeparamref name="TResponse"/>.</returns>
        Task<TResponse> MultiLineCommandAsync<TResponse>(string command, IAsyncMultiLineResponseParser<TResponse> parser);

        /// Gets a single-line response from the usenet server.
        /// </summary>
        /// <typeparam name="TResponse">The type of the parsed response.</typeparam>
        /// <param name="parser">The multi-line response parser to use.</param>
        /// <returns>A response object of type <typeparamref name="TResponse"/>.</returns>
        Task<TResponse> GetResponseAsync<TResponse>(IResponseParser<TResponse> parser);
    }
}
