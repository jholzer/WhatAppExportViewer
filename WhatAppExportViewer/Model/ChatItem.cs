using System;
using System.Collections.Generic;
using System.Text;

namespace WhatAppExportViewer.Model
{
    public class ChatItem
    {
        public ChatItem(string name, string text, DateTime timeStamp)
        {
            Name = name;
            Text = text;
            TimeStamp = timeStamp;
        }

        public string Name { get; }
        public string Text { get; set; }
        public DateTime TimeStamp { get; set; }

        public void AppendText(string text)
        {
            Text = $"{Text}\n{text}";
        }
    }
}
