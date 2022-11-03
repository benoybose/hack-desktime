using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace DeskTime
{
    [KnownType(typeof(LoginResponse))]
    [DataContract]
    public class Response
    {
        [DataMember(Name = "error", EmitDefaultValue = false, IsRequired = false)]
        public ErrorData Error { get; set; }

        [DataMember(Name = "__lang", EmitDefaultValue = false, IsRequired = false)]
        public string Lang { get; set; }

        [DataMember(Name = "__companyActivityStatus", EmitDefaultValue = false, IsRequired = false)]
        public int? CompanyActivityStatus { get; set; }

        [DataMember(Name = "__ipRestriction", EmitDefaultValue = false, IsRequired = false)]
        public bool? IpRestriction { get; set; }

        [DataMember(Name = "__disablePrivateTime", EmitDefaultValue = false, IsRequired = false)]
        public bool? DisablePrivateTime { get; set; }

        [DataMember(Name = "__disableWindowTitle", EmitDefaultValue = false, IsRequired = false)]
        public bool? DisableWindowTitle { get; set; }

        [DataMember(Name = "__disableMouseClick", EmitDefaultValue = false, IsRequired = false)]
        public bool? DisableMouseClick { get; set; }

        [DataMember(Name = "__disableMouseMovement", EmitDefaultValue = false, IsRequired = false)]
        public bool? DisableMouseMovement { get; set; }

        [DataMember(Name = "__disableLogOut", EmitDefaultValue = false, IsRequired = false)]
        public bool? DisableLogOut { get; set; }

        [DataMember(Name = "__privateTime", EmitDefaultValue = false, IsRequired = false)]
        public bool? PrivateTime { get; set; }

        [DataMember(Name = "__inOffice", EmitDefaultValue = false, IsRequired = false)]
        public bool? InOffice { get; set; }

        [DataMember(Name = "__track", EmitDefaultValue = false, IsRequired = false)]
        public bool? IsTracking { get; set; }

        [DataMember(Name = "__idleTimeInterval", EmitDefaultValue = false, IsRequired = false)]
        public int? IdleTimeInterval { get; set; }

        [DataMember(Name = "__userId", EmitDefaultValue = false, IsRequired = false)]
        public int? UserId { get; set; }

        [DataMember(Name = "__screenshots", EmitDefaultValue = false, IsRequired = false)]
        public bool? AllowScreenCapture { get; set; }

        [DataMember(Name = "__screenshotsInterval", EmitDefaultValue = false, IsRequired = false)]
        public int? ScreenshotsCaptureInterval { get; set; }

        [DataMember(Name = "__activeProject", EmitDefaultValue = false, IsRequired = false)]
        public ActiveProject ActiveProject { get; set; }

        [DataMember(Name = "__last_project_list_update", EmitDefaultValue = false, IsRequired = false)]
        public int? LastProjectListUpdate { get; set; }

        [DataMember(Name = "session", EmitDefaultValue = false, IsRequired = false)]
        public string Session { get; set; }

        [DataMember(Name = "access_token", EmitDefaultValue = false, IsRequired = false)]
        public string AccessToken { get; set; }

        [DataMember(Name = "access_token_id", EmitDefaultValue = false, IsRequired = false)]
        public int? AccessTokenId { get; set; }

        [DataMember(Name = "name", EmitDefaultValue = false, IsRequired = false)]
        public string Name { get; set; }

        [DataMember(Name = "version", EmitDefaultValue = false, IsRequired = false)]
        public string Version { get; set; }

        [DataMember(Name = "url", EmitDefaultValue = false, IsRequired = false)]
        public string Url { get; set; }

        [DataMember(Name = "status", EmitDefaultValue = false, IsRequired = false)]
        public int Status { get; set; }

        [DataMember(Name = "slacking", EmitDefaultValue = false, IsRequired = false)]
        public bool? Slacking { get; set; }

        [DataMember(Name = "push_interval", EmitDefaultValue = false, IsRequired = false)]
        public int? PushInterval { get; set; }

        [DataMember(Name = "projects", EmitDefaultValue = false, IsRequired = false)]
        public Collection<Project> Projects { get; set; }

        [DataMember(Name = "projects_recent", EmitDefaultValue = false, IsRequired = false)]
        public Collection<Project> RecentProjects { get; set; }
    }
}
