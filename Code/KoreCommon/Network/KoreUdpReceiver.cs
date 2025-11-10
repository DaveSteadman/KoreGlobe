// <fileheader>

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace KoreCommon;

public static class AsyncExtensions
{
    public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>();
        using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s!).TrySetResult(true), tcs))
        {
            if (task != await Task.WhenAny(task, tcs.Task))
            {
                throw new OperationCanceledException(cancellationToken);
            }
        }

        return task.Result;
    }
}


class KoreUdpReceiver : KoreCommonConnection
{
    public int port;
    private UdpClient? UdpClient;
    private IPEndPoint? localEndPoint;
    private int NumMsgsHandled;

    private CancellationTokenSource? _cancellationTokenSource;

    // ------------------------------------------------------------------------------------------------------------

    public KoreUdpReceiver()
    {
        localEndPoint  = null;
        port           = 0;
        NumMsgsHandled = 0;
        UdpClient      = null;
        _cancellationTokenSource = null;
    }

    public void SetConnectionDetails(int inPort)
    {
        port = inPort;
    }

    // ========================================================================================
    // override methods
    // ========================================================================================

    public override KoreConnectionType Type() => KoreConnectionType.UdpReceiver;

    // ------------------------------------------------------------------------------------------------------------

    public override string ConnectionDetailsString()
    {
        string addr = "<null>";
        if (localEndPoint != null)
            addr = localEndPoint.Address.ToString();

        return $"type:UdpReceiver // name:{Name} // addr:{addr}:{port} // count:{NumMsgsHandled}";
    }

    // ------------------------------------------------------------------------------------------------------------

    public override void StartConnection()
    {
        // create Udp Port
        UdpClient = new UdpClient(new IPEndPoint(IPAddress.Any, port));
        localEndPoint = UdpClient?.Client.LocalEndPoint as IPEndPoint;

        _cancellationTokenSource = new CancellationTokenSource();
        Task.Run(() => ReceiveThreadFuncAsync(_cancellationTokenSource.Token));
    }

    // ------------------------------------------------------------------------------------------------------------

    public override void StopConnection()
    {
        _cancellationTokenSource?.Cancel();
        UdpClient?.Close();
    }

    // ------------------------------------------------------------------------------------------------------------

    public override void SendMessage(string msgData)
    {
        // A receiver class, should be used to send.
    }

    // ========================================================================================

    private async Task ReceiveThreadFuncAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                // Sender object records the incoming message details, recreated for each incoming message
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

                // Receive a message. BLOCKING.
                if (UdpClient != null)
                {
                    var result = await UdpClient.ReceiveAsync().WithCancellation(token);
                    byte[] receiveBytes = result.Buffer;
                    string returnData = Encoding.UTF8.GetString(receiveBytes);
                    QueueIncomingMessage(returnData);
                    if (NumMsgsHandled<10000) NumMsgsHandled++;
                }
                else
                    throw new Exception("UdpClient is null within ReceiveThreadFuncAsync()");
            }
            catch (OperationCanceledException)
            {
                // UdpClient was closed, exit loop
            }
        }
    }
}
