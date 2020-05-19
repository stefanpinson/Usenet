using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace Usenet.Nntp.Parsers
{
    /// <summary>
    /// Represents a multi-line response parser.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface IAsyncMultiLineResponseParser<TResponse>
    {
        /// <summary>
        /// Parses the multi-line response of an NNTP command into a new instance of type <typeparamref name="TResponse"/>.
        /// </summary>
        /// <param name="code">The response code received from the server.</param>
        /// <param name="message">The response message received from the server.</param>
        /// <param name="dataBlock">The multi-line datablock as received from the server.</param>
        /// <returns>A new instance of type <typeparamref name="TResponse"/>.</returns>
        Task<TResponse> ParseAsync(int code, string message, PipeReader reader);
    }
}
