using System;
using System.Collections.Generic;

using WBPlatform.Database.IO;
using WBPlatform.StaticClasses;

namespace WBPlatform.TableObject
{
    public abstract class DataTableObject
    {
        public const string DefaultObjectID = "0000000000";
        public readonly DateTime DefaultTime = DateTime.MinValue;
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
    public abstract class DataTableObject<T> : DataTableObject where T : new()
    {
        public virtual T Default => new T();
    }

    public class DataTableComparer<T> : IEqualityComparer<T> where T : DataTableObject<T>, new()
    {
        public static DataTableComparer<T> Default { get; } = new DataTableComparer<T>();
        public bool Equals(T x, T y) => x.ObjectId == y.ObjectId;
        public int GetHashCode(T obj) => obj.ObjectId.GetHashCode();
    }
}
