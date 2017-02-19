using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class TerrainGenIO 
{
    public void WriteHeightMap(ref float[,] heightmap, int terrainLength, string filePath, string logPath)
    {
        File.Delete(filePath);
        File.Delete(logPath);

        bool debug = false;
        string debugStr = "";

        StreamWriter writer = File.CreateText(logPath);

        if (writer.BaseStream == null)
        {
            Debug.LogError("Could not open file writer at location " + filePath);
            return;
        }

        if (heightmap.Length > 0)
        {
            Debug.Log("Write out bytes");
            ushort[,] shortMap = new ushort[terrainLength, terrainLength];
            for (int width = 0; width < terrainLength; ++width)
            {
                for (int height = 0; height < terrainLength; ++height)
                {
                    // keep around 2 decimal places
                    shortMap[width, height] = (ushort)(heightmap[width, height] * 100.0f);
                }


                if (debug)
                {
                    // print out one line at a time
                    for (int i = 0; i < terrainLength; ++i)
                    {
                        debugStr += heightmap[width, i] + ",";
                    }

                    Debug.Log(debugStr);
                }
            }


            byte[] byteArray = new byte[shortMap.Length * 2];
            Buffer.BlockCopy(shortMap, 0, byteArray, 0, byteArray.Length);
            File.WriteAllBytes(filePath, byteArray);
        }

        writer.Close();
    }

    public void LoadTerrainMap(int smoothPasses, float smoothConstant, int terrainLength, string fileName, string logPath)
    {
        string[] fileNameNoExtension = fileName.Split('.');
        TextAsset binaryData = Resources.Load(fileNameNoExtension[0]) as TextAsset;
        byte[] bytes = binaryData.bytes;

        float[,] heightmap = new float[terrainLength, terrainLength];
        int byteNum = 0;
        bool bDebug = false;
        string debugStr = "";

        // Currently making assumptions about the data
        // We assume the data is 16-bit integers (unsigned shorts)
        // Read in 2 bytes at a time, convert it to a ushort, then normalize it (0.0 - 1.0)
        // I know that I kept 2 decimal places (/100.0f) and my max value was 255

        for (int width = 0; width < terrainLength; ++width)
        {
            for (int height = 0; height < terrainLength; ++height)
            {
                ushort val = System.BitConverter.ToUInt16(bytes, byteNum);
                heightmap[width, height] = (val / 100.0f) / 255.0f;
                byteNum += 2;

                if (bDebug)
                {
                    debugStr += (heightmap[width, height]) + " ";
                }
            }

        }

        if (bDebug)
        {
            Debug.Log(debugStr);
        }

        // Data loaded, perform smoothing
        for (int i = 0; i < smoothPasses; ++i)
        {
            SmoothTerrain(ref heightmap, smoothConstant, terrainLength);
        }


        // Assign terrain to actual terrain
        TerrainData terrainData = GameObject.Find("Terrain").GetComponent<Terrain>().terrainData;
        terrainData.heightmapResolution = terrainLength;
        terrainData.size = new Vector3(terrainData.size.x, 254, terrainData.size.z);
        terrainData.SetHeights(0, 0, heightmap);

        Resources.UnloadAsset(binaryData);
    }

    private void SmoothTerrain(ref float[,] terrainPoints, float smoothConstant, int terrainLength )
    {
        // use the neighbors to make the terrain a little more level

        float result = 0.0f;
        // left to right
        for (int row = 1; row < terrainLength; ++row)
        {
            for (int col = 0; col < terrainLength - 1; ++col)
            {
                result = terrainPoints[row - 1, col] * (1.0f - smoothConstant) + terrainPoints[row, col] * smoothConstant;
                terrainPoints[row, col] = (result);
            }
        }

        // right to left
        for (int row = terrainLength - 2; row >= 0; row--)
        {
            for (int col = 0; col < terrainLength; ++col)
            {
                result = terrainPoints[row + 1, col] * (1.0f - smoothConstant) + terrainPoints[row, col] * smoothConstant;
                terrainPoints[row, col] = (result);
            }
        }

        // top to bottom
        for (int row = 0; row < terrainLength; ++row)
        {
            for (int col = 1; col < terrainLength; ++col)
            {
                result = terrainPoints[row, col - 1] * (1.0f - smoothConstant) + terrainPoints[row, col] * smoothConstant;
                terrainPoints[row, col] = (result);
            }
        }

        // bottom to top
        for (int row = 0; row < terrainLength; ++row)
        {
            for (int col = terrainLength - 2; col >= 0; col--)
            {
                result = terrainPoints[row, col + 1] * (1.0f - smoothConstant) + terrainPoints[row, col] * smoothConstant;
                terrainPoints[row, col] = (result);
            }
        }

    }
}
