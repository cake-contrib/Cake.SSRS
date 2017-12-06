using System;
using System.Collections.Generic;
using System.Text;

namespace Cake.SSRS
{
    public enum ProxyCredentialType
    {
        None = 0,
        Basic = 1,
        Digest = 2,
        Ntlm = 3,
        Windows = 4
    }
}
