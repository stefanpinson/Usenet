using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usenet.Exceptions;
using Usenet.Logging;
using Usenet.Nntp.Parsers;
using Usenet.Nntp.Responses;

namespace Usenet.Nntp
{
    public partial class NntpConnection : IAsyncNntpConnection
    {
        public async Task<TResponse> MultiLineCommandAsync<TResponse>(string command, IAsyncMultiLineResponseParser<TResponse> parser)
        {
            throw new NotImplementedException();
        }

        public async Task<TResponse> GetResponseAsync<TResponse>(IResponseParser<TResponse> parser)
        {
            string responseText = await reader.ReadLineAsync();
            log.Info("Response received: {Response}", responseText);

            if (responseText == null)
            {
                throw new NntpException("Received no response.");
            }
            if (responseText.Length < 3 || !int.TryParse(responseText.Substring(0, 3), out int code))
            {
                throw new NntpException("Received invalid response.");
            }
            return parser.Parse(code, responseText.Substring(3).Trim());
        }

        public Task<TResponse> ConnectAsync<TResponse>(string hostname, int port, bool useSsl, IAsyncResponseParser<TResponse> parser)
        {
            throw new NotImplementedException();
        }

        public Task<TResponse> CommandAsync<TResponse>(string command, IAsyncResponseParser<TResponse> parser)
        {
            throw new NotImplementedException();
        }
    }
}
