using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Usenet.Exceptions;
using Usenet.Nntp.Parsers;
using Usenet.Nntp.Responses;
using Usenet.Util;

namespace Usenet.Nntp
{
    public class PipelineNntpConnection : IAsyncNntpConnection
    {
        private readonly TcpClient client = new TcpClient();
        private StreamWriter sslStreamWriter;
        private PipeReader sslStreamReader;

        public async Task<TResponse> CommandAsync<TResponse>(string command, IAsyncResponseParser<TResponse> parser)
        {
            // send command
            await sslStreamWriter.WriteLineAsync(command);

            // return response
            return await parser.ParseAsync(sslStreamReader);
        }

        public async Task<TResponse> ConnectAsync<TResponse>(string hostname, int port, bool useSsl, IAsyncResponseParser<TResponse> parser)
        {
            // connect
            await client.ConnectAsync(hostname, port);

            // setup SSL
            var sslStream = new SslStream(client.GetStream());
            await sslStream.AuthenticateAsClientAsync(hostname);

            // create writer for sending commands
            sslStreamWriter = new StreamWriter(sslStream, UsenetEncoding.Default) { AutoFlush = true };

            // create reader for receiving responses
            sslStreamReader = PipeReader.Create(sslStream);

            // return response.
            return await parser.ParseAsync(sslStreamReader);
        }

        public async Task<TResponse> MultiLineCommandAsync<TResponse>(string command, IAsyncMultiLineResponseParser<TResponse> parser)
        {
            // execute command
            NntpResponse response = await CommandAsync(
                command,
                new ResponseParser());

            // process payload
            return await parser.ParseAsync(response.Code, response.Message, sslStreamReader);
        }
        
        public static async ValueTask<string> ReadLineAsync(PipeReader reader)
        {
            while (true)
            {
                var result = await reader.ReadAsync();
                var buffer = result.Buffer;
                
                // In the event that no message is parsed successfully, mark consumed
                // as nothing and examined as the entire buffer.
                var consumed = buffer.Start;
                var examined = buffer.End;
                try
                {
                    // Lines end in \r\n
                    // Look for EOL in the buffer.
                    var positionCr = buffer.PositionOf((byte)'\r');
                    var positionLf = buffer.PositionOf((byte)'\n');

                    if (positionCr.HasValue && positionLf.HasValue)
                    {
                        // convert to string
                        var lineData = buffer.Slice(0, positionCr.Value);
                        var line = BufferToString(UsenetEncoding.Default, ref lineData);

                        // skip the line + the \r\n char (consume from buffer)
                        buffer = buffer.Slice(buffer.GetPosition(1, positionLf.Value));

                        // set consumed
                        consumed = buffer.Start;
                        examined = consumed;

                        // return the line
                        return line;
                    }
                    
                    // stop reading if no more data
                    if (result.IsCompleted)
                    {
                        if (buffer.Length > 0)
                        {
                            // The message is incomplete and there's no more data to process.
                            throw new InvalidDataException("Incomplete message.");
                        }
                        break;
                    }
                }
                finally
                {
                    // tell the reader how much buffer we consumed
                    reader.AdvanceTo(consumed, examined);
                }
            }

            return null;
        }

        private static string BufferToString(Encoding encoding, ref ReadOnlySequence<byte> data)
        {
            var sb = new StringBuilder((int)data.Length);
            foreach (var segment in data)
                sb.Append(encoding.GetString(segment.Span));

            return sb.ToString();
        }
    }
}
