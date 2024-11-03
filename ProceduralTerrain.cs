using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Untuk menggunakan TextMeshPro

public class ProceduralTerrain : MonoBehaviour
{
    public int width = 256;
    public int depth = 256;
    public float scale = 20f;
    public float height = 20f;

    public int octaves = 4; // Untuk variasi noise yang lebih kompleks
    public float persistence = 0.5f; // Seberapa besar pengaruh dari setiap octave
    public float lacunarity = 2f; // Skala frekuensi untuk setiap octave

    public TMP_InputField widthInputField; // InputField untuk width
    public TMP_InputField depthInputField; // InputField untuk depth
    public TMP_InputField scaleInputField; // InputField untuk scale
    public TMP_InputField heightInputField; // InputField untuk height

    private MeshFilter meshFilter;

    void Start()
    {
        // Tidak generate terrain langsung di Start()
        // GenerateTerrain();
    }

    // Method untuk tombol Generate
    public void OnGenerateButtonClicked()
    {
        // Ambil nilai dari input field dan ubah tipe datanya
        width = int.Parse(widthInputField.text);
        depth = int.Parse(depthInputField.text);
        scale = float.Parse(scaleInputField.text);
        height = float.Parse(heightInputField.text);

        // Pastikan mesh lama dibersihkan sebelum generate ulang
        if (meshFilter != null && meshFilter.mesh != null)
        {
            meshFilter.mesh.Clear();
        }

        // Generate terrain dengan parameter yang diperbarui
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        if (meshFilter == null)
        {
            meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }
        }

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        }

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(width + 1) * (depth + 1)];

        for (int i = 0, z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float y = GeneratePerlinNoise(x, z) * height;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        mesh.vertices = vertices;

        int[] triangles = new int[width * depth * 6];
        for (int z = 0, vert = 0, tris = 0; z < depth; z++, vert++)
        {
            for (int x = 0; x < width; x++, vert++, tris += 6)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    // Fungsi untuk menghasilkan noise dengan beberapa octave
    float GeneratePerlinNoise(int x, int z)
    {
        float total = 0f;
        float frequency = 1f;
        float amplitude = 1f;
        float maxValue = 0f; // Digunakan untuk normalisasi nilai

        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * scale * 0.1f * frequency, z * scale * 0.1f * frequency) * amplitude;

            maxValue += amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return total / maxValue; // Normalisasi hasil noise
    }
}
