using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CategoryProducts
{
    public class CheckUser
    {
        public string login { get; set; }
        public bool IsAdmin { get; }
        public bool IsManager { get; }
        public string GetStatuseMethod()
        {
            string Status;
            if (IsAdmin)
            {
                Status = "Admin";
            }
            else if (IsManager)
            {
                Status = "Manager";
            }
            else
            {
                Status = "User";
            }
             return Status;
        }
        public CheckUser(string login, bool isAdmin,bool isManager)
        {
            login = login.Trim();
            IsAdmin = isAdmin;
            IsManager = isManager;
        }
             
    }
}
