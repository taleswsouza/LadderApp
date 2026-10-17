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
