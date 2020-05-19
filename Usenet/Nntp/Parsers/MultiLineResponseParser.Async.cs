using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usenet.Nntp.Responses;
using Usenet.Util;

namespace Usenet.Nntp.Parsers
{
    internal partial class MultiLineResponseParser : IAsyncMultiLineResponseParser<NntpMultiLineResponse>
    {
        public async Task<NntpMultiLineResponse> ParseAsync(int code, string message, PipeReader reader)
        {
            // Fail fast.
            if (!IsSuccessResponse(code))
                return new NntpMultiLineResponse(code, message, false, Enumerable.Empty<string>());

            // NNTP multi-part terminator
            bool IsTerminatingLine(string line)
            {
                // there is only one character, and that character is a '.'
                return line == ".";
            }

            var lines = new List<string>();
            while (true)
            {
                var line = await PipelineNntpConnection.ReadLineAsync(reader);

                // read lines until single '.'
                if (IsTerminatingLine(line))
                    break;

                // remove leading '.' when lines starts w/ ".."
                if (line.Length > 0 && line[0] == '.')
                    line = line.Substring(1);

                lines.Add(line);
            }
            return new NntpMultiLineResponse(code, message, true, lines);
        }
    }
}
