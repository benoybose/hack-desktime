using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace DeskTime.Properties
{
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [DebuggerNonUserCode]
    [CompilerGenerated]
    internal class Resources
    {
        private static ResourceManager resourceMan;

        private static CultureInfo resourceCulture;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (resourceMan == null)
                {
                    resourceMan = new ResourceManager("DeskTime.Properties.Resources", typeof(Resources).Assembly);
                }
                return resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        internal static Bitmap desktime => (Bitmap)ResourceManager.GetObject("desktime", resourceCulture);

        internal static string ErrorMsgCon => ResourceManager.GetString("ErrorMsgCon", resourceCulture);

        internal static string ErrorMsgDt => ResourceManager.GetString("ErrorMsgDt", resourceCulture);

        internal static Icon idle => (Icon)ResourceManager.GetObject("idle", resourceCulture);

        internal static string InitFailed => ResourceManager.GetString("InitFailed", resourceCulture);

        internal static string InitFailedTitle => ResourceManager.GetString("InitFailedTitle", resourceCulture);

        internal static Bitmap IntroPopup => (Bitmap)ResourceManager.GetObject("IntroPopup", resourceCulture);

        internal static Bitmap IntroStartPopup => (Bitmap)ResourceManager.GetObject("IntroStartPopup", resourceCulture);

        internal static Bitmap logo_login => (Bitmap)ResourceManager.GetObject("logo_login", resourceCulture);

        internal static Bitmap Logo_Symbol => (Bitmap)ResourceManager.GetObject("Logo Symbol", resourceCulture);

        internal static string NotAvailable => ResourceManager.GetString("NotAvailable", resourceCulture);

        internal static string NotAvailableTitle => ResourceManager.GetString("NotAvailableTitle", resourceCulture);

        internal static Icon offline => (Icon)ResourceManager.GetObject("offline", resourceCulture);

        internal static Icon online => (Icon)ResourceManager.GetObject("online", resourceCulture);

        internal static Icon ooffice => (Icon)ResourceManager.GetObject("ooffice", resourceCulture);

        internal static string OutOfOfficeBalloonText => ResourceManager.GetString("OutOfOfficeBalloonText", resourceCulture);

        internal static string OutOfOfficeBalloonTitle => ResourceManager.GetString("OutOfOfficeBalloonTitle", resourceCulture);

        internal static Bitmap oval_1 => (Bitmap)ResourceManager.GetObject("oval-1", resourceCulture);

        internal static Bitmap oval_2 => (Bitmap)ResourceManager.GetObject("oval-2", resourceCulture);

        internal static Icon pause => (Icon)ResourceManager.GetObject("pause", resourceCulture);

        internal static string PaymentErrorBalloonText => ResourceManager.GetString("PaymentErrorBalloonText", resourceCulture);

        internal static string PaymentErrorBalloonTitle => ResourceManager.GetString("PaymentErrorBalloonTitle", resourceCulture);

        internal static string PleaseWait => ResourceManager.GetString("PleaseWait", resourceCulture);

        internal static Icon project => (Icon)ResourceManager.GetObject("project", resourceCulture);

        internal static Icon project_slack => (Icon)ResourceManager.GetObject("project_slack", resourceCulture);

        internal static Bitmap project_started => (Bitmap)ResourceManager.GetObject("project_started", resourceCulture);

        internal static Icon slack => (Icon)ResourceManager.GetObject("slack", resourceCulture);

        internal static string TrialOverBalloonText => ResourceManager.GetString("TrialOverBalloonText", resourceCulture);

        internal static string TrialOverBalloonTitle => ResourceManager.GetString("TrialOverBalloonTitle", resourceCulture);

        internal static string UpdateRunFailed => ResourceManager.GetString("UpdateRunFailed", resourceCulture);

        internal static string UpdateRunFailedTitle => ResourceManager.GetString("UpdateRunFailedTitle", resourceCulture);

        internal static string UserName => ResourceManager.GetString("UserName", resourceCulture);

        internal static string WebFailed => ResourceManager.GetString("WebFailed", resourceCulture);

        internal static string WebFailedTitle => ResourceManager.GetString("WebFailedTitle", resourceCulture);

        internal Resources()
        {
        }
    }
}
