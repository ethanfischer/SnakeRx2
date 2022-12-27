using Assets;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [SerializeField]
    private Transform _collisionRoot;
    [SerializeField]
    private int _ticksPerSecond = 5;
    [SerializeField]
    private GameObject _snakePrefab;

    void Start()
    {
        var inputStream = SnakeOperations.SnakeInputObservable().StartWith(Vector3.down);
        var tickStream = SnakeOperations.TickObservable(_ticksPerSecond);

        tickStream
            .WithLatestFrom(inputStream, (tick, input) => input)
            .ValidationScan()
            .Subscribe(input => transform.localPosition += input);

        var snakeBodyGrowthStream = _collisionRoot
            .OnTriggerEnterAsObservable()
            .Where(trigger => trigger.gameObject.GetComponent<Apple>() != null)
            .Select(x => Instantiate(_snakePrefab))
            .StartWith(Instantiate(_snakePrefab))
            .Scan(new GameObject[] { Instantiate(_snakePrefab) }, (acc, src) =>
            {
                return acc.Concat(src).ToArray();
            })
            .Publish();

        snakeBodyGrowthStream.Connect();

        tickStream.WithLatestFrom(snakeBodyGrowthStream, (tick, snakeBody) => snakeBody);
    }
}
