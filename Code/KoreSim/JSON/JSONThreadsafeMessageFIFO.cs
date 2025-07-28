using System;
using System.Collections.Concurrent;

namespace KoreSim.JSON;

#nullable enable

public class JSONThreadsafeMessageFIFO
{
    private ConcurrentQueue<JSONMessage> MessageQueue = new ConcurrentQueue<JSONMessage>();

    public int NumMessages => MessageQueue.Count;

    // EnqueueMessage method using expression-bodied member syntax
    public void EnqueueMessage(JSONMessage message) => MessageQueue.Enqueue(message);

    // TryDequeueMessage method using expression-bodied member syntax
    public bool TryDequeueMessage(out JSONMessage? message) => MessageQueue.TryDequeue(out message);
}
