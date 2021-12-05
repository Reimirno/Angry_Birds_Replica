/***
TileMapExt.cs

Description: Unity Tilemap BoxFill is horrible. So why not self-write our own version?
This is modified from https://forum.unity.com/threads/tilemap-boxfill-is-horrible.502864/
I created some overloads to make things easier.
Author: Yu Long
Created: Monday, November 22 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
***/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Reimirno
{
    public static class TileMapExt
    {
        public static void m_BoxFill(this Tilemap map, TileBase tile, Vector3Int start, Vector3Int end)
        {
            //Determine directions on X and Y axis
            var xDir = start.x < end.x ? 1 : -1;
            var yDir = start.y < end.y ? 1 : -1;
            //How many tiles on each axis?
            int xCols = 1 + Mathf.Abs(start.x - end.x);
            int yCols = 1 + Mathf.Abs(start.y - end.y);
            //Start painting
            for (var x = 0; x < xCols; x++)
            {
                for (var y = 0; y < yCols; y++)
                {
                    var tilePos = start + new Vector3Int(x * xDir, y * yDir, 0);
                    map.SetTile(tilePos, tile);
                }
            }
        }

        public static void m_BoxFill(this Tilemap map, TileBase tile, int startX, int startY, int endX, int endY)
        {
            m_BoxFill(map, tile, new Vector3Int(startX, startY,0), new Vector3Int(endX, endY, 0));
        }

        
        public static void m_BoxFill(this Tilemap map, TileBase tile, Vector3 start, Vector3 end)
        {
            m_BoxFill(map, tile, map.WorldToCell(start), map.WorldToCell(end));
        }
    }
}

