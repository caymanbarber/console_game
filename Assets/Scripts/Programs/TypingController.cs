using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine.XR;
using System;

public class TypingController : Program   //state in state machine
{
    Canvas canvas;
    [SerializeField]
    String userName = "User";
    [SerializeField]
    String commandPrefix = ">>";
    public TMP_Text text;
    public TMP_Text cursorText;
    Cursor cursor;
    [SerializeField]
    int lineNums = 19;
    List<Line> lines;

    int maxLineLength = 38;
    // TODO: change maxLineLength depending on font and space
    Line currentLine;
    int cursorPosition;
    List<Line> lineHistory;
    int historyPosition;
    int maxHistoryLength = 16;
    bool firstLineCopied = false;
    CommandController commandController;
    public bool isActive = false;

    private GameObject gameObject;

    public TypingController(ComputerMonitor computerMonitor):base(computerMonitor){
        gameObject = computerMonitor.gameObject;
        canvas = gameObject.GetComponent<Canvas>();
        
        text = gameObject.transform.Find("Text").gameObject.GetComponent<TMP_Text>();
        cursorText = gameObject.transform.Find("Cursor").gameObject.GetComponent<TMP_Text>();
        
        cursorPosition = 0;
        historyPosition = 0;
        lineHistory = new List<Line>();
        lines = new List<Line>();
        currentLine = new Line("", true, userName + " " + commandPrefix + " ");
        lines.Add(currentLine);
        cursor = new Cursor(cursorText);
        cursor.UpdateCursor(currentLine.GetPrefixLength(),cursorPosition);
        commandController = new CommandController(stateController, this);
    }

    public override void OnStateEnter() {
        isActive = true;
        text.gameObject.SetActive(true);
        cursorText.gameObject.SetActive(true);
        //add backgrounds
        //Unhide and draw letters
    }

    public override void OnStateExit() {
        isActive = false;
        text.gameObject.SetActive(false);
        cursorText.gameObject.SetActive(false);
        //remove backgorund
        //hide letters and clear?
    }

    public void ClearLines() {
        Line newLine = new Line("", true, userName + " " + commandPrefix + " ");
        currentLine = newLine;
        lines = new List<Line>();
    }

    public override void OnUpdate() {
        if(!isActive)
            return;

        if(HandleKeys()) //up, down, left, right
            return;

        if(HandleDelete())
            return;

        if(HandleTab()) //Auto complete
            return;

        if(HandleEnter())
            return;

        if(ParseInput())
            return;


        return;
    }

    private bool HandleKeys() {
        
        // handle navigatio from keys
        // left and right change cursor
        // up and down change history
        if(Input.GetKeyDown(KeyCode.LeftArrow)) {
             if(cursorPosition>0) {
                cursorPosition--;
                cursor.UpdateCursor(currentLine.GetPrefixLength(),cursorPosition);
             }
        } else if(Input.GetKeyDown(KeyCode.RightArrow)){
             if(cursorPosition<maxLineLength && cursorPosition<currentLine.content.Length) {
                cursorPosition++;
                cursor.UpdateCursor(currentLine.GetPrefixLength(),cursorPosition);
             }
        } else if(Input.GetKeyDown(KeyCode.DownArrow)) {
            if(historyPosition>0) {
                
                currentLine = (Line)lineHistory[--historyPosition].Clone();
                cursorPosition = currentLine.content.Length;
                //(Line)currentLine.Clone()
                UpdateTextLine();
            }
        } else if(Input.GetKeyDown(KeyCode.UpArrow)){
            if(historyPosition<lineHistory.Count-1 || (lineHistory.Count == 1 && historyPosition == 0)) {
                if(historyPosition == 0 && !firstLineCopied) {
                    firstLineCopied = true;
                    lineHistory.Insert(0,(Line)currentLine.Clone());
                }
                currentLine = (Line)lineHistory[++historyPosition].Clone();
                cursorPosition = currentLine.content.Length;
                UpdateTextLine();
            } else if (lineHistory.Count == 1 && historyPosition == 0){
                
            }
        } else {
            
            return false;
        }
        //Debug.Log("Handle Keys");
        return true;
    }

    private bool HandleDelete() {

        if(!Input.GetKeyDown(KeyCode.Backspace)) {
            
            return false;
        }
            
        RemoveAtCursorPosition();

        return true; 
    }

    private bool HandleTab() {
        if(!Input.GetKeyDown(KeyCode.Tab)) {
            
            return false;
        }
        Debug.Log("Handle Tab");
        // TODO: add autocomplete trie
            
        return true; 
    }

    private bool HandleEnter () {
        if(!Input.GetKeyDown(KeyCode.Return)) {
            return false;
        }
        
        // Create new empty line
        Line newLine = new Line("", true, userName + " " + commandPrefix + " ");

        //Insert copy of current line into lines to be displayed
        lines[0] = (Line)currentLine.Clone();


        //Add output 
        List<String> currentLineList = lines[0].content.Split(' ').ToList();
        List<Line> output = new List<Line>();
        commandController.ParseCommand(currentLineList[0], currentLineList.GetRange(1,currentLineList.Count-1) ,output);

        if (output.Count > 0) {
            lines.InsertRange(0, output.Select(line => (Line)line.Clone()));
        }

        lines.Insert(0, newLine);
        
        //If the first line was copied into the history, remove it 
        if(firstLineCopied) {
            lineHistory.RemoveAt(0);
            firstLineCopied = false;
        }

        //Insert a copy of the current line into the line history
        lineHistory.Insert(0,(Line)currentLine.Clone());

        historyPosition = 0; 

        //Make the current line the empty line previously created
        
        
        currentLine = newLine;
        cursorPosition = 0;

        //Remove last line in line history if too long
        if(lineHistory.Count > maxHistoryLength)
            lineHistory.RemoveAt(lineHistory.Count-1);
            
        cursor.UpdateCursor(currentLine.GetPrefixLength(),cursorPosition);

        return true; 
    }

    private bool ParseInput() {
        if (currentLine.GetPrefixLength() + currentLine.content.Length >= maxLineLength) {
            return false;
        }

        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            //Debug.Log(keyCode.ToString());
            if(!isShiftPressed && keyDict.ContainsKey(keyCode) && Input.GetKeyDown(keyCode)) {
                InsertAtCursorPosition(keyDict[keyCode]);
                break;
            } else if(isShiftPressed && keyDict.ContainsKey(keyCode) && Input.GetKeyDown(keyCode)) {
                InsertAtCursorPosition(shiftDict[keyDict[keyCode]]);
                break;
            }
        }

        UpdateTextLine();

        return true;
    }

    private void InsertAtCursorPosition(String character) {
        if(cursorPosition<maxLineLength) {
            currentLine.Insert(cursorPosition++, character);
            cursor.UpdateCursor(currentLine.GetPrefixLength(),cursorPosition);
        }
    }

    private void RemoveAtCursorPosition() {
        if(cursorPosition > 0) {
            currentLine.Remove(--cursorPosition);
            cursor.UpdateCursor(currentLine.GetPrefixLength(),cursorPosition);
        }
    }

    private void UpdateTextLine() {
        //update current line
        lines[0] = currentLine;
        WriteLines();
    }

    private void LoadLines() {
        int lineNum = 0;
        this.lines = new List<Line>();
        // split text into list of Strings
        String[] lines = Regex.Split(text.text, "\r\n|\r|\n");

        //loop throguh lines until we reach max amout of lines
        while(lineNum < lineNums) {
            // start at last line and go up. 
            if(lines.Length - lineNum < 1)
                return;
            this.lines.Add(new Line(lines[lines.Length - lineNum++ - 1], true, userName + " " + commandPrefix + " ")); 
        }
    }

    private void WriteLines() {
        StringBuilder myStringBuilder = new StringBuilder();

        for(int i = this.lines.Count-1; i>=0; i--) {
            myStringBuilder.Append(lines[i].Get());
            myStringBuilder.Append(System.Environment.NewLine);
        }
        text.text = myStringBuilder.ToString();
    }

    public List<String> GetCharacter() {
        List<String> output = new List<String>();
        
        if(Input.GetKeyDown(KeyCode.LeftShift)) {
            foreach (String key in shiftDict.Keys) {
                if (Input.GetKeyDown(key)) 
                    output.Add(shiftDict[key]);
            }
        } else {
            foreach (String key in shiftDict.Keys) {
                if (Input.GetKeyDown(key)) 
                    output.Add(key);
            }
        }

        return output;
    }

    public void Hide() {
        this.text.gameObject.SetActive(true);
    }

    public void Show() {
        this.text.gameObject.SetActive(false);
    }

    Dictionary<KeyCode,String> keyDict = new Dictionary<KeyCode,String>() {
        {KeyCode.BackQuote,"`"}, {KeyCode.Alpha1,"1"}, {KeyCode.Alpha2,"2"}, {KeyCode.Alpha3,"3"}, {KeyCode.Alpha4,"4"}, 
        {KeyCode.Alpha5,"5"}, {KeyCode.Alpha6,"6"}, {KeyCode.Alpha7,"7"}, {KeyCode.Alpha8,"8"}, {KeyCode.Alpha9,"9"}, {KeyCode.Alpha0,"0"}, 
        {KeyCode.Minus,"-"}, {KeyCode.Equals,"="}, {KeyCode.Q,"q"}, {KeyCode.W,"w"}, {KeyCode.E,"e"}, {KeyCode.R,"r"}, {KeyCode.T,"t"}, 
        {KeyCode.Y,"y"}, {KeyCode.U,"u"}, {KeyCode.I,"i"}, {KeyCode.O,"o"}, {KeyCode.P,"p"}, {KeyCode.LeftBracket,"["}, {KeyCode.RightBracket,"]"}, 
        {KeyCode.Backslash,"\\"}, {KeyCode.A,"a"}, {KeyCode.S,"s"}, {KeyCode.D,"d"}, {KeyCode.F,"f"}, {KeyCode.G,"g"}, {KeyCode.H,"h"}, 
        {KeyCode.J,"J"}, {KeyCode.K,"k"}, {KeyCode.L,"l"}, {KeyCode.Semicolon,";"}, {KeyCode.Quote,"'"}, {KeyCode.Z,"z"}, {KeyCode.X,"x"}, 
        {KeyCode.C,"c"}, {KeyCode.V,"v"}, {KeyCode.B,"b"}, {KeyCode.N,"n"}, {KeyCode.M,"m"}, {KeyCode.Comma,","}, {KeyCode.Period,"."}, 
        {KeyCode.Slash,"/"}, {KeyCode.Space, " "}
    };

    Dictionary<String,String> shiftDict = new Dictionary<String,String>() {
       {"`","~"}, {"1","!"}, {"2","@"}, {"3","#"}, {"4","$"}, {"5","%"}, {"6","^"}, {"7","&"}, {"8","*"}, {"9","("}, {"0",")"}, 
       {"-","_"}, {"=","+"}, {"q","Q"}, {"w","W"}, {"e","E"}, {"r","R"}, {"t","T"}, {"y","Y"}, {"u","U"}, {"i","I"}, {"o","O"}, 
       {"p","P"}, {"[","{"}, {"]","}"}, {"\\","|"}, {"a","A"}, {"s","S"}, {"d","D"}, {"f","F"}, {"g","G"}, {"h","H"}, {"j","J"}, 
       {"k","K"}, {"l","L"}, {";",":"}, {"'","\""}, {"z","Z"}, {"x","X"}, {"c","C"}, {"v","V"}, {"b","B"}, {"n","N"}, {"m","M"}, 
       {",","<"}, {".",">"}, {"/","?"}, {" ", " "}
    };

    HashSet<KeyCode> modifiers = new HashSet<KeyCode>() {
        KeyCode.LeftShift, KeyCode.RightShift, KeyCode.Delete, KeyCode.Tab, KeyCode.Return, KeyCode.CapsLock, KeyCode.LeftControl, KeyCode.RightControl,
        KeyCode.Backspace, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow
    };

    class Cursor {
        public TMP_Text cursorText;

        public Cursor(TMP_Text cursorText) {
            this.cursorText = cursorText;
        }

        public void UpdateCursor(int prefixLength, int cursorPos) {
            StringBuilder myStringBuilder = new StringBuilder();
            for(int i = 0; i<prefixLength+cursorPos; i++) {
                myStringBuilder.Append(" ");
            }
            myStringBuilder.Append("|");
            this.cursorText.text = myStringBuilder.ToString();
        }

        public void Hide() {
            this.cursorText.gameObject.SetActive(true);
        }

        public void Show() {
            this.cursorText.gameObject.SetActive(false);
        }
    }
}
