﻿using Newtonsoft.Json;
using System;
using System.Linq;

namespace WBPlatform.TableObject
{
    public struct UserGroup
    {
        [JsonIgnore] public bool AnyThing => IsAdmin || IsBusManager || IsClassTeacher || IsParent;

        public bool IsAdmin { get; private set; }
        public bool IsBusManager { get; private set; }
        public bool IsClassTeacher { get; private set; }
        public bool IsParent { get; private set; }

        public UserGroup(bool isAdmin, bool isTeacher, bool isBusManager, bool isParent)
        {
            IsAdmin = isAdmin;
            IsClassTeacher = isTeacher;
            IsBusManager = isBusManager;
            IsParent = isParent;
        }
    }
}
