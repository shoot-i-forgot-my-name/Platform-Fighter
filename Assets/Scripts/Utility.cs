using System;
using System.Collections;
using MonoBehaviour = UnityEngine.MonoBehaviour;
using System.Timers;

public static class Utility {

    public static Action<T> CallActionOnce<T> (Action<T> action) {
        var called = false;
        return (T param) => {
            if (!called) {
                action(param);
            }
        };
    }

}
