using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicPearl : MonoBehaviour
{
    public SpriteRenderer sprite { get; private set; }

    private void Awake()
    {
        sprite = transform.Find("Front").GetComponent<SpriteRenderer>();
    }
}
