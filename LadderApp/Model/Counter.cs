using System;
using System.Collections.Generic;
using System.Text;

namespace LadderApp.Model
{
    public class Counter
    {
        public int Type = 0;
        public int Preset;
        public int Accumulated = 0;
        public bool Enable = false;
        public bool Pulse = true;
        public bool Done = false;
        public bool Reset = false;
    }
}
