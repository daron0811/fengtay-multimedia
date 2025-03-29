using UnityEngine;
using System.IO.Ports;
using System.Threading;
using UnityEngine.UI;

public class PN532ManagerThreaded : MonoBehaviour
{
    public string portName = "COM3";
    public int baudRate = 115200;

    public Button stopThreadButton;  // 💡 拖入你的「停止按鈕」
    public Text statusText;          // （選填）顯示目前狀態

    private SerialPort serialPort;
    private Thread readThread;
    private bool isRunning = false;
    private string latestMessage = "";

    void Start()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 50;
            serialPort.Open();

            isRunning = true;
            readThread = new Thread(ReadSerialLoop);
            readThread.Start();

            Debug.Log("✅ Serial 執行緒啟動");
            statusText.text = "✅ Serial Running";

            // 綁定按鈕事件
            if (stopThreadButton != null)
                stopThreadButton.onClick.AddListener(StopSerialThread);
        }
        catch
        {
            Debug.LogError("❌ Serial 初始化失敗");
            if (statusText) statusText.text = "❌ Serial Failed";
        }
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(latestMessage))
        {
            Debug.Log("📥 " + latestMessage);
            latestMessage = "";
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) SendCommand("R1");
        if (Input.GetKeyDown(KeyCode.Alpha2)) SendCommand("R2");
    }

    void ReadSerialLoop()
    {
        while (isRunning && serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string line = serialPort.ReadLine();
                latestMessage = line;
            }
            catch (System.TimeoutException)
            {
                // 忽略
            }
        }
    }

    void SendCommand(string command)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.WriteLine(command);
            Debug.Log("📤 傳送：" + command);
        }
    }

    public void StopSerialThread()
    {
        if (!isRunning) return;

        Debug.Log("🛑 停止 Serial 執行緒...");
        isRunning = false;
        Thread.Sleep(100); // 給執行緒時間結束

        if (readThread != null && readThread.IsAlive)
            readThread.Join();

        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();

        if (statusText) statusText.text = "🛑 Serial Stopped";
    }

    void OnApplicationQuit()
    {
        StopSerialThread();
    }
}
