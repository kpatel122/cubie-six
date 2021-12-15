using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using UnityEngine;
using TMPro;

public class SerialControl : MonoBehaviour
{
    public TMP_Dropdown    TMPPortsDropdown;
    public TMP_Text SerialText;

    
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


    // Start is called before the first frame update
    void Start()
    {
         
        string[] ports = SerialPort.GetPortNames();

            
        TMPPortsDropdown.options.Clear();

        SerialText.text = "hello";

        //WriteTextFile();
        ReadTextFile();
 
        // Display each port name to the console.
        foreach(string port in ports)
        {
            TMPPortsDropdown.options.Add (new TMP_Dropdown.OptionData() {text=port});
                
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
