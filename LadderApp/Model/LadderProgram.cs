using System;
using System.Collections.Generic;
using System.Text;



using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;
using LadderApp.Formularios;
using LadderApp.Resources;
using LadderApp.Services;

namespace LadderApp.Model
{
    [XmlInclude(typeof(LadderAddressing))]
    [Serializable]
    public class LadderProgram
    {
        public LadderProgram()
        {
        }

        public string Name { get; set; } = "NoName";

        public LadderAddressing addressing = new LadderAddressing();

        public Device device;
        public List<Line> Lines { get; } = new List<Line>();

        public int InsertLineAtEnd(Line line)
        {
            Lines.Add(line);
            return (Lines.Count - 1);
        }

        public int InsertLineAt(int index, Line line)
        {
            if (index > Lines.Count)
                index = Lines.Count;

            if (index < 0)
                index = 0;

            Lines.Insert(index, line);
            return index;
        }

        public void RemoveLineAt(int index)
        {
            Lines[index].DeleteLine();
            Lines.RemoveAt(index);
        }
    }
}
