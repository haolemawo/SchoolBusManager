using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBPlatform.Database
{
    public enum DBVerbs
    {
        Create = 0,
        QuerySingle = 1,
        QueryMulti = 2,
        Update = 3,
        Delete = 4
    }

    public enum DBQueryStatus
    {
        INJECTION_DETECTED = -3,
        NOT_CONNECTED = -2,
        INTERNAL_ERROR = -1,
        NO_RESULTS = 0,
        ONE_RESULT = 1,
        MORE_RESULTS
    }
}
