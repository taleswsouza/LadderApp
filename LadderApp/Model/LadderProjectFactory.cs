using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Model
{
    class LadderProjectFactory
    {
        public static LadderProject CreateNewProject()
        {
            LadderProject newProject = new LadderProject()
            {
                Name = "NoName"
            };
            return newProject;
        }
    }
}
