using UnityEngine;
using System;
using System.IO.Ports;
using System.Threading;

public class ArduinoTest : MonoBehaviour
{
    public string portName = "COM3";
    public int baudRate = 9600;
    Parity parity = Parity.None;
    int dataBits = 8;
    StopBits stopBits = StopBits.One;

    SerialPort sp = null;

    public int isClosedPort = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OpenPort();
    }

    public void OpenPort()
    {
        sp = new SerialPort(portName,baudRate,parity,dataBits,stopBits);
        try{
            sp.Open();
        }
        catch(Exception e){
            Debug.LogError("Error opening serial port: " + e.Message);
        }
    }

    public void ClosePort()
    {
        try{
            sp.Close();
        }
        catch(Exception e){
            Debug.LogError("Error closing serial port: " + e.Message);
        }
    }

    void ReadData()
    {
        if(sp.IsOpen)
        {
            string a = sp.ReadExisting();
            Debug.LogWarning(a);
            Thread.Sleep(500);
            // try{
            //     string str = sp.ReadLine();
            //     Debug.Log(str);
            // }
            // catch(Exception e){
            //     Debug.LogError("Error reading serial port: " + e.Message);
            // }
        }
        // if(sp!=null && sp.IsOpen)
        // {
        //     try{
        //         string str = sp.ReadLine();
        //         Debug.Log(str);
        //     }
        //     catch(Exception e){
        //         Debug.LogError("Error reading serial port: " + e.Message);
        //     }
        // }
    }
    // Update is called once per frame
    void Update()
    {
        ReadData();
    }
}
