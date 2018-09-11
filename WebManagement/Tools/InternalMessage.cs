using WBPlatform.StaticClasses;
using WBPlatform.TableObject;

namespace WBPlatform.WebManagement.Tools
{
    public struct InternalMessage
    {
        public InternalMessageTypes _Type { get; set; }
        public UserObject User { get; set; }
        public object DataObject { get; set; }
        public string Identifier { get; set; }
    }
}
