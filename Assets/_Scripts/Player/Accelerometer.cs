using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using UnityEngine;

public class Accelerometer : MonoBehaviour
{
    SerialPort serialPort;
    string[] stringDelimiters = new string[] { ":", "R", };

    private Vector3 lastAccData = Vector3.zero;

    void Start()
    {
        try
        {
            serialPort = new SerialPort("COM3", 115200, Parity.None, 8, StopBits.One)
            {
                DtrEnable = false,
                ReadTimeout = 1,
                WriteTimeout = 1
            };
            serialPort.Open();
        }
        catch
        {
            Debug.Log("Couldn't find COM3, use regular controls...");
        }
    }

    void Update()
    {
        // TO DO: REMOVE (FOR DEVELOPMENT)
        if (Input.GetKeyDown(KeyCode.Escape) && serialPort.IsOpen)
            serialPort.Close();
    }

    // String data to Vector3
    public Vector3 ParseAccelerometerData(string data)
    {
        try
        {
            string[] splitResult = data.Split(stringDelimiters, StringSplitOptions.RemoveEmptyEntries);
            int x = int.Parse(splitResult[0]);
            int y = int.Parse(splitResult[1]);
            int z = int.Parse(splitResult[2]);
            lastAccData = new Vector3(x, y, z);
            return lastAccData;
        }
        catch
        {
            //Debug.Log("Serial transmission is malformed.");
            return Vector3.zero;
        }
    }

    // Request data change from Arduino
    public string CheckForReceivedData()
    {
        try
        {
            string inData = serialPort.ReadLine();
            int inSize = inData.Count();
            if (inSize > 0)
            {
                //Debug.Log($"Arduino data received: ({inData})! Message size: ({inSize.ToString()})");
            }

            //Got the data. Flush the in-buffer to speed reads up.
            inSize = 0;
            serialPort.BaseStream.Flush();
            serialPort.DiscardInBuffer();
            return inData;
        }
        catch
        {
            return string.Empty;
        }
    }

    // Brr
    int valueCounter = 10;
    private Vector3[] values = new Vector3[10];

    readonly float[] valueWeight = new float[] { 0.5f, 0.25f, 0.125f, 0.0625f, 0.03125f, 0.015625f, 0.0078125f, 0.00390625f, 0.001953125f,
                                        0.0009765625f, 0.00048828125f }; // som van waarden is 1

    void FixedUpdate()
    {
        string cmd = CheckForReceivedData();
        Vector3 value = ParseAccelerometerData(cmd);
        if (value != Vector3.zero)
            values[valueCounter++ % 10] = value;
    }

    public Vector3 GetValue()
    {
        Vector3 value = new Vector3();
        int count = 0;

        for (int i = -1; i >= -10; i--)
        {
            value += values[(valueCounter - i) % 10] * valueWeight[count++];
        }

        return value;
    }
    // Brr

    // Turn 0-360 degree data to -1 to +1 input data ... 180 deg = 0
    public float ParseAccelerationToInput(float singleAxisAcceleration, Direction axis)
    {
        float rad = singleAxisAcceleration * (2 * Mathf.PI) / 360;
        float input = Mathf.Sin(rad);

        if (axis == Direction.X)
        {
            if (input >= -0.2f && input <= 0.2f)
                input = 0f;

            input = input / 2;
        }
        else if (axis == Direction.Y)
        {
            if (input >= -0.2f && input <= 0.2f)
                input = 0f;
        }

        if (input > 0.9f)
            input = 1;
        else if (input < -0.9f)
            input = -1;

        //else
        //{
        //    input = Mathf.Round(input * 10) / 10;
        //    //Debug.Log("rounded:" + input);
        //}



        if (axis == Direction.X)
            Debug.Log("INPUT " + Direction.X + ": "+ input);
        return input;
    }
}
