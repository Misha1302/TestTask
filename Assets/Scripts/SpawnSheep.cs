using UnityEngine;

public class SpawnSheep : MonoBehaviour
{
    [SerializeField] private Transform sheepParent;
    [SerializeField] private Transform sheepPrefab;
    [SerializeField] private int sheepCount = 21;
    [SerializeField] private float spawnY = 0.75f;
    [SerializeField] private Vector2 fieldSize = new(50, 50);

    private float _halfFieldWight;
    private float _halfFieldHeight;
    
    private void Start()
    {
        _halfFieldWight = fieldSize.x / 2;
        _halfFieldHeight = fieldSize.y / 2;
        for (var i = 0; i < sheepCount; i++)
        {
            var spawnVector3 = new Vector3
            {
                x = Random.Range(-_halfFieldWight, _halfFieldWight),
                y = spawnY,
                z = Random.Range(-_halfFieldHeight, _halfFieldHeight)
            };
            Instantiate(sheepPrefab, sheepParent).position = spawnVector3;
        }
    }
}