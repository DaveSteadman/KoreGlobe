using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace KoreCommon;
    /*
    public struct CommsMessage
    {
        public string connectionName;
        public string msgData;

        public string msgDebug()
        {
            return "<connectionName:" + connectionName + ", msgData:" + msgData + ">";
        }
    }
    */

class KoreMsgSplitter
{
    private char sentinel;
    StringBuilder inBuffer = new StringBuilder();
    private List<KoreMessageText> OutBuffers = new List<KoreMessageText>();

    // Create a debug logfile of everythgin that passes through this function
    private string debugLogFilename = "KoreMsgSplitter.log";


    public KoreMsgSplitter(char sentinelCharacter)
    {
        sentinel = sentinelCharacter;
        inBuffer.Clear();
    }

    public void AddRawMessage(string inRawMsg)
    {
        // open the debug file and append the new string
        System.IO.File.AppendAllText(debugLogFilename, inRawMsg);


        // Loop through each new character
        foreach (char c in inRawMsg)
        {
            if (c == sentinel)
            {
                // Add the message to the list
                KoreMessageText newMsg = new KoreMessageText() { connectionName = "UNDEFINED", msgData = inBuffer.ToString() };
                OutBuffers.Add(newMsg);

                KoreCentralLog.AddEntry($"=====> Message from SENTINEL character: {newMsg.msgData}");

                // Clear the buffer
                inBuffer.Clear();
            }
            else
            {
                inBuffer.Append(c);
            }
        }
    }

    public bool HasMessage()
    {
        return (OutBuffers.Count > 0);
    }

    public KoreMessageText NextMsg()
    {
        if (OutBuffers.Count == 0)
        {
            throw new InvalidOperationException("No messages available.");
        }

        KoreMessageText n = OutBuffers[0];
        OutBuffers.RemoveAt(0); // Adding at last, reading and removing at first
        return n;
    }
}
