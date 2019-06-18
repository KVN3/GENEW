using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using UnityEngine;

public class Accelerometer : MonoBehaviour
{
    private SerialPort serialPort;

    // Delimiters used to parse the received string data
    private string[] stringDelimiters = new string[] { ":", "R", };

    // Last received acceleration
    private Vector3 lastAccData = Vector3.zero;

    // How many values the total weight (1) is divided amongst.
    private int valueCounter = 10;

    // The weight (how much a received value matters at the moment) of a value. Sum of these values = 1.
    private readonly float[] valueWeight = new float[] { 0.5f, 0.25f, 0.125f, 0.0625f, 0.03125f, 0.015625f, 0.0078125f, 0.00390625f, 0.001953125f,
                                        0.0009765625f, 0.00048828125f };
    // Current values
    private Vector3[] values = new Vector3[10];

    private void Awake()
    {
        // Try to find the serial port through which the accelerometer communicates
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
            // Do nothing
        }
    }

    void Update()
    {
        // Debug
        //if (Input.GetKeyDown(KeyCode.Escape) && serialPort.IsOpen)
        //    serialPort.Close();
    }

    private void FixedUpdate()
    {
        // Request and parse data
        string cmd = RequestData();
        Vector3 value = ParseAccelerometerData(cmd);

        // Update the values array
        if (value != Vector3.zero)
            values[valueCounter++ % 10] = value;
    }

    // Request data change from Arduino
    private string RequestData()
    {
        try
        {
            // Read data and check size
            string inData = serialPort.ReadLine();
            int inSize = inData.Count();

            // Debug
            //if (inSize > 0)
            //    Debug.Log($"Arduino data received: ({inData})! Message size: ({inSize.ToString()})");

            // Got the data. Flush and discard to speed up reads.
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

    // String data to Vector3
    private Vector3 ParseAccelerometerData(string data)
    {
        try
        {
            // Split the string into an array of strings based on given delimiters
            string[] splitResult = data.Split(stringDelimiters, StringSplitOptions.RemoveEmptyEntries);

            // Get the x, y and z values from the array
            int x = int.Parse(splitResult[0]);
            int y = int.Parse(splitResult[1]);
            int z = int.Parse(splitResult[2]);

            // Set this as the last received acceleration data and return it
            lastAccData = new Vector3(x, y, z);
            return lastAccData;
        }
        catch
        {
            // Return no movement
            return Vector3.zero;
        }
    }

    // Get Vector3 acceleration value based on all values in array and their respective weight
    public Vector3 GetAcceleration()
    {
        Vector3 value = new Vector3();
        int count = 0;

        for (int i = -1; i >= -10; i--)
        {
            // Add the values in the array to the total value, after applying weight
            value += values[(valueCounter - i) % 10] * valueWeight[count++];
        }

        return value;
    }

    // Turn 0-360 degree data to -1 to +1 input data used by the PC script. 180 deg = 0 input data
    public float ParseAccelerationToInput(float singleAxisAcceleration, Direction axis)
    {
        float rad = singleAxisAcceleration * (2 * Mathf.PI) / 360;
        float input = Mathf.Sin(rad);

        if (axis == Direction.X)
        {
            // Round up/down
            if (input >= -0.2f && input <= 0.2f)
                input = 0f;

            // Half the input
            input /= 2;
        }
        else if (axis == Direction.Y)
        {
            // Round up/down
            if (input >= -0.2f && input <= 0.2f)
                input = 0f;
        }

        // Round up/down
        if (input > 0.9f)
            input = 1;
        else if (input < -0.9f)
            input = -1;

        // Debug
        //if (axis == Direction.X)
        //    Debug.Log("INPUT " + Direction.X + ": " + input);

        return input;
    }
}
