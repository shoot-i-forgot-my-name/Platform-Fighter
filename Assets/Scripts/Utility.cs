using System;

public static class Utility
{

    public static Action<T> CallActionOnce<T> (Action<T> action)
    {
        var called = false;
        return (T param) =>
        {
            if (!called)
            {
                action(param);
            }
        };
    }

}
