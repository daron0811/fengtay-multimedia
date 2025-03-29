using UnityEngine;
using System.IO.Ports;

public class NFCWriter : MonoBehaviour
{
    public string portName = "COM3";  // 你的 Arduino 連接的序列埠
    public int baudRate = 115200;

    private SerialPort serialPort;

    void Start()
    {
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))  // 按 "W" 鍵寫入 NFC
        {
            WriteNFC("Hello from Unity!");
        }
    }

    void WriteNFC(string message)
    {
        if (serialPort.IsOpen)
        {
            serialPort.WriteLine(message);  // 傳送 NDEF 訊息給 Arduino
            Debug.Log("已傳送: " + message);
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort == null)
        {
            return;
        }
        if (serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
