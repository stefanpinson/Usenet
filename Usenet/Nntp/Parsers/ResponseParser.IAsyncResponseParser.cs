using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using Usenet.Nntp.Responses;
using Usenet.Util;

namespace Usenet.Nntp.Parsers
{
    internal partial class ResponseParser : IAsyncResponseParser<NntpResponse>
    {
        public async Task<NntpResponse> ParseAsync(PipeReader reader)
        {
            var line = await PipelineNntpConnection.ReadLineAsync(reader);
            
            try
            {
                var code = Convert.ToInt32(line.Substring(0, 3));
                var message = line.Substring(3);

                return new NntpResponse(code, message, IsSuccessResponse(code));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Could not parse response: {ex}");
                throw;
            }
        }
    }
}
