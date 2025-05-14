using UnityEngine;
using System.Collections.Generic;

public class DropSprites : MonoBehaviour
{
    public float dropSpeed = 5f;
    public float dropDistance = 10f;

    private List<Transform> droppableSprites = new List<Transform>();
    private Dictionary<Transform, float> initialYPositions = new Dictionary<Transform, float>();
    private bool shouldDrop = false;

    void Start()
    {
        GameObject[] sprites = GameObject.FindGameObjectsWithTag("Droppable");
        foreach (GameObject sprite in sprites)
        {
            Transform t = sprite.transform;
            droppableSprites.Add(t);
            initialYPositions[t] = t.position.y;
        }
    }

    void Update()
    {
        if (shouldDrop)
        {
            bool allDone = true;

            foreach (Transform t in droppableSprites)
            {
                float distanceDropped = initialYPositions[t] - t.position.y;

                if (distanceDropped < dropDistance)
                {
                    t.Translate(Vector3.down * dropSpeed * Time.deltaTime);
                    allDone = false;
                }
            }

            // Stop dropping when all have completed their drop
            if (allDone)
            {
                shouldDrop = false;
            }
        }
    }

    public void TriggerDrop()
    {
        Debug.Log("trigger drop");
        shouldDrop = true;
    }
}