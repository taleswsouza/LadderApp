using System;
using System.Xml.Serialization;

namespace LadderApp.Model
{
    [XmlInclude(typeof(LadderAddressing))]
    [Serializable]
    public class LadderProject
    {
        internal LadderProject()
        {
        }

        public string Name { get; set; }
        public Device Device { get; set; }
        public LadderAddressing Addressing { get; set; }
        public LadderProgram Program { get; set; }

    }
}
