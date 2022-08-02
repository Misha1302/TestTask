using UnityEngine;

public class SheepSpawner : MonoBehaviour
{
    [Header("Sheep")]
    [SerializeField] private SheepMover sheepPrefab;
    [SerializeField] private Transform sheepParent;
    
    [Header("Sheep Settings")]
    [SerializeField] private PlayerMover player;
    [SerializeField] private Transform[] escapePointsOnHorror;
    
    [Header("Other Settings")]
    [SerializeField] private int sheepCount = 21;
    [SerializeField] private float spawnY = 0.75f;
    [SerializeField] private Vector2 fieldSize = new(50, 50);
    [SerializeField] private Vector2 offsetField;

    private void Start()
    {
        var halfFieldWight = fieldSize.x / 2 + offsetField.x;
        var halfFieldHeight = fieldSize.y / 2 + offsetField.y;
        
        for (var i = 0; i < sheepCount; i++)
        {
            var spawnVector3 = new Vector3
            {
                x = Random.Range(-halfFieldWight, halfFieldWight),
                y = spawnY,
                z = Random.Range(-halfFieldHeight, halfFieldHeight)
            };
            
            var newSheep = Instantiate(sheepPrefab, spawnVector3, Quaternion.identity, sheepParent);
            newSheep.SetPlayer(player);
            newSheep.SetEscapePointsOnHorror(escapePointsOnHorror);
        }
    }
}