using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using UnityEngine;
using TMPro;

public class SerialConnectionManager : MonoBehaviour
{
    public TMP_Dropdown    TMPPortsDropdown;
    public TMP_Dropdown    TMPBaudDropdown;
    public GameObject    ConnectButton;
    public GameObject    DisconnectButton;

    public GameObject HomeButton;
    public TMP_Text SerialText;
    public TMP_InputField StepValue;
    public TMP_InputField J1Value;
    public TMP_InputField J2Value;
    public TMP_InputField J3Value;
    public TMP_InputField J4Value;
    public TMP_InputField J5Value;
    public TMP_InputField J6Value;

    bool connectedPressed = false;

     
    int[] angle = {90,90}; //update for more servos

    SerialController SerialControllerScript;

    enum BUTTON
    {
        CONNECT = 0,
        DISCONNECT = 1,
        HOME = 2
    }

    void LogSerialN(string msg)//log with newline
    {
        LogSerial(msg+="\n");
    }
    void LogSerial(string msg)
    {
        SerialText.text +=msg;
    }

    void ClearSerialLog()
    {
        SerialText.text = "";
    }

    void ButtonVisible(BUTTON button, bool visible)
    {
         
        switch(button)
        {
            case (BUTTON.CONNECT):
            {
                ConnectButton.SetActive(visible);
            }break;
            case (BUTTON.DISCONNECT):
            {
                DisconnectButton.SetActive(visible);
            }break;
            case (BUTTON.HOME):
            {
                HomeButton.SetActive(visible);
            }break;
        }
        

    }
    
    void WriteTextFile()
    {
        string path = "Assets/Resources/test.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("Test");
        writer.Close();
    }

    void ReadTextFile()
    {
        string path = "Assets/Resources/test.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path); 
        string line = "";
        
        while ((line = reader.ReadLine()) != null)
        {
            Debug.Log("Line " + line);
            string[] playerScore = line.Split('|');
            Debug.Log("Name is " + playerScore[0]);
            Debug.Log("Score is " + playerScore[1]);
        }
        
        //Debug.Log("Read " + reader.ReadToEnd());
        reader.Close();
    }

    void SetJointValues()
    {
        //ad more servos
        J1Value.text = ""+angle[0];
        J2Value.text = ""+angle[1];
        

        StepValue.text = "20";
    }

    public void onDisconnectPressed()
    {
        ButtonVisible(BUTTON.CONNECT,true);
        ButtonVisible(BUTTON.DISCONNECT,false);
        ButtonVisible(BUTTON.HOME,false);
        SerialControllerScript.Disconnect();
        connectedPressed = false;
        LogSerialN("Diconnected");
    }

    public void onConnectPressed()
    {
        string port = TMPPortsDropdown.options[TMPPortsDropdown.value].text;
        string baud = TMPBaudDropdown.options[TMPBaudDropdown.value].text;

        if(baud=="")
        {
            Debug.Log("Baud is blank, using default 115200");
        }

        LogSerialN("Conecting on Port " + port + " baud " + baud);
        int reconnectionDelay = 1000;
        int maxUnreadMessages = 10;

        SerialControllerScript.portName = port;
        SerialControllerScript.baudRate = int.Parse(baud);
        SerialControllerScript.reconnectionDelay = reconnectionDelay;
        SerialControllerScript.maxUnreadMessages = maxUnreadMessages;
        SerialControllerScript.Connect();

        connectedPressed = true;

    }

    public void onHomePressed()
    {
        string serialMsg = "0:90&1:90";
        SerialControllerScript.SendSerialMessage(serialMsg);

        for(int i=0;i<angle.Length;i++)
        {
            angle[i] = 90;
        }

        SetJointValues();
    }


    void ReadSerial()
    {
         string message = SerialControllerScript.ReadSerialMessage();

        if (message == null)
            return;

        // Check if the message is plain data or a connect/disconnect event.
        if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_CONNECTED))
        {
            LogSerialN("Connection established");
            ButtonVisible(BUTTON.CONNECT,false);
            ButtonVisible(BUTTON.DISCONNECT,true);
            ButtonVisible(BUTTON.HOME,true);
            SetJointValues();
        }
            
        else if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_DISCONNECTED))
            LogSerialN("Connection attempt failed or disconnection detected");
        else
            LogSerialN(message);
    }     

    void InitDropdowns()
    {
        string[] ports = SerialPort.GetPortNames();

            
        TMPPortsDropdown.options.Clear();
        foreach(string port in ports)
        {
            TMPPortsDropdown.options.Add (new TMP_Dropdown.OptionData() {text=port});
                
        }

        TMPBaudDropdown.options.Add (new TMP_Dropdown.OptionData() {text="115200"});
        TMPBaudDropdown.options.Add (new TMP_Dropdown.OptionData() {text="9600"});

    }

    void SendSerial(string value)
    {
        SerialControllerScript.SendSerialMessage(value);
    }

    public void onJogPressed(string strParams)
    {
        string [] values = strParams.Split('|');
        LogSerialN("jogging " + values[0] + " direction " + values[1]);

        string joint = values[0];
        string direction = values[1];

        int index = int.Parse(joint);
        int currentAngle = angle[index];
        int step = int.Parse(StepValue.text);
        
        //step = 1;//remove

        if(direction == "left" || direction == "down" )
        {
            currentAngle -= step;
        }
        else if(direction == "right" || direction == "up" )
        {
            currentAngle += step;
        }
        string serialMsg = joint + ":" + currentAngle;
        SerialControllerScript.SendSerialMessage(serialMsg);

        if(joint == "0")
        {
            J1Value.text = ""+currentAngle;
        }
        else if(joint == "1")
        {
            J2Value.text = ""+currentAngle;
        }

        angle[index] = currentAngle;


    }

    // Start is called before the first frame update
    void Start()
    {
         
        ClearSerialLog();
        InitDropdowns();
        SerialControllerScript = GetComponent<SerialController>();

    }

    // Update is called once per frame
    void Update()
    {
        if(connectedPressed == true)
            ReadSerial();
    }
}
