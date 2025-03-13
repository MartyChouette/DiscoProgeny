using System.Collections;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public Rect bounds; 
    public float moveInterval = 3f; 

    private void Start()
    {
        StartCoroutine(MoveRandomly());
    }

    private IEnumerator MoveRandomly()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveInterval);

            float randomX = Random.Range(bounds.xMin, bounds.xMax);
            float randomY = Random.Range(bounds.yMin, bounds.yMax);
            Vector3 randomPosition = new Vector3(randomX, randomY, transform.position.z);

            transform.position = randomPosition;
        }
    }
}
