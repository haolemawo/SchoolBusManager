using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using WBPlatform.StaticClasses;

namespace WBPlatform.TableObject
{
    public class UserObject : DataTableObject<UserObject>
    {
        private const string passwordHolder = "#######################";
        public string UserName { get; set; }
        //public string Password { get; set; }
        public string RealName { get; set; }
        public string Sex { get; set; }

        [JsonIgnore]
        [NotMapped]
        public bool AnyThing => IsAdmin || IsBusManager || IsClassTeacher || IsParent;
        public bool IsAdmin { get; set; }
        public bool IsBusManager { get; set; }
        public bool IsClassTeacher { get; set; }
        public bool IsParent { get; set; }

        public string AvatarPath { get; set; }
        public string PhoneNumber { get; set; }

        public string Childs
        {
            get { return string.Join(",", ChildList); }
            set { ChildList = new List<string>(value.Split(",")); }
        }
        [NotMapped]
        public List<string> ChildList { get; set; } = new List<string>();

        public float X { get; set; }
        public float Y { get; set; }
        public decimal Precision { get; set; }

        public string GetIdentifiableCode()
        {
            return string.Join("_", UserName, ObjectId);
        }

        public string GetFullIdentity() => string.Join("-", GetIdentifiableCode(), RealName);
    }
}