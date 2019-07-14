using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Timer {

    public bool started { get; private set; } = false;
    public int millisecond { get; private set; } = 0;

    public float second {
        get {
            return (millisecond == 0) ? 0 : millisecond / 1000;
        }
    }

    public float minute {
        get {
            return (second == 0) ? 0 : second / 60;
        }
    }

    public void StartTimer () {
        started = true;
        millisecond += DateTime.Now.Millisecond;
    }

    public void StopTimer () {
        started = false;
    }

    public void RestartTimer () {
        StopTimer();
        millisecond = 0;
    }
}
