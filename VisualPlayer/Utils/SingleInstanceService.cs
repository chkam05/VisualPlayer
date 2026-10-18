using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace VisualPlayer.Utils;

// Own the mutex on the entry thread for the entire desktop lifetime. The pipe
// starts before Avalonia so launches during initialization can already be queued.
internal sealed class SingleInstanceService : IDisposable
{
    private readonly string _name;
    private readonly Mutex _mutex;
    private readonly Channel<string[]> _requests = Channel.CreateUnbounded<string[]>();
    private readonly CancellationTokenSource _shutdown = new();
    
    private Task? _server;
    private Task? _receiver;
    private bool _ownsMutex;

    #region CONSTRUCTORS

    public SingleInstanceService(string applicationId = "VisualPlayer")
    {
        _name = applicationId + "-" + Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(
                Environment.UserDomainName + "\\" + Environment.UserName)))[..24];
        _mutex = new Mutex(false, _name);
    }

    public void Dispose()
    {
        _shutdown.Cancel();
        _requests.Writer.TryComplete();
        try
        {
            _server?.GetAwaiter().GetResult();
            _receiver?.GetAwaiter().GetResult();
        }
        catch (OperationCanceledException)
        {
        }

        if (_ownsMutex)
            _mutex.ReleaseMutex();
        _mutex.Dispose();
        _shutdown.Dispose();
    }
    
    #endregion CONSTRUCTORS

    public bool ForwardToExistingInstance(string[] args)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        while (!timeout.IsCancellationRequested)
        {
            try
            {
                _ownsMutex = _mutex.WaitOne(0);
            }
            catch (AbandonedMutexException)
            {
                _ownsMutex = true;
            }

            if (_ownsMutex)
            {
                _server = Task.Run(ListenAsync);
                return false;
            }

            try
            {
                using var pipe = new NamedPipeClientStream(".", _name,
                    PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
                pipe.ConnectAsync(500, timeout.Token).GetAwaiter().GetResult();
                using var writer = new StreamWriter(pipe, new UTF8Encoding(false), leaveOpen: true);
                writer.WriteLineAsync(JsonSerializer.Serialize(args).AsMemory(), timeout.Token)
                    .GetAwaiter().GetResult();
                writer.FlushAsync(timeout.Token).GetAwaiter().GetResult();
                var acknowledgement = new byte[1];
                pipe.ReadExactlyAsync(acknowledgement, timeout.Token).AsTask().GetAwaiter().GetResult();
                if (acknowledgement[0] == 1)
                    return true;
            }
            catch (Exception e) when (e is IOException or TimeoutException or OperationCanceledException)
            {
                // Retry acquisition as well: the previous instance may be exiting.
            }
        }

        throw new TimeoutException("Could not pass files to the running VisualPlayer instance.");
    }

    public void StartReceiving(Action<string[]> receiver)
    {
        _receiver = Task.Run(async () =>
        {
            await foreach (var args in _requests.Reader.ReadAllAsync(_shutdown.Token))
                receiver(args);
        });
    }

    private async Task ListenAsync()
    {
        while (!_shutdown.IsCancellationRequested)
        {
            try
            {
                using var pipe = new NamedPipeServerStream(_name, PipeDirection.InOut, 1,
                    PipeTransmissionMode.Byte, PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
                await pipe.WaitForConnectionAsync(_shutdown.Token);
                using var requestTimeout = CancellationTokenSource.CreateLinkedTokenSource(_shutdown.Token);
                requestTimeout.CancelAfter(TimeSpan.FromSeconds(5));
                using var reader = new StreamReader(pipe, Encoding.UTF8, leaveOpen: true);
                var json = await reader.ReadLineAsync(requestTimeout.Token);
                var args = json is null ? null : JsonSerializer.Deserialize<string[]>(json);
                if (args is null)
                    continue;

                await _requests.Writer.WriteAsync(args, _shutdown.Token);
                await pipe.WriteAsync(new byte[] { 1 }, requestTimeout.Token);
                await pipe.FlushAsync(requestTimeout.Token);
            }
            catch (Exception e) when (e is IOException or JsonException or OperationCanceledException)
            {
                Trace.WriteLine(e);
            }
        }
    }
}
