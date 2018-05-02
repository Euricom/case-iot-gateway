namespace Euricom.IoT.Models
{
    public class Role
    {
        private Role()
        {
            //EF
        }

        public Role(string name): this()
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}