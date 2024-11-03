using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Referensi untuk panel-panel yang ada
    public GameObject mainScreen;            // Panel utama
    public GameObject terrainGeneratorUI;     // Panel untuk Terrain Generator
    public GameObject lSystemUI;              // Panel untuk L-System
    public GameObject backButtonUI;           // Panel untuk tombol kembali ke main screen

    void Start()
    {
        // Pastikan saat game dimulai hanya mainScreen yang aktif
        ShowMainScreen();
    }

    // Method untuk menampilkan MainScreen dan menonaktifkan panel lain
    public void ShowMainScreen()
    {
        mainScreen.SetActive(true);
        terrainGeneratorUI.SetActive(false);
        lSystemUI.SetActive(false);
        backButtonUI.SetActive(false);
    }

    // Method untuk menampilkan UI Terrain Generator dan menonaktifkan MainScreen
    public void ShowTerrainGeneratorUI()
    {
        mainScreen.SetActive(false);
        terrainGeneratorUI.SetActive(true);
        lSystemUI.SetActive(false);
        backButtonUI.SetActive(true); // Tampilkan tombol atau panel untuk kembali ke main screen
    }

    // Method untuk menampilkan UI L-System dan menonaktifkan MainScreen
    public void ShowLSystemUI()
    {
        mainScreen.SetActive(false);
        terrainGeneratorUI.SetActive(false);
        lSystemUI.SetActive(true);
        backButtonUI.SetActive(true); // Tampilkan tombol atau panel untuk kembali ke main screen
    }

    // Method untuk kembali ke MainScreen dari tombol Back
    public void BackToMainScreen()
    {
        ShowMainScreen();
    }
}
