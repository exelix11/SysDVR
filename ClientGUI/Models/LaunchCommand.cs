namespace SysDVRClientGUI.Models
{
    public sealed class LaunchCommand
    {
        public string Executable { get; set; }
        public string Arguments { get; set; }

        public string[] FileDependencies { get; set; }

        public override string ToString()
        {
            return $"\"{this.Executable}\" {this.Arguments}";
        }
    }
}
