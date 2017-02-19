using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

public class TerrainGenWindow : EditorWindow {

    // Layout values
    const string GenOptions = "General Options";
    int m_terrainLength = 129;

    // Diamond Square
    float m_initHeightDS = 20.0f;
    float m_amplitudeDS = 11.0f;
    float m_roughnessDS = 0.75f;

    // Fault
    float m_initHeightFault = 15.0f;
    int m_iterationsFault = 250;
    float m_maxDisplacementFault = 6.0f;


    int m_smoothPasses = 1;
    float m_smoothConstant = 0.7f;


    // Non editable values
    private float[,] m_heightmap = new float[0, 0];
    string m_filePath = "D:/Development/Unity/TerrainGenSandbox/Assets/Resources/";
    string m_fileName = "terrainmap.bytes";
    string logPath = "D:/Development/Unity/TerrainGenSandbox/Assets/DebugOutput/outputlog.txt";
    

    private DiamondSquareAlg dsAlgorithm = new DiamondSquareAlg();
    private FaultAlg faultAlg = new FaultAlg();
    private TerrainGenIO terrainIO = new TerrainGenIO();

    [MenuItem("TerrainGen/Generation Options")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TerrainGenWindow));
    }

    void OnGUI()
    {
        GUILayout.Label(GenOptions, EditorStyles.boldLabel);

        m_terrainLength = EditorGUILayout.IntField("Terrain Length (2^x + 1)", m_terrainLength);
        m_filePath = EditorGUILayout.TextField("Resources dir path", m_filePath);
        m_fileName = EditorGUILayout.TextField("File name", m_fileName);
        

        EditorGUILayout.Space();

        GUILayout.Label("Diamond Square Algorithm", EditorStyles.boldLabel);
        m_initHeightDS = EditorGUILayout.FloatField("Initial height", m_initHeightDS);
        m_amplitudeDS = EditorGUILayout.FloatField("Amplitude", m_amplitudeDS);
        m_roughnessDS = EditorGUILayout.FloatField("Roughness", m_roughnessDS);
        if (GUILayout.Button("Generate using Diamond Square"))
        {
            dsAlgorithm.Generate(out m_heightmap, m_terrainLength, m_initHeightDS, m_amplitudeDS, m_roughnessDS);
            terrainIO.WriteHeightMap(ref m_heightmap, m_terrainLength, m_filePath + m_fileName, logPath);
        }

        EditorGUILayout.Space();

        GUILayout.Label("Fault Algorithm", EditorStyles.boldLabel);
        m_initHeightFault = EditorGUILayout.FloatField("Initial  height", m_initHeightFault);
        m_iterationsFault = EditorGUILayout.IntField("Num Interations", m_iterationsFault);
        m_maxDisplacementFault = EditorGUILayout.FloatField("Max Displacement", m_maxDisplacementFault);
        if (GUILayout.Button("Generate using Fault"))
        {
            faultAlg.Generate(out m_heightmap, m_terrainLength, m_initHeightFault, m_iterationsFault, m_maxDisplacementFault);
            terrainIO.WriteHeightMap(ref m_heightmap, m_terrainLength, m_filePath + m_fileName, logPath);
        }


        EditorGUILayout.Space();

        GUILayout.Label("Loading/Display Options", EditorStyles.boldLabel);
        m_smoothPasses = EditorGUILayout.IntField("Smoothing Passes", m_smoothPasses);
        m_smoothConstant = EditorGUILayout.Slider(m_smoothConstant, 0.0f, 1.0f);


        EditorGUILayout.Space();
        if (GUILayout.Button("Load Terrain"))
        {
            terrainIO.LoadTerrainMap(m_smoothPasses, m_smoothConstant, m_terrainLength, m_fileName, logPath);
        }
    }
}
