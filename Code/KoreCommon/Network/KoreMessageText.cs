// <fileheader>

using System;

namespace KoreCommon;

// At the lowest level, we deal with sending messages in text. When we construct of decode a message,
// this text is prsed as JSON.

public struct KoreMessageText
{
    public string connectionName;
    public string msgData;

    public string msgDebug()
    {
        return $"<{connectionName} => {msgData}>";
    }

    public override string ToString()
    {
        return $"<{connectionName} => {msgData}>";
    }

    public bool HasValidContent()
    {
        if (string.IsNullOrEmpty(connectionName) || string.IsNullOrEmpty(msgData))
        {
            return false;
        }
        return true;
    }
}

