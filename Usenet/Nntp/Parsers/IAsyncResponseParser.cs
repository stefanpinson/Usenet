using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

namespace Usenet.Nntp.Parsers
{
    public interface IAsyncResponseParser<TResponse>
    {
        /// <summary>
        /// Parses the response of an NNTP command into a new instance of type <typeparamref name="TResponse"/>.
        /// </summary>
        /// <param name="code">The response code received from the server.</param>
        /// <param name="message">The response message received from the server.</param>
        /// <returns>A new instance of type <typeparamref name="TResponse"/>.</returns>

        Task<TResponse> ParseAsync(PipeReader reader);
    }
}
