using UnityEngine;
using System.IO.Ports;
using System.Threading;
using UnityEngine.UI;
using System;
using UnityCommunity.UnitySingleton;

[Serializable]
public class ArduinoMessage
{
    public int status;    // 狀態碼，例如 0 表示成功
    public string nfc;    // 偵測到的卡片編號或其他資料
    public string reader;
}

public class PN532Manager : MonoSingleton<PN532Manager>
{
    public string portName = "COM3";
    public int baudRate = 115200;

    public Button stopThreadButton;  // 💡 拖入你的「停止按鈕」
    public Text statusText;          // （選填）顯示目前狀態

    public string command = "R1";

    private SerialPort serialPort;
    private Thread readThread;
    private bool isRunning = false;
    private string latestMessage = "";
    public event Action<ArduinoMessage> onSuccessEvent;

    void Start()
    {
        if (stopThreadButton != null)
            stopThreadButton.onClick.AddListener(StopSerialThread);

        onSuccessEvent = null;

        ReadConfig();
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(latestMessage))
        {
            Debug.Log("📥 接收到原始資料: " + latestMessage);
            HandleArduinoMessage(latestMessage);
            latestMessage = "";
        }
        // if (Input.GetKeyDown(KeyCode.Alpha1)) SendCommand("R1");
        // if (Input.GetKeyDown(KeyCode.Alpha2)) SendCommand("R2");
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

    void HandleArduinoMessage(string message)
    {
        try
        {
            ArduinoMessage data = JsonUtility.FromJson<ArduinoMessage>(message);
            Debug.Log($"🔍 狀態碼: {data.status}, 標籤: {data.nfc}, 目前Reader: {data.reader}");
            if (data.reader == "0" && command != "R1")
            {
                SendCommand();
            }
            if (data.reader == "1" && command != "R2")
            {
                SendCommand();
            }
            OnArduinoCallback(data);
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ JSON 解析失敗: " + ex.Message);
        }
    }

    void OnArduinoCallback(ArduinoMessage data)
    {
        // ✅ 你可以在這裡依照狀態碼做不同邏輯
        // Debug.Log("✅ 處理 Callback 中的資料");

        if (data.status == 202)
        {
            if (string.IsNullOrEmpty(data.nfc))
            {
                return;
            }
            if (data.nfc.Length < 20)
            {
                Debug.Log("無效標籤");
                return;
            }
            // onSuccessEvent?.Invoke(); // 呼叫成功事件
            onSuccessEvent?.Invoke(data);
        }
        else
        {
            Debug.LogWarning("⚠️ 非成功狀態: " + data.status);
        }
    }

    public void SendCommand()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.WriteLine(command);
            Debug.Log("📤 傳送：" + command);
        }
    }

    public void ReadConfig()
    {
        DataManager.Instance.GetConfigData("ArduinoPortName", out portName);
        DataManager.Instance.GetConfigData("ArduinoBaudRate", out baudRate);
    }

    public void StartSerialThread(string commandLine = "", Action<ArduinoMessage> successEvent = null)
    {

        command = commandLine;
        if (isRunning) return;
        ReadConfig();
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 50;
            serialPort.Open();

            isRunning = true;
            readThread = new Thread(ReadSerialLoop);
            readThread.Start();
            Debug.Log("✅ Serial 執行緒啟動");
            if (statusText) statusText.text = "✅ Serial Running";

            onSuccessEvent = successEvent;
            Invoke("SendCommand", 1.0f);
        }
        catch
        {
            Debug.LogError("❌ Serial 初始化失敗");
            if (statusText) statusText.text = "❌ Serial Failed";
        }
    }

    public void StopSerialThread()
    {
        onSuccessEvent = null;
        if (!isRunning) return;

        Debug.Log("🛑 停止 Serial 執行緒...");
        isRunning = false;
        Thread.Sleep(100);

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
