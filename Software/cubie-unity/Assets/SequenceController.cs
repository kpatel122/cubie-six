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
        GRIPPER_CLOSE = 6,   
    }

    enum OPTION_INPUTS
    {
        DROPDOWN,
        INPUT
    }
 


    public TMP_Dropdown ActionList;
    public TMP_Dropdown OptionList;

    public TMP_InputField OptionInput;

    List<string> LabelList = new List<string>();
    
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
