﻿using System;
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
        IIdentity _identity;

        public IdentityReferenceCollection Groups;
        public List<Permission> Permissions;
        public IIdentity Identity
        {
            get
            {
                return _identity;
            }
            private set
            {
                _identity = value;
            }
        }

        public CustomPrincipal(WindowsIdentity winId)
        {
            Identity = winId;
            Groups = winId.Groups;

            Permissions = new List<Permission>();

            foreach (var group in Groups)
            {
                string groupName = group.Translate(typeof(NTAccount)).Value;
                if (groupName.Contains("\\"))
                    groupName = groupName.Split(new[] { "\\" }, StringSplitOptions.None)[1];
                switch (groupName)
                {
                    case ("Readers"):
                        Permissions.Add(Permission.Read);
                        break;
                    case ("Subscribers"):
                        Permissions.Add(Permission.Read);
                        Permissions.Add(Permission.Subscribe);
                        break;
                    case ("Admins"):
                        Permissions.Add(Permission.Read);
                        Permissions.Add(Permission.Subscribe);
                        Permissions.Add(Permission.Modify);
                        Permissions.Add(Permission.Supervise);
                        break;
                }
            }
        }
        /// <summary>
        /// Override IPrincipal, postoji cisto da bi se osigurala implementacija interfejsa i ne koristi se u programu.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool IsInRole(string role)
        {
            bool retVal = false;
            foreach (var group in Groups)
            {
                if (role == group.Value)
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Provera permisije datog CustomPrincipal-a.
        /// </summary>
        /// <param name="permission">Ime dozvole.</param>
        /// <returns>True u slucaju da ima tu dozvolu u listi Permissions, inace false.</returns>
        public bool HasPermission(string permission)
        {
            bool retVal = false;
            foreach (var actualPermission in Permissions)
            {
                if (actualPermission.ToString() == permission)
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }
    }
}
