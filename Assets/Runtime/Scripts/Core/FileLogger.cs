using System.Collections;
using System.IO;
using UnityEngine;

public class FileLogger : MonoBehaviour
{
    [SerializeField] private string path = "Assets/Resources/test.txt";

    public static FileLogger Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) {
            gameObject.SetActive(false);
            return;
        }
        Instance = this;
    }

    public void Write(string message, bool logToConsole = false)
    {
        if (logToConsole) Debug.Log(message);

        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(message);
        writer.Close();
    }
}
