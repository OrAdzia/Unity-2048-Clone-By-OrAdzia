using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    public static int GridSize = 4;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private TileSettings tileSettings;

    private readonly Transform[,] tilePositions = new Transform[GridSize, GridSize];
    private readonly Tile[,] tiles = new Tile[GridSize, GridSize];
    private bool isAnimating;
    private bool tilesUpdated;
    private int lastXInput, lastYInput;

    // Start is called before the first frame update
    private void Start()
    {
        GetTilePositions();
        TrySpawnTile();
        TrySpawnTile();
        UpdateTilePosition(true);
    }

    // Update is called once per frame
    private void Update()
    {
        var xInput = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        var yInput = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

        if (lastXInput == 0 && lastYInput == 0)
        {
            if (!isAnimating)
            {
                TryMove(xInput, yInput);
            }
        }

        lastXInput = xInput;
        lastYInput = yInput;
    }

    public void RestartGame()
    {
        var activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
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

    private void UpdateTilePosition(bool instant)
    {
        if (!instant)
        {
            isAnimating = true;
            StartCoroutine(WaitForTileAnimation());
        }

        for (int x = 0; x < GridSize; x++)
        {
            for (int y = 0; y < GridSize; y++)
            {
                if (tiles[x, y] != null)
                {
                    tiles[x, y].SetPosition(tilePositions[x, y].position, instant);
                }
            }
        }
    }

    private IEnumerator WaitForTileAnimation()
    {
        yield return new WaitForSeconds(tileSettings.animationTime);

        if (!TrySpawnTile())
        {
            Debug.LogError("UNABLE TO SPAWN TILE!");
        }

        UpdateTilePosition(true);
        isAnimating = false;
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

        tilesUpdated = false;

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

        if (tilesUpdated)
        {
            UpdateTilePosition(false);
        }
    }

    private bool TileExistsBetween(int x, int y, int x2, int y2)
    {
        if (x == x2)
        {
            return TileExistsBetweenVertical(x, y, y2);
        }
        else if (y == y2)
        {
            return TileExistsBetweenHorizontal(x, x2, y);
        }

        Debug.LogError($"BETWEEN CHECK - INVALID PARAMETERS ({x}, {y}) ({x2}, {y2})");

        return true;
    }

    private bool TileExistsBetweenHorizontal(int x, int x2, int y)
    {
        int minX = Mathf.Min(x, x2);
        int maxX = Mathf.Max(x, x2);

        for (int xIndex = minX + 1; xIndex < maxX; xIndex++)
        {
            if (tiles[xIndex, y] != null)
            {
                return true;
            }
        }

        return false;
    }

    private bool TileExistsBetweenVertical(int x, int y, int y2)
    {
        int minY = Mathf.Min(y, y2);
        int maxY = Mathf.Max(y, y2);

        for (int yIndex = minY + 1; yIndex < maxY; yIndex++)
        {
            if (tiles[x, yIndex] != null)
            {
                return true;
            }
        }

        return false;
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
                    if (tiles[x2, y] != null) 
                    { 
                        if (TileExistsBetween(x, y, x2, y))
                        {
                            continue;
                        }

                        if (tiles[x2, y].Merge(tiles[x, y]))
                        {
                            tiles[x, y] = null;
                            tilesUpdated = true;
                            break;
                        }
                        continue;
                    }

                    tilesUpdated = true;
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
                    if (tiles[x2, y] != null) 
                    {
                        if (TileExistsBetween(x, y, x2, y))
                        {
                            continue;
                        }

                        if (tiles[x2, y].Merge(tiles[x, y]))
                        {
                            tiles[x, y] = null;
                            tilesUpdated = true;
                            break;
                        }
                        continue;
                    }

                    tilesUpdated = true;
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
                    if (tiles[x, y2] != null) 
                    {
                        if (TileExistsBetween(x, y, x, y2))
                        {
                            continue;
                        }

                        if (tiles[x, y2].Merge(tiles[x, y]))
                        {
                            tiles[x, y] = null;
                            tilesUpdated = true;
                            break;
                        }
                        continue;
                    }

                    tilesUpdated = true;
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
                    if (tiles[x, y2] != null) 
                    {
                        if (TileExistsBetween(x, y, x, y2))
                        {
                            continue;
                        }
                        
                        if (tiles[x, y2].Merge(tiles[x, y]))
                        {
                            tiles[x, y] = null;
                            tilesUpdated = true;
                            break;
                        }
                        continue;
                    }

                    tilesUpdated = true;
                    tiles[x, y2] = tiles[x, y];
                    tiles[x, y] = null;
                    break;
                }
            }
        }
    }
}