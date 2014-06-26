using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paperboy
{
    internal enum ResponseCode
    {
        ServerReadyPostingAllowed = 200,
        ServerReadyNoPostingAllowed = 201,

        NewsgroupSelected = 211,
        NoSuchNewsgroup = 411,

        AuthenticationRequired = 480,
        PasswordRequired = 381,
        AuthenticationAccepted = 281,

        ArticleRetrieved = 220
    }
}
