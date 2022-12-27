using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [SerializeField]
    private Transform _collisionRoot;
    [SerializeField]
    private int _ticksPerSecond = 5;

    void Start()
    {
        var inputStream = SnakeOperations.SnakeInputObservable().StartWith(Vector3.down);
        var tickStream = SnakeOperations.TickObservable(_ticksPerSecond);

        tickStream
            .WithLatestFrom(inputStream, (tick, input) => input)
            .ValidationScan()
            .Subscribe(input => transform.localPosition += input);

        _collisionRoot.OnTriggerEnterAsObservable().Subscribe(x => Debug.Log(x.name)).AddTo(this);
    }
}
