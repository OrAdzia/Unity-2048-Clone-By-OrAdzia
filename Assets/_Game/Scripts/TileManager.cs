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
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        TryMove(Mathf.RoundToInt(xInput), Mathf.RoundToInt(yInput));
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

    private void TryMove(int x, int y)
    {
        if (x == 0 && y == 0)
        {
            return;
        }

        if (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)
        {
            Debug.Log($"Invalid Move {x}, {y}");
            return;
        }

        if (x == 0)
        {
            if (y > 0)
            {
                TryMoveUp();
            }
            else
            {
                TryMoveDown();
            }
        }
        else
        {
            if (x < 0)
            {
                TryMoveLeft();
            }
            else
            {
                TryMoveRight();
            }
        }

        UpdateTilePosition();
    }

    private void TryMoveRight()
    {
        for (int y = 0; y < GridSize; y++)
        {
            for (int x = GridSize - 1; x >= 0; x--)
            {
                if (tiles[x, y] == null) continue;

                for (int x2 = GridSize - 1; x2 > x; x2--)
                {
                    if (tiles[x2, y] != null) continue;

                    tiles[x2, y] = tiles[x, y];
                    tiles[x, y] = null;
                    break;
                }
            }
        }
    }

    private void TryMoveLeft()
    {
        for (int y = 0; y < GridSize; y++)
        {
            for (int x = 0; x < GridSize; x++)
            {
                if (tiles[x, y] == null) continue;

                for (int x2 = 0; x2 < x; x2++)
                {
                    if (tiles[x2, y] != null) continue;

                    tiles[x2, y] = tiles[x, y];
                    tiles[x, y] = null;
                    break;
                }
            }
        }
    }

    private void TryMoveDown()
    {
        for (int x = 0; x < GridSize; x++)
        {
            for (int y = GridSize - 1; y >= 0; y--)
            {
                if (tiles[x, y] == null) continue;

                for (int y2 = GridSize - 1; y2 > y; y2--)
                {
                    if (tiles[x, y2] != null) continue;

                    tiles[x, y2] = tiles[x, y];
                    tiles[x, y] = null;
                    break;
                }
            }
        }
    }

    private void TryMoveUp()
    {
        for (int x = 0; x < GridSize; x++)
        {
            for (int y = 0; y < GridSize; y++)
            {
                if (tiles[x, y] == null) continue;

                for (int y2 = 0; y2 < y; y2++)
                {
                    if (tiles[x, y2] != null) continue;

                    tiles[x, y2] = tiles[x, y];
                    tiles[x, y] = null;
                    break;
                }
            }
        }
    }
}