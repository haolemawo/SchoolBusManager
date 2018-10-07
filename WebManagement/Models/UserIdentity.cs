using System;
using System.Security.Principal;

using WBPlatform.TableObject;

namespace WBPlatform.WebManagement.Tools
{
    public class UserIdentity
    {
        public GenericIdentity Identity { get; private set; }
        public UserIdentity(string userAgent, UserObject user)
        {
            UserAgent = userAgent ?? throw new ArgumentNullException(nameof(userAgent));
            User = user ?? throw new ArgumentNullException(nameof(user));
            Identity = new GenericIdentity(user.GetIdentifiableCode());
            SetLastActive();
        }

        public string ApiTicket { get; private set; }
        public UserObject User { get; }
        public string UserAgent { get; }
        public DateTime LastActive { get; private set; }

        public void SetApiTicket(string Ticket) => ApiTicket = Ticket;
        public void SetLastActive() => LastActive = DateTime.Now;
    }
}
