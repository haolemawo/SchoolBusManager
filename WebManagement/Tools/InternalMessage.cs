using WBPlatform.StaticClasses;
using WBPlatform.TableObject;

namespace WBPlatform.WebManagement.Tools
{
    public struct InternalMessage
    {
        public GlobalMessageTypes _Type { get; set; }
        public UserObject User { get; set; }
        public object DataObject { get; set; }
        public string Identifier { get; set; }
    }
}
