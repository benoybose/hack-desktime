using System.Runtime.Serialization;

namespace DeskTime
{
    [DataContract]
    public class ErrorData
    {
        [DataMember(Name = "code", EmitDefaultValue = false, IsRequired = false)]
        public string Code { get; set; }

        [DataMember(Name = "description", EmitDefaultValue = false, IsRequired = false)]
        public string Description { get; set; }
    }
}
