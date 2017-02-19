using UnityEngine;
using System.Collections;
using System;

public class FaultAlg 
{
    private int m_iterations = 250;
    private float m_maxDisplacement = 6.0f;
    private float m_displacement = 0;

    private int m_maxPoints;
    private float[,] m_points;

    private const float MAX_HEIGHT = 254.0f;

    public void Generate(out float[,] byteArray, int inMaxPoints, float inInitHeight, int inIterations, float inMaxDisplacement)
    {
        m_iterations = inIterations;
        m_maxDisplacement = inMaxDisplacement;
        m_maxPoints = inMaxPoints;

        m_points = new float[m_maxPoints, m_maxPoints];

        for (int i = 0; i < m_maxPoints; ++i)
        {
            for (int j = 0; j < m_maxPoints; ++j)
            {
                m_points[i, j] = inInitHeight;
            }
        }

        // we know we're using a square, so our max distance is our num points
        float dist = m_maxPoints;

        for (int i = 0; i < m_iterations; ++i)
        {
            SetDisplacement(i);

            // calculate a random line across plane that does not intersect center
            // ax + bz - c = 0
            // a = sin(v), b = cos(v)
            float c = UnityEngine.Random.Range(0.0f, 1.0f) * dist - (dist / 2.0f);
             
            float v = UnityEngine.Random.Range(0, Mathf.PI * 2);
            float a = Mathf.Sin(v);
            float b = Mathf.Cos(v);

            // Adjust terrain
            for (int row = 0; row < m_maxPoints; ++row)
            {
                float y = row -(m_maxPoints * 0.5f);
                for (int col = 0; col < m_maxPoints; ++col)
                {
                    float x = col -(m_maxPoints * 0.5f);
                    if (a * x + b * y - c > 0)
                    {
                        RaiseTerrain(col, row);
                    }
                    else
                    {
                        LowerTerrain(col, row);
                    }
                }
            }   
        }

        byteArray = m_points;
    }

    private void RaiseTerrain(int x, int y)
    {
        if (m_points[x, y] + m_displacement < MAX_HEIGHT)
        {
            m_points[x, y] += m_displacement;
        }
    }

    private void LowerTerrain(int x, int y)
    {
        if (m_points[x, y] > m_displacement)
        {
            m_points[x, y] -= m_displacement;
        }
        else
        {
            m_points[x, y] = 0;
        }
    }

    private void SetDisplacement(int curIteration)
    {
        // dampen our displacement as iterations continue to make the terrain less erratic

        m_displacement = (byte)(m_maxDisplacement - m_maxDisplacement * (curIteration / ((float)m_iterations)));

        if (m_displacement < 1)
            m_displacement = 1;
    }
}
