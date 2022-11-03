using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace DeskTime
{
    [DataContract]
    public class Project
    {
        [DataMember(Name = "id", EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Name = "name", EmitDefaultValue = false, IsRequired = false)]
        public string Name { get; set; }

        [DataMember(Name = "created_at", EmitDefaultValue = false, IsRequired = false)]
        public int CreatedAt { get; set; }

        [DataMember(Name = "tasks", EmitDefaultValue = false, IsRequired = false)]
        public Collection<ProjectTask> Tasks { get; set; }

        public override string ToString()
        {
            return "|ID:" + Id + " |Created:" + CreatedAt + " |Name:" + Name.ToString() + " |Tasks:" + Tasks.Count;
        }
    }
}
