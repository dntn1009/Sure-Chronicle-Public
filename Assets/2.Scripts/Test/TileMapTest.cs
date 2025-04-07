using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapTest : MonoBehaviour
{
    [SerializeField] Tilemap _tile;

    // Start is called before the first frame update
    void Start()
    {
        _tile.RefreshAllTiles();
    }
}
