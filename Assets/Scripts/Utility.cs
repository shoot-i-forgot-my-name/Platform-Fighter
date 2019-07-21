using System;
using System.Timers;

public static class Utility {

    public static Action<T> CallActionOnce<T> (Action<T> action) {
        var contextCalled = false;
        Action<T> ret = (T param) => {
            if (!contextCalled) {
                action(param);
                contextCalled = true;
            }
        };

        return ret;
    }

    public static Func<T, TResult, TResult> CallFuncOnce<T, TResult> (Func<T, TResult> func) {
        var contextCalled = false;

        Func<T, TResult, TResult> @return = (T param, TResult failParam) => {
            if (!contextCalled) {
                contextCalled = true;
                return func(param);
            }

            return failParam;
        };

        return @return;
    }

    public static Func<TResult, TResult> CallFuncOnce<TResult> (Func<TResult> func) {
        var contextCalled = false;

        Func<TResult, TResult> @return = (TResult failParam) => {
            if (!contextCalled) {
                contextCalled = true;
                return func();
            }

            return failParam;
        };

        return @return;
    }

    public static void SetTimeout (ElapsedEventHandler action, int mileseconds) {
        Timer tmr = new Timer();
        tmr.Interval = mileseconds;

        tmr.Elapsed += action;
        tmr.Elapsed += (object sender, ElapsedEventArgs e) => tmr.Stop();

        tmr.Start();
    }

}
