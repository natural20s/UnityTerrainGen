using UnityEngine;
using System.Collections;

public class DiamondSquareAlg {

    private float m_roughness = 0.75f;
    private float m_amplitude = 9.0f;

    private int m_maxPoints;
    private float[,] m_points;

    private const float MAX_HEIGHT = 254.0f;

    public void Generate(out float[,] points, int inMaxPoints, float inInitHeight, float inAmplitude, float inRoughness)
    {
        m_roughness = inRoughness;
        m_amplitude = inAmplitude;
        m_maxPoints = inMaxPoints;

        m_points = new float[m_maxPoints, m_maxPoints];
        for (int i = 0; i < m_maxPoints; ++i)
        {
            for (int j = 0; j < m_maxPoints; ++j)
            {
                m_points[i, j] = inInitHeight;
            }
        }

        // seed the corner height
        float cornerHeight = inInitHeight * 1.75f;
        m_points[0, 0] = cornerHeight;
        m_points[0, m_maxPoints - 1] = cornerHeight;
        m_points[m_maxPoints - 1, 0] = cornerHeight;
        m_points[m_maxPoints - 1, m_maxPoints - 1] = cornerHeight;

        
        int stride = (m_maxPoints - 1) / 2;

        while (stride > 0)
        {
            //DiamondStep
            for (int i = stride; i < m_maxPoints; i = i + stride * 2)
            {
                for (int j = stride; j < m_maxPoints; j = j + stride * 2)
                {
                    DiamondStep(i, j, stride);
                }
            }

            for (int i = 0; i * stride < m_maxPoints; ++i)
            {
                //SquareStep(i, stride, stride);
                int startStep = i % 2 == 0 ? stride : 0;
                //Debug.Log ("i = " + i + ", startStep = " + startStep);
                for (int j = startStep; j < m_maxPoints; j = j + stride * 2)
                {
                    SquareStep(i * stride, j, stride);
                }
            }

            stride /= 2;
            m_amplitude *= m_roughness;
        }

        points = m_points;
    }

    private void DiamondStep(int centerRow, int centerCol, int range)
    {
        // find average point
        float p1 = 0.0f, p2 = 0.0f, p3 = 0.0f, p4 = 0.0f;
        int count = 0;
        if (centerRow - range >= 0 && centerCol - range >= 0)
        {
            p1 = m_points[centerRow - range, centerCol - range];
            ++count;
        }
        if (centerCol + range < m_maxPoints)
        {
            p2 = m_points[centerRow - range, centerCol + range];
            ++count;
        }
        if (centerRow + range < m_maxPoints)
        {
            p3 = m_points[centerRow + range, centerCol - range];
            ++count;
        }
        if (centerRow + range < m_maxPoints && centerCol + range < m_maxPoints)
        {
            p4 = m_points[centerRow + range, centerCol + range];
            ++count;
        }

        /*
        p1 = m_points[centerRow - range, centerCol - range];
        p2 = m_points[centerRow - range, centerCol + range];
        p3 = m_points[centerRow + range, centerCol - range];
        p4 = m_points[centerRow + range, centerCol + range];
        */
        float avg = (p1 + p2 + p3 + p4) / count + Random.Range(-m_amplitude, m_amplitude);
        if (avg > MAX_HEIGHT || avg < 0.0f)
        {
            Debug.LogError("Diamond avg value not within byte range: " + avg);
        }
        Mathf.Clamp(avg, 0, MAX_HEIGHT);

        m_points[centerRow, centerCol] = avg;
        //Debug.Log("Value " + avg + " turned to byte value " + m_points[centerRow, centerCol]);
    }

    private void SquareStep(int rowStart, int colStart, int radius)
    {
        float p1 = 0.0f, p2 = 0.0f, p3 = 0.0f, p4 = 0.0f;
        int count = 0;
        if (rowStart - radius >= 0)
        {
            p1 = m_points[rowStart - radius, colStart];
            ++count;
        }

        if (colStart + radius < m_maxPoints)
        {
            p2 = m_points[rowStart, colStart + radius];
            ++count;
        }

        if (rowStart + radius < m_maxPoints)
        {
            p3 = m_points[rowStart + radius, colStart];
            ++count;
        }

        if (colStart - radius >= 0)
        {
            p4 = m_points[rowStart, colStart - radius];
            ++count;
        }

        float avg = (p1 + p2 + p3 + p4) / count + Random.Range(-m_amplitude, m_amplitude);
        if (avg > MAX_HEIGHT || avg < 0.0f)
        {
            Debug.LogError("Square avg value not within byte range: " + avg);
        }

        Mathf.Clamp(avg, 0, MAX_HEIGHT);
        m_points[rowStart, colStart] = avg;
        //Debug.Log("Value " + avg + " turned to byte value " + m_points[rowStart, colStart]);
    }
}
