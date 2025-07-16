using System.Collections.Generic;
using System.Collections.Concurrent;

namespace KoreCommon;

public abstract class KoreCommonConnection
{
    public string Name { set; get; } = "DefaultConnection";

    public abstract string type();
    public abstract string connectionDetailsString();

    public abstract void sendMessage(string msgData);

    public abstract void startConnection();
    public abstract void stopConnection();

    // ========================================================================================
    // Incoming message queue
    // ========================================================================================

    public BlockingCollection<KoreMessageText> IncomingQueue = new BlockingCollection<KoreMessageText>();
    public List<KoreMessageText> IncomingMessageLog = new List<KoreMessageText>();

    public void setupIncomingQueue(BlockingCollection<KoreMessageText> newIncomingQueue)
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

    public bool hasIncomingMessage()
    {
        return IncomingQueue.Count > 0;
    }

    public KoreMessageText getNextIncomingMessage()
    {
        return IncomingQueue.Take();
    }

}

