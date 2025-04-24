using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutHelper : MonoBehaviour
{
    [SerializeField] int columns;
    [SerializeField] Vector2 startPos, cellSize;
    public void LayoutCards()
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            int row = i / columns;
            int col = i % columns;
            Vector2 pos = startPos + new Vector2(col * cellSize.x, row * cellSize.y);
            transform.GetChild(i).position = pos;
        }
    }
}
