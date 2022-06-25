using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    public static int GridSize = 4;
    
    private readonly Transform[,] tilePositions = new Transform[GridSize, GridSize];
    private readonly Tile[,] tiles = new Tile[GridSize, GridSize];
    [SerializeField] private Tile tilePrefab;

    // Start is called before the first frame update
    private void Start()
    {
        GetTilePositions();
        TrySpawnTile();
        TrySpawnTile();
        UpdateTilePosition();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void GetTilePositions()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        int x = 0;
        int y = 0;

        foreach (Transform transform in this.transform)
        {
            tilePositions[x, y] = transform;
            x++;
            
            if (x >= GridSize)
            {
                x = 0;
                y++;
            }
        }
    }

    private bool TrySpawnTile()
    {
        List<Vector2Int> availableSpots = new List<Vector2Int>();

        for (int x = 0; x < GridSize; x++)
        {
            for (int y = 0; y < GridSize; y++)
            {
                if (tiles[x, y] == null)
                {
                    availableSpots.Add(new Vector2Int(x, y));
                }
            }
        }

        if (!availableSpots.Any())
        {
            return false;
        }

        int randomIndex = Random.Range(0, availableSpots.Count);
        Vector2Int spot = availableSpots[randomIndex];

        var tile = Instantiate(tilePrefab, transform.parent);
        tile.SetValue(GetRandomValue());
        tiles[spot.x, spot.y] = tile;

        return true;
    }

    private int GetRandomValue()
    {
        var rand = Random.Range(0f, 1f);
        if (rand <= 0.8f)
        {
            return 2;
        }
        else
        {
            return 4;
        }
    }

    private void UpdateTilePosition()
    {
        for (int x = 0; x < GridSize; x++)
        {
            for (int y = 0; y < GridSize; y++)
            {
                if (tiles[x, y] != null)
                {
                    tiles[x, y].transform.position = tilePositions[x, y].position;
                }
            }
        }
    }
}