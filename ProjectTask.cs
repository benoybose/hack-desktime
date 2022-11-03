using System.Runtime.Serialization;

namespace DeskTime
{
    [DataContract]
    public class ProjectTask
    {
        [DataMember(Name = "id", EmitDefaultValue = false, IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Name = "name", EmitDefaultValue = false, IsRequired = false)]
        public string Name { get; set; }
    }
}
