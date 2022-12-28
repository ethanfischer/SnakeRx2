using Assets;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [SerializeField]
    private int _ticksPerSecond = 5;
    [SerializeField]
    private GameObject _snakePrefab;
    [SerializeField]
    private Transform _spawnPoint;

    void Start()
    {
        var inputStream = SnakeOperations.SnakeInputObservable().StartWith(Vector3.down);
        var tickStream = SnakeOperations.TickObservable(_ticksPerSecond);

        var snakeBodyGrowthStream = tickStream
            .Skip(1)
            .Take(1)
            .Select(x => Instantiate(_snakePrefab, position: _spawnPoint.position, rotation: Quaternion.identity))
            .SelectMany(headPrefab => headPrefab.OnTriggerEnterAsObservable()
                                                .Where(trigger => trigger.gameObject.GetComponent<Apple>() != null)
                                                .Select(x => Instantiate(_snakePrefab))
                                                .StartWith(headPrefab))
            .Scan(new GameObject[] { Instantiate(_snakePrefab) }, (acc, src) =>
            {
                return acc.Append(src).ToArray();
            })
            .Publish();

        tickStream
            .WithLatestFrom(inputStream, (tick, input) => input)
            .ValidationScan()
            .WithLatestFrom(snakeBodyGrowthStream, (input, snakeBody) => new { input, snakeBody })
            .Subscribe(x =>
            {
                var snakeBody = x.snakeBody;
                var input = x.input;
                for (int i = 1; i < snakeBody.Length; i++)
                {
                    snakeBody[i].transform.localPosition = snakeBody[i - 1].transform.localPosition;
                }
                snakeBody[0].transform.localPosition += input;
            })
            .AddTo(this);

        snakeBodyGrowthStream
            .Subscribe()
            .

        snakeBodyGrowthStream.Connect();
    }
}
