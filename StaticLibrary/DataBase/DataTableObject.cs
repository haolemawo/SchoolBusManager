using System;
using System.Collections.Generic;

using WBPlatform.Database.IO;
using WBPlatform.StaticClasses;
using Newtonsoft.Json;

namespace WBPlatform.TableObject
{
    public abstract class DataTableObject
    {
        [JsonIgnore] public const string TABLE_Mgr_StuData = "StudentsData";
        [JsonIgnore] public const string TABLE_Mgr_Classes = "Classes";
        [JsonIgnore] public const string TABLE_Mgr_BusData = "SchoolBuses";
        [JsonIgnore] public const string TABLE_Mgr_WeekIssue = "WeeklyIssues";
        [JsonIgnore] public const string TABLE_Gen_UserTable = "AllUsersTable";
        [JsonIgnore] public const string TABLE_Gen_General = "GeneralData";
        [JsonIgnore] public const string TABLE_Gen_Bugreport = "UserQuestions";
        [JsonIgnore] public const string TABLE_Gen_Notification = "Notifications";
        [JsonIgnore] public const string TABLE_Gen_UserRequest = "UserRequest";
        [JsonIgnore] public const string DefaultObjectID = "0000000000";
        [JsonIgnore] public readonly DateTime DefaultTime = DateTime.MinValue;

        [JsonIgnore]
        public abstract string Table { get; }

        public virtual string ObjectId { get; set; }
        public virtual DateTime CreatedAt { get; internal set; }
        public virtual DateTime UpdatedAt { get; internal set; }
        public virtual void ReadFields(DataBaseIO input)
        {
            ObjectId = input.GetString("objectId");
            CreatedAt = input.GetDateTime("createdAt");
            UpdatedAt = input.GetDateTime("updatedAt");
        }

        public virtual void WriteObject(DataBaseIO output, bool all)
        {
            output.Put("objectId", ObjectId);
            if (all)
            {
                output.Put("createdAt", CreatedAt);
                output.Put("updatedAt", UpdatedAt);
            }
        }
    }
    public abstract class DataTableObject<T> : DataTableObject where T : new() { }

    public class DataTableComparer<T> : IEqualityComparer<T> where T : DataTableObject
    {
        public static DataTableComparer<T> Default => new DataTableComparer<T>();
        public bool Equals(T x, T y) => x.ObjectId == y.ObjectId;
        public int GetHashCode(T obj) => obj.ObjectId.GetHashCode();
    }
}
