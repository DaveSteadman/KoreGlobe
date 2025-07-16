using System;

#nullable enable

namespace KoreCommon;


public class KoreThreadSafeValue<T> where T : class?
{
    private T? _value; // Mark the field as nullable
    private readonly object _lock = new();
    private bool _isSet = false;

    public T? Value // Mark the property as nullable
    {
        get
        {
            lock (_lock)
            {
                return _isSet ? _value : default;
            }
        }
        set
        {
            lock (_lock)
            {
                _value = value;
                _isSet = true;
            }
        }
    }

    public bool IsSet
    {
        get
        {
            lock (_lock)
            {
                return _isSet;
            }
        }
    }
}
