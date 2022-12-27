using UnityEngine;
using UniRx;
using System;
using System.Collections;
using System.Collections.Generic;

public static class SnakeOperations
{
    public static IObservable<Unit> OnKeyDownObservable(KeyCode key)
    {
        return Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(key))
            .AsUnitObservable();
    }


    public static IObservable<Vector3> SnakeInputObservable()
    {
        var w = OnKeyDownObservable(KeyCode.W).Select(_ => Vector3.up);
        var a = OnKeyDownObservable(KeyCode.A).Select(_ => Vector3.left);
        var s = OnKeyDownObservable(KeyCode.S).Select(_ => Vector3.down);
        var d = OnKeyDownObservable(KeyCode.D).Select(_ => Vector3.right);
        return Observable.Merge(w, a, s, d);
    }

    public static IObservable<long> TickObservable(double ticksPerSecond)
    {
        return Observable.Interval(TimeSpan.FromSeconds(1 / ticksPerSecond));
    }

    public static IObservable<Vector3> ValidationScan(this IObservable<Vector3> source)
    {
        return source.Scan((acc, input) =>
        {
            if (input + acc == Vector3.zero)
            {
                return acc;
            }
            return input;
        });
    }

    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T concatValue)
    {
        foreach(var a in source)
        {
            yield return a;
        }

        yield return concatValue;
    }
}
