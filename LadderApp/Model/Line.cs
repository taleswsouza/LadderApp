using LadderApp.Model;
using LadderApp.Model.Instructions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace LadderApp
{
    [XmlInclude(typeof(Instruction))]
    [XmlInclude(typeof(NormallyOpenContact))]
    [XmlInclude(typeof(NormallyClosedContact))]
    [XmlInclude(typeof(OutputCoil))]
    [XmlInclude(typeof(ResetOutputInstruction))]
    [XmlInclude(typeof(TimerInstruction))]
    [XmlInclude(typeof(CounterInstruction))]
    [Serializable]

    public class Line
    {
        public Line()
        {
        }

        [XmlElement(ElementName = "Instruction")]
        public List<Instruction> Instructions { get; set; } = new List<Instruction>();
        public List<Instruction> Outputs { get; set; } = new List<Instruction>();

        public void DeleteLine()
        {
            Outputs.Reverse();
            foreach (Instruction instruction in Outputs)
            {
                instruction.Dispose();
            }
            Outputs.Clear();

            Instructions.Reverse();
            foreach (Instruction instruction in Instructions)
            {
                instruction.Dispose();
            }
            Instructions.Clear();
        }
    }
}
