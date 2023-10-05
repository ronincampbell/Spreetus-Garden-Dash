using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGen : MonoBehaviour
{
    public GameObject OutsideWallPrefab;
    public GameObject OutsideCornerPrefab;
    public GameObject InsideWallPrefab;
    public GameObject InsideCornerPrefab;
    public GameObject EmptyPrefab;
    public GameObject EmptyPelletPrefab;
    public GameObject EmptyPowerPrefab;
    public GameObject JunctionPrefab;

    public float tileSize = 1.0f;

    int[,] levelMap =
        {
            {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
            {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
            {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
            {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
            {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
            {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
            {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
            {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
            {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
            {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
            {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
            {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
            {0,0,0,0,0,2,5,4,4,0,3,4,4,0},
            {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
            {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
        };

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        int rows = levelMap.GetLength(0);
        int cols = levelMap.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                GenerateTile(x, y);
            }
        }

        MirrorLevel(rows, cols);
        AdjustCamera(rows, cols);
    }


    // Tile generation
    void GenerateTile(int x, int y){
        int tileType = levelMap[y, x];
        Vector2 spawnPos = new Vector2(x, y);

        GameObject spawnedTile = null;

        if (tileType == 0) {
            GameObject Empty = Instantiate(EmptyPrefab, spawnPos, Quaternion.identity);
        } else if (tileType == 1) {
            GameObject OutsideCorner = Instantiate(OutsideCornerPrefab, spawnPos, Quaternion.identity);
        }
        else if (tileType == 2){
            spawnedTile = Instantiate(OutsideWallPrefab, spawnPos, Quaternion.identity);
            if (ShouldRotateOutsideWall(x, y))
            {
                spawnedTile.transform.Rotate(0, 0, 90);
            }
        } else if (tileType == 3) {
            GameObject InsideCorner = Instantiate(InsideCornerPrefab, spawnPos, Quaternion.identity);
        } else if (tileType == 4) {
            GameObject InsideWall = Instantiate(InsideWallPrefab, spawnPos, Quaternion.identity);
        } else if (tileType == 5) {
            GameObject EmptyPellet = Instantiate(EmptyPelletPrefab, spawnPos, Quaternion.identity);
        } else if (tileType == 6) {
            GameObject EmptyPower = Instantiate(EmptyPowerPrefab, spawnPos, Quaternion.identity);
        } else if (tileType == 7) {
            GameObject Junction = Instantiate(JunctionPrefab, spawnPos, Quaternion.identity);
        }
    }

    void MirrorLevel(int rows, int cols)
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 position = new Vector3(x * tileSize, (rows - y - 1) * tileSize + rows * tileSize, 0);
                GameObject tile = Instantiate(GetTilePrefab(levelMap[y, x]), position, Quaternion.identity);
                ApplyRotationForMirror(tile, x, y, "1stQuadrant");
            }
        }

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 position = new Vector3((cols - x - 1) * tileSize + cols * tileSize, (rows - y - 1) * tileSize + rows * tileSize, 0);
                GameObject tile = Instantiate(GetTilePrefab(levelMap[y, x]), position, Quaternion.identity);
                ApplyRotationForMirror(tile, x, y, "2ndQuadrant");
            }
        }

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 position = new Vector3((cols - x - 1) * tileSize + cols * tileSize, y * tileSize, 0);
                GameObject tile = Instantiate(GetTilePrefab(levelMap[y, x]), position, Quaternion.identity);
                ApplyRotationForMirror(tile, x, y, "4thQuadrant");
            }
        }
    }

    void ApplyRotationForMirror(GameObject tile, int x, int y, string quadrant)
    {
        if (levelMap[y, x] == 2 && ShouldRotateOutsideWall(x, y))
        {
                tile.transform.Rotate(0, 0, 90);
        }
    }


    // Returns the correct tile prefab based on the tile type
    GameObject GetTilePrefab(int tileType) {
        if (tileType == 0) return EmptyPrefab;
        if (tileType == 1) return OutsideCornerPrefab;
        if (tileType == 2) return OutsideWallPrefab;
        if (tileType == 3) return InsideCornerPrefab;
        if (tileType == 4) return InsideWallPrefab;
        if (tileType == 5) return EmptyPelletPrefab;
        if (tileType == 6) return EmptyPowerPrefab;
        if (tileType == 7) return JunctionPrefab;
        return null;
    }

    void AdjustCamera(int rows, int cols)
    {
        // Get the width and height of map
        float mapWidth = cols * tileSize * 2;
        float mapHeight = rows * tileSize * 2;

        Camera cam = Camera.main;

        // Set cam size
        cam.orthographicSize = mapHeight / 2;

        // Centering camera
        cam.transform.position = new Vector3(mapWidth / 2, (mapHeight - tileSize * 1f) / 2, -10f);
    }

    bool ShouldRotateOutsideWall(int x, int y)
    {
        // Check for top
        bool hasTopNeighbor = y + 1 < levelMap.GetLength(0) && (levelMap[y + 1, x] == 1 || levelMap[y + 1, x] == 2);

        // Check for bottom
        bool hasBottomNeighbor = y - 1 >= 0 && (levelMap[y - 1, x] == 1 || levelMap[y - 1, x] == 2);

        // Neither
        return !(hasTopNeighbor || hasBottomNeighbor);
    }

    
}
