using Newtonsoft.Json;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using WBPlatform.Database.IO;
using WBPlatform.StaticClasses;

namespace WBPlatform.TableObject
{
    public class UserChangeRequest : DataTableObject<UserChangeRequest>
    {
        [ForeignKey("CreatorID")]
        public UserObject User { get; set; }
        [ForeignKey("SolverID")]
        public UserObject Solver { get; set; }
        public string DetailTexts { get; set; }
        public UserChangeRequestTypes RequestTypes { get; set; }
        public UCRProcessStatus Status { get; set; }
        public UCRRefusedReasons ProcessResultReason { get; set; }
        public string NewContent { get; set; }
    }
}
