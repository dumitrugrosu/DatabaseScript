namespace DatabaseScript.Context
{
    public class ScriptDbContext
    {
        public object? Barges { get; set; }
        public object? Pilots { get; set; }
        public object? Tugs { get; set; }

        public ScriptDbContext() { }
    }
}
