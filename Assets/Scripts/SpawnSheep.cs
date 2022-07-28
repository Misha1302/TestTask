using UnityEngine;

public class SpawnSheep : MonoBehaviour
{
    [SerializeField] private Transform sheepParent;
    [SerializeField] private Transform sheepPrefab;
    [SerializeField] private int sheepCount = 21;
    [SerializeField] private float spawnY = 0.75f;
    [SerializeField] private Vector2 fieldSize = new(50, 50);

    private float halfFieldWight;
    private float halfFieldHeight;
    
    private void Start()
    {
        halfFieldWight = fieldSize.x / 2;
        halfFieldHeight = fieldSize.y / 2;
        for (var i = 0; i < sheepCount; i++)
        {
            var spawnVector3 = new Vector3
            {
                x = Random.Range(-halfFieldWight, halfFieldWight),
                y = spawnY,
                z = Random.Range(-halfFieldHeight, halfFieldHeight)
            };
            Instantiate(sheepPrefab, sheepParent).position = spawnVector3;
        }
    }
}