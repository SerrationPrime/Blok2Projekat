using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

public enum Permission { Read, Supervise, Modify, Subscribe };

namespace UserLogic
{
    public class CustomPrincipal : IPrincipal
    {
        public IIdentity Identity => throw new NotImplementedException();

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }

        public bool HasPermission (string permission)
        {
            throw new NotImplementedException();
        }
    }
}
