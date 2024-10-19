using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Line  : ICloneable
{
    public enum LineType {
        Command,
        Response
    }
    public String content;
    public String prefix;
    public LineType lineType;

    public Line() {}

    public Line(string content) {
        this.content = content;
        this.prefix = "";
        this.lineType = LineType.Response;
    }

    public Line(string content, bool isCommand, string prefix) {
        if (isCommand) {
            this.content = content;
            this.prefix = prefix;
            this.lineType = LineType.Command;
        } else {
            this.content = content;
        this.prefix = "";
        this.lineType = LineType.Response;
        }
    }

    public String Get() {
        return this.prefix + this.content;
    }

    public object Clone() {
        return this.MemberwiseClone();
    }

    public void Insert(int startIndex, String value) {
        content = content.Insert(startIndex, value);
    }

    public void Remove(int startIndex) {
        content = content.Remove(startIndex, 1);
    }

    public int GetPrefixLength() {
        return prefix.Length;
    }
}
