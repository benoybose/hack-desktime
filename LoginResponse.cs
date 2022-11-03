using System.Runtime.Serialization;

namespace DeskTime
{
    [DataContract]
    public class LoginResponse
    {
        [DataMember(Name = "session", EmitDefaultValue = false)]
        public string Session { get; set; }
    }
}
