// <fileheader>

using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading;

#nullable enable

namespace KoreCommon;

class KoreTcpServerClientConnection : KoreCommonConnection
{
    public bool running;

    public string ParentConnectionName { get; set; } = "";

    public NetworkStream? stream;
    public TcpClient? client;
    public DateTime lastUpdateTime;
    private int NumMsgsHandled;

    private Thread? sendThread;
    private Thread? receiveThread;

    public BlockingCollection<string> sendMsgQueue;

    // Splits messages based on a sentinel character between messages
    private KoreMsgSplitter IncomingMsgSplitter;

    private byte[] ReadBuffer = new byte[100 * 1024]; // 100 KB

    // ------------------------------------------------------------------------------------------------------------

    public KoreTcpServerClientConnection()
    {
        running = false;

        sendMsgQueue = new BlockingCollection<string>();

        NumMsgsHandled = 0;

        char sentinelCharacter = '\u0003';
        IncomingMsgSplitter = new KoreMsgSplitter(sentinelCharacter);

        client = null;
        stream = null;
    }

    public void SetConnectionDetails(TcpClient newTcpClient, NetworkStream newStream)
    {
        client = newTcpClient;
        stream = newStream;
    }

    // ========================================================================================
    // override methods
    // ========================================================================================

    public override KoreConnectionType Type() => KoreConnectionType.TcpServerClient;

    // ------------------------------------------------------------------------------------------------------------

    public override string ConnectionDetailsString()
    {
        return $"type:TcpServerClient // name:{Name} // count:{NumMsgsHandled}";
    }

    // ------------------------------------------------------------------------------------------------------------

    public override void StartConnection()
    {
        running = true;

        // Process the client connection in a new thread.
        sendThread = new Thread(new ThreadStart(SendThreadFunc));
        sendThread?.Start();

        receiveThread = new Thread(new ThreadStart(ReceiveThreadFunc));
        receiveThread?.Start();
    }

    // ------------------------------------------------------------------------------------------------------------

    public override void StopConnection()
    {
        // Set the running flag to false.
        running = false;

        // sendThread?.Interrupt();
        // receiveThread?.Interrupt();

        // if (stream != null)
        // {
        //     stream.Close();
        //     stream.Dispose();
        //     stream = null;
        // }

        // if (client != null)
        // {
        //     client.Close();
        //     client.Dispose();
        //     client = null;
        // }

        // if (sendThread != null && sendThread.IsAlive)
        //     sendThread.Join();

        // if (receiveThread != null && receiveThread.IsAlive)
        //     receiveThread.Join();
    }

    private void FinaliseConnection()
    {
        if (!running)
        {
            if (stream != null)
            {
                stream.Close();
                stream.Dispose();
                stream = null;
            }

            if (client != null)
            {
                client.Close();
                client.Dispose();
                client = null;
            }
        }
    }

    // ------------------------------------------------------------------------------------------------------------

    // The call to send a message adds it to a queue. The send operation may be blocking, so the
    // send thread will unblock on the collection getting a new element and send it on its own
    // timeline.
    public override void SendMessage(string msgData)
    {
        if (running)
            sendMsgQueue.Add(msgData);
    }

    // ========================================================================================

    void SendThreadFunc()
    {
        KoreCentralLog.AddEntry($"Start ServerClient Send Thread {ConnectionDetailsString()}");

        // Enter an infinite loop to process client connections.
        while (running)
        {
            // Blocking call on the new message collection, with a 1sec timeout to allow the running flag to be checked.
            if (sendMsgQueue != null && sendMsgQueue.TryTake(out string? nextMsg, 1000))
            {
                if (nextMsg != null)
                {
                    byte[] msgBuffer = Encoding.ASCII.GetBytes(nextMsg);
                    if (stream != null)
                        stream.Write(msgBuffer, 0, msgBuffer.Length);
                    if (NumMsgsHandled < 10000) NumMsgsHandled++;
                }
            }
        }
    }

    // ------------------------------------------------------------------------------------------------------------

    void ReceiveThreadFunc()
    {
        KoreCentralLog.AddEntry($"Start ServerClient Receive Thread {ConnectionDetailsString()}");

        // Enter a loop to continuously process client requests.
        while (running)
        {
            // Check if the client is still connected, of break out of the infinite loop, to the removal and exit.
            if (client != null)
            {
                if (!client.Connected)
                {
                    KoreCentralLog.AddEntry($"ServerClient Disconnected {ConnectionDetailsString()}");
                    client?.Close();
                    break;
                }
            }

            // Timeout a client if not sent a message recently
            DateTime currentTime = DateTime.UtcNow;
            if (currentTime.Subtract(lastUpdateTime).TotalSeconds > 10)
            {
                KoreCentralLog.AddEntry($"ServerClient Inactive {ConnectionDetailsString()} (10secs)");
                client?.Close();
                break;
            }

            // Check if data is available on the client stream.
            if (stream != null)
            {
                if (stream.DataAvailable)
                {
                    // Update the timer if we have anything.
                    lastUpdateTime = DateTime.Now;

                    // Read data from the client stream.
                    int bytesRead = stream.Read(ReadBuffer, 0, ReadBuffer.Length);

                    if (bytesRead == 0)
                    {
                        // listener was closed or broken, exit loop
                        running = false;
                        continue; // to top of loop to exit
                    }
                    else
                    {
                        // Read the raw incoming data
                        string data = Encoding.ASCII.GetString(ReadBuffer, 0, bytesRead);

                        IncomingMsgSplitter.AddRawMessage(data);
                    }
                }
            }

            // When messages arrive close together, there can be multiple-per-input, so we need to "while"
            // to ensure they all get sent onto the main application.
            while (IncomingMsgSplitter.HasMessage())
            {
                // create a new message object to add to the incoming message queue.
                QueueIncomingMessage(IncomingMsgSplitter.NextMsg());
                if (NumMsgsHandled < 10000) NumMsgsHandled++;
            }

            // Put the thread on a timer, so it doesn't 100% busy-wait
            var now = DateTime.Now;
            var waitTime = 100 - (now.Millisecond % 100); // Calculate the time to wait until the next 0.1 second interval
            Thread.Sleep(waitTime);
        }

        FinaliseConnection();
    }

} // class


