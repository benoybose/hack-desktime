using System.Runtime.Serialization;

namespace DeskTime
{
    [DataContract]
    public class ActiveProject
    {
        [DataMember(Name = "id", EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Name = "name", EmitDefaultValue = false, IsRequired = false)]
        public string Name { get; set; }

        [DataMember(Name = "taskId", EmitDefaultValue = false, IsRequired = false)]
        public int TaskId { get; set; }

        [DataMember(Name = "taskName", EmitDefaultValue = false, IsRequired = false)]
        public string TaskName { get; set; }

        [DataMember(Name = "duration", EmitDefaultValue = false, IsRequired = false)]
        public string Duration { get; set; }
    }
}
