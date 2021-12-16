using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.TableUI;
using TMPro;

public class SequenceController : MonoBehaviour
{

    public TableUI table;
    public int startWait = 1;

    int currentLineNumber = 1;

    int currentRow = 1;

    const int LINE_NUMBER = 0;
    const int ACTION = 1;
    const int P1 = 2;
    const int P2 = 3;
    const int P3 = 4;
    const int P4 = 5;
    const int P5 = 6;
    const int P6 = 7;

    enum ACTION_LIST //MUST MATCH THE ORDER IN THE EDITOR OPTIONS LIST
    {
        WAIT =0 , //index must be same in the editor options for action list
        PLACE_LABEL =1,
        GOTO_LABEL =2,
        NEW_LABEL = 3,
        REPEAT = 4,
        GRIPPER_OPEN = 5,
        GRIPPER_CLOSE = 6 
    }

    enum OPTION_INPUTS
    {
        DROPDOWN,
        INPUT
    }

    enum SEQUENCER_MODE //MUST MATCH THE ORDER IN THE EDITOR OPTIONS LIST
    {
        ADD = 0, //index must be same in the editor options for mode list
        EDIT = 1,
        DELETE = 2
    }
 


    public TMP_Dropdown ActionList;
    public TMP_Dropdown OptionList;
    public TMP_InputField OptionInput;
    public TMP_Dropdown LineList;
    public TMP_Dropdown ModeList;

    

    List<string> LabelList = new List<string>();
    
    

    void AddLineNumberList(int number)
    {
         LineList.options.Add (new TMP_Dropdown.OptionData() {text=""+number});
    }


    public void onModeChange()
    {
        SEQUENCER_MODE mode = (SEQUENCER_MODE)ModeList.value;

        switch(mode)
        {
            case SEQUENCER_MODE.ADD:
            {
                ActionList.gameObject.SetActive(true);
                onChangeActionList(); //set the options
                LineList.gameObject.SetActive(false);
            }break;
            case SEQUENCER_MODE.DELETE:
            {
                ActionList.gameObject.SetActive(false);
                 
                LineList.gameObject.SetActive(true);
                LineList.gameObject.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = "Line Number";
                LineList.RefreshShownValue ();
                SetOptionVisible(OPTION_INPUTS.DROPDOWN,false);
                SetOptionVisible(OPTION_INPUTS.INPUT, false);
            }break;
            case SEQUENCER_MODE.EDIT:
            {
                ActionList.gameObject.SetActive(true);
                onChangeActionList(); //set the options
                LineList.gameObject.SetActive(true);
                LineList.gameObject.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = "Line Number";
                LineList.RefreshShownValue ();
                
            }break;
        }


    }

    public void onChangeActionList()
    {
        int selectedIndex = ActionList.value;
        string selectedOption = ActionList.options[selectedIndex].text;
        Debug.Log("Selected option " + selectedOption);

        PopulateActionOptions((ACTION_LIST)ActionList.value);
    }

      public void onOptionsListChange()
    {
        //SetOptionPromptText(OptionList.options[OptionList.value].text);
         OptionList.RefreshShownValue ();
    }



    public void onPressedAddToSequence()
    {
        int selectedIndex = ActionList.value;
        string selectedOption = ActionList.options[selectedIndex].text;
        Debug.Log("Selected option " + selectedOption);

        AddToSequence((ACTION_LIST)ActionList.value);
    }

    void AddToSequence(ACTION_LIST action)
    {
         switch (action)
        {
            case ACTION_LIST.NEW_LABEL:
            {
                AddSequenceLabel();
            }break;
            case ACTION_LIST.GOTO_LABEL:
            {
                int selectedIndex = OptionList.value;
                string label = OptionList.options[selectedIndex].text;
                SetRow("GOTO",label);
            }break;
            case ACTION_LIST.REPEAT:
            {
                int selectedIndex = OptionList.value;
                string label = OptionList.options[selectedIndex].text;
                string repeatTimes = OptionInput.text;
                SetRow("REPEAT",label,repeatTimes);
            }break;
            case ACTION_LIST.PLACE_LABEL:
            {
                int selectedIndex = OptionList.value;
                string label = OptionList.options[selectedIndex].text;
                SetRow("LABEL",label);
            }break;
            case ACTION_LIST.WAIT:
            {
                 string time = OptionInput.text;
                 SetRow("WAIT", time);
                 
            }break;
            case ACTION_LIST.GRIPPER_OPEN:
            {
                 
                 SetRow("GRIPPER OPEN");
                 
            }break;
            case ACTION_LIST.GRIPPER_CLOSE:
            {
                 
                 SetRow("GRIPPER CLOSE");
                 
            }break;
        }

    }

    //called directly from serial control
    public void AddSequenceMove(float J1,float J2,float J3,float J4,float J5,float J6 )
    {
        Debug.Log("Adding Move ");
    }

    void AddSequenceLabel()
    {
       string labelName = OptionInput.text;

       if(!LabelList.Contains(labelName))
       {
           LabelList.Add(labelName);
           OptionList.options.Add (new TMP_Dropdown.OptionData() {text=labelName});
           OptionList.RefreshShownValue ();
       }
    }

    

    

    void SetOptionVisible(OPTION_INPUTS option, bool visibilty)
    {
        if(option == OPTION_INPUTS.INPUT)
        {
            OptionInput.gameObject.SetActive(visibilty);
        }
        else if(option == OPTION_INPUTS.DROPDOWN)
        {
            OptionList.gameObject.SetActive(visibilty);
            OptionList.RefreshShownValue ();
        }
         
    }

    public void SetOptionPromptText(string text)
    {
        OptionList.gameObject.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = text;
    }

    void SetInputPromptText(string text)
    {
        OptionInput.text = "";
        OptionInput.gameObject.transform.GetChild(0).Find("Placeholder").GetComponent<TextMeshProUGUI>().text = text;
        
    }

    void PopulateActionOptions(ACTION_LIST action)
    {

        switch (action)
        {
            case ACTION_LIST.NEW_LABEL:
            {
                SetOptionVisible(OPTION_INPUTS.DROPDOWN,false);
                SetOptionVisible(OPTION_INPUTS.INPUT,true);
                SetInputPromptText("Label Name");
            }break;
            case ACTION_LIST.PLACE_LABEL:
            {
                SetOptionVisible(OPTION_INPUTS.DROPDOWN,true);
                SetOptionVisible(OPTION_INPUTS.INPUT,false);
                SetOptionPromptText(OptionList.options[0].text );

                 
                 
                 
            }break;
            case ACTION_LIST.GOTO_LABEL:
            {
                SetOptionVisible(OPTION_INPUTS.DROPDOWN,true);
                SetOptionVisible(OPTION_INPUTS.INPUT,false);
            }break;
            case ACTION_LIST.GRIPPER_CLOSE:
            {
                SetOptionVisible(OPTION_INPUTS.DROPDOWN,false);
                SetOptionVisible(OPTION_INPUTS.INPUT,false);
            }break;
            case ACTION_LIST.GRIPPER_OPEN:
            {
                SetOptionVisible(OPTION_INPUTS.DROPDOWN,false);
                SetOptionVisible(OPTION_INPUTS.INPUT,false);
            }break;
            case ACTION_LIST.REPEAT:
            {
                SetOptionVisible(OPTION_INPUTS.DROPDOWN,true);
                SetOptionVisible(OPTION_INPUTS.INPUT,true);
                
                 
                
                
                SetOptionPromptText("Repeat to label");

                OptionList.RefreshShownValue ();

                SetInputPromptText("Number of repeats");
            }break;
            case ACTION_LIST.WAIT:
            {
                SetOptionVisible(OPTION_INPUTS.DROPDOWN,false);
                SetOptionVisible(OPTION_INPUTS.INPUT,true);
                SetInputPromptText("Wait Seconds");
            }break;

        }
    }

  


    public void SetRow(string action,string p1="",string p2="",string p3="",string p4="",string p5="",string p6="")
    {
        AddLineNumberList(currentLineNumber);
        table.GetCell(currentRow,LINE_NUMBER).text =""+currentLineNumber;
        table.GetCell(currentRow,ACTION).text =action;
        table.GetCell(currentRow,P1).text =p1;
        table.GetCell(currentRow,P2).text =p2;
        table.GetCell(currentRow,P3).text =p3;
        table.GetCell(currentRow,P4).text =p4;
        table.GetCell(currentRow,P5).text =p5;
        table.GetCell(currentRow,P6).text =p6;
        currentRow++;
        table.Rows++;
        currentLineNumber++;
        
    
    }
    public void InitTable()
    {
        SetRow("WAIT","1");
         
        onChangeActionList(); //populate the action list
        //ActionList.gameObject.SetActive(true);
        //onChangeActionList(); //set the options
        LineList.gameObject.SetActive(false);

    }

    // Start is called before the first frame update
    void Start()
    {
        InitTable();
        LabelList.Clear();
        OptionList.ClearOptions();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
