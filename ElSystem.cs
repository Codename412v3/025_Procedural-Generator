using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Include the TMP namespace for TMP_InputField

public class LSystemGypsophila3D : MonoBehaviour
{
    private string axiom = "X"; 
    private Dictionary<char, string> rules = new Dictionary<char, string>();
    private string currentString;

    // Parameter yang dapat diubah
    public int iterations = 5;
    public float angle = 25.0f;
    public float length = 0.5f;

    // TMP Input Fields
    public TMP_InputField iterationsInputField;
    public TMP_InputField angleInputField;
    public TMP_InputField lengthInputField;

    // Prefab references
    public GameObject stemPrefab;
    public GameObject leafPrefab;
    public GameObject stamenPrefab;
    private List<GameObject> createdPrefabs = new List<GameObject>();

    // Spawn Position
    private Vector3 spawnPosition = new Vector3(921.7f, 352.7f, 220.8f);

    void Start()
    {
        rules.Clear();
        rules.Add('X', "F[?L][+XZ]F[-XZ][^XZ][&XZ][>XZ][<XZ]");
        rules.Add('F', "FF[+L][-L]");
        rules.Add('Y', "F[?S][+L][-L]");
        rules.Add('Z', "F[+L][-L]F[^S][&S]");
        
        transform.position = spawnPosition; // Set the spawn position initially
    }

    public void OnGenerateButtonPressed()
    {
        // Parse input field values, with default fallback values
        if (int.TryParse(iterationsInputField.text, out int inputIterations))
        {
            iterations = inputIterations;
        }

        if (float.TryParse(angleInputField.text, out float inputAngle))
        {
            angle = inputAngle;
        }

        if (float.TryParse(lengthInputField.text, out float inputLength))
        {
            length = inputLength;
        }

        RegenerateLSystem(); // Call regeneration L-System
    }

    void Update()
    {
        // Mengubah sudut (angle) dengan tombol panah kiri dan kanan
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            angle -= 1f; // Kurangi sudut
            RegenerateLSystem();
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            angle += 1f; // Tambah sudut
            RegenerateLSystem();
        }

        // Mengubah panjang cabang (length) dengan tombol panah atas dan bawah
        if (Input.GetKey(KeyCode.UpArrow))
        {   
            if (length < 1f)
            {
                length += 0.01f; // Tambah panjang
                RegenerateLSystem();
            }
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (length > 0.1f)
            {
                length -= 0.01f; // Kurangi panjang
                RegenerateLSystem();
            }
        }

        // Membatasi panjang cabang agar tidak negatif
        length = Mathf.Max(0.1f, length);

        // Mengubah jumlah iterasi dengan tombol '+' dan '-'
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            if (iterations < 6)
            {
                iterations++;
                RegenerateLSystem();
            }
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            if (iterations > 1)
            {
                iterations--;
                RegenerateLSystem();
            }
        }
    }

    void RegenerateLSystem()
    {
        ClearCreatedPrefabs(); // Hapus prefab yang ada sebelumnya
        GenerateLSystem(); // Regenerasi string L-System baru
        transform.position = spawnPosition; // Reset ke posisi spawn yang telah ditentukan
        DrawLSystem(); // Mulai menggambar kembali
    }

    void ClearCreatedPrefabs()
    {
        foreach (var prefab in createdPrefabs)
        {
            Destroy(prefab);
        }
        createdPrefabs.Clear(); // Kosongkan daftar prefab
    }

    void GenerateLSystem()
    {
        currentString = axiom;
        for (int i = 0; i < iterations; i++)
        {
            currentString = ApplyRules(currentString);
        }
    }

    string ApplyRules(string input)
    {
        string output = "";
        foreach (char c in input)
        {
            output += rules.ContainsKey(c) ? rules[c] : c.ToString();
        }
        return output;
    }

    void DrawLSystem()
    {
        Stack<TransformInfo> transformStack = new Stack<TransformInfo>();
        foreach (char c in currentString)
        {
            switch (c)
            {
                case 'F': CreateStem(); break;
                case 'L': CreateLeaf(); break;
                case 'S': CreateStamen(); break;
                case '+': Rotate(Vector3.forward, angle); break;
                case '-': Rotate(Vector3.forward, -angle); break;
                case '^': Rotate(Vector3.right, angle); break;
                case '&': Rotate(Vector3.right, -angle); break;
                case '>': Rotate(new Vector3(-0.5f, 0, 0), angle); break;
                case '<': Rotate(new Vector3(-0.5f, 0, 0), -angle); break;
                case ',': Rotate(new Vector3(0, 0, -0.5f), angle); break;
                case '.': Rotate(new Vector3(0, 0, -0.5f), -angle); break;
                case '?': RotateRandom(); break;
                case '[': SaveTransform(transformStack); break;
                case ']': RestoreTransform(transformStack); break;
            }
        }
    }

    void CreateStem()
    {
        GameObject stem = Instantiate(stemPrefab, transform.position, transform.rotation);
        createdPrefabs.Add(stem);
        transform.Translate(Vector3.up * length);
    }

    void CreateLeaf()
    {
        GameObject leaf = Instantiate(leafPrefab, transform.position, transform.rotation);
        createdPrefabs.Add(leaf);
        transform.Translate(Vector3.up * length);
        Rotate(new Vector3(Random.Range(-0.5f, 0), 0, Random.Range(-0.5f, 0)), angle);
    }

    void CreateStamen()
    {
        GameObject stamen = Instantiate(stamenPrefab, transform.position, transform.rotation);
        createdPrefabs.Add(stamen);
        transform.Translate(Vector3.up * length);
    }

    void Rotate(Vector3 axis, float angle)
    {
        transform.Rotate(axis * angle);
    }

    void RotateRandom()
    {
        float randomX = Random.Range(-0.5f, 0.5f);
        float randomZ = Random.Range(-0.5f, 0.5f);
        Rotate(new Vector3(randomX, 0, randomZ), -angle);
    }

    void SaveTransform(Stack<TransformInfo> transformStack)
    {
        transformStack.Push(new TransformInfo()
        {
            position = transform.position,
            rotation = transform.rotation
        });
    }

    void RestoreTransform(Stack<TransformInfo> transformStack)
    {
        if (transformStack.Count > 0)
        {
            var ti = transformStack.Pop();
            transform.position = ti.position;
            transform.rotation = ti.rotation;
        }
    }

    private struct TransformInfo
    {
        public Vector3 position;
        public Quaternion rotation;
    }
}
