using TMPro;
using UnityEngine;

public class SheepCatcher : MonoBehaviour
{
    [SerializeField] private TMP_Text numberOfSheepCaughtText;
    [SerializeField] private string sheepTag = "Sheep";

    private int _caughtSheepCount;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag(sheepTag)) return;

        _caughtSheepCount++;
        numberOfSheepCaughtText.text = _caughtSheepCount.ToString();

        OnSheepCaught(collision.gameObject);
    }

    private static void OnSheepCaught(GameObject sheep)
    {
        Destroy(sheep);
    }
}