using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

[ExecuteInEditMode]
public class GridMap : MonoBehaviour
{
    public MapData_SO mapData;
    public GridType gridType;
    private Tilemap currentTilemap;

    private void OnEnable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();
            if (mapData != null)
                mapData.tileProperites.Clear();
        }
    }

    private void OnDisable()
    {
        if (!Application.IsPlaying(this))
        {
            currentTilemap = GetComponent<Tilemap>();
            UpdateTileProperties();

#if UNITY_EDITOR
            if (mapData != null)
                EditorUtility.SetDirty(mapData);
#endif
        }
    }

    private void UpdateTileProperties()
    {
        currentTilemap.CompressBounds();
        if (!Application.IsPlaying(this))
        {
            if (mapData != null)
            {
                Vector3Int startPos = currentTilemap.cellBounds.min;
                Vector3Int endPos = currentTilemap.cellBounds.max;

                for (int i = startPos.x; i < endPos.x; i++)
                {
                    for (int j = startPos.y; j < endPos.y; j++)
                    {
                        TileBase tile = currentTilemap.GetTile(new Vector3Int(i, j, 0));
                        if (tile != null)
                        {
                            TileProperty newTile = new TileProperty
                            {
                                tileCoordinate = new Vector2Int(i, j),
                                gridType = this.gridType,
                                boolTypeValue = true
                            };

                            mapData.tileProperites.Add(newTile);
                        }
                    }
                }
            }
        }
    }
}
