using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.Utility
{
    public class SD // System Definition
    {
        // Roles
        public const string Role_JobSeeker = "JobSeeker";
        public const string Role_Employer = "Employer";
        public const string Role_Admin = "Admin";

        // Account Status
        public const string StatusSuspending = "Suspending";
        public const string StatusPending = "Pending";
        public const string StatusActive = "Active";

        // CV Status
        public const string StatusResponded = "Responded";

        // Chatting Action 
        public const string ReloadPage = "Reload";
        public const string RemoveStartChat = "RemoveStart";
    }
}
