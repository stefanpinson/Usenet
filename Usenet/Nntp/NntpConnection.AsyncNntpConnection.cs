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
        public async Task<TResponse> CommandAsync<TResponse>(string command, IResponseParser<TResponse> parser)
        {
            ThrowIfNotConnected();
            await writer.WriteLineAsync(command);
            return await GetResponseAsync(parser);
        }

        public async Task<TResponse> MultiLineCommandAsync<TResponse>(string command, IAsyncMultiLineResponseParser<TResponse> parser)
        {
            NntpResponse response = await CommandAsync(command, new ResponseParser());

            var dataBlock = parser.IsSuccessResponse(response.Code)
                ? ReadMultiLineDataBlockAsync()
                : AsyncEnumerable.Empty<string>();

            return await parser.ParseAsync(response.Code, response.Message, dataBlock);
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

        private async IAsyncEnumerable<string> ReadMultiLineDataBlockAsync()
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }
}
