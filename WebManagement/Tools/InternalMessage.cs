using System;
using WBPlatform.StaticClasses;
using WBPlatform.TableObject;

namespace WBPlatform.WebManagement.Tools
{
    public struct InternalMessage
    {
        public InternalMessage(InternalMessageTypes Type, UserObject user, object dataObject, string identifier) : this()
        {
            _Type = Type;
            User = user ?? throw new ArgumentNullException(nameof(user));
            DataObject = dataObject;
            Identifier = identifier;
        }

        public InternalMessageTypes _Type { get; set; }
        public UserObject User { get; set; }
        public object DataObject { get; set; }
        public string Identifier { get; set; }
    }
}
