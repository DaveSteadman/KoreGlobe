// <fileheader>

using System.Collections.Generic;
using System.Collections.Concurrent;

namespace KoreCommon;

public abstract class KoreCommonConnection
{
    public string Name { set; get; } = "DefaultConnection";

    public abstract KoreConnectionType Type();
    public abstract string ConnectionDetailsString();

    public abstract void SendMessage(string msgData);

    public abstract void StartConnection();
    public abstract void StopConnection();

    // ========================================================================================
    // Incoming message queue
    // ========================================================================================

    public BlockingCollection<KoreMessageText> IncomingQueue = new BlockingCollection<KoreMessageText>();
    public List<KoreMessageText> IncomingMessageLog = new List<KoreMessageText>();

    public void SetupIncomingQueue(BlockingCollection<KoreMessageText> newIncomingQueue)
    {
        IncomingQueue = newIncomingQueue;
        IncomingMessageLog = new List<KoreMessageText>();
    }

    public void QueueIncomingMessage(string msgData)
    {
        KoreMessageText newMsg = new KoreMessageText();

        newMsg.connectionName = Name;
        newMsg.msgData = msgData;

        IncomingMessageLog.Add(newMsg);
        IncomingQueue.Add(newMsg);
    }

    public void QueueIncomingMessage(KoreMessageText newMsg)
    {
        IncomingMessageLog.Add(newMsg);
        IncomingQueue.Add(newMsg);
    }

    public bool HasIncomingMessage()
    {
        return IncomingQueue.Count > 0;
    }

    public KoreMessageText GetNextIncomingMessage()
    {
        return IncomingQueue.Take();
    }

}

