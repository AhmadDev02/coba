using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KN_KAMPUS_MERDEKA.COMMON.Entity.Systems
{
    public partial class mUser
    {
        public int intUserID { get; set; } = 0;
        public string txtUserName { get; set; }
        public string txtFullName { get; set; }
        public string txtNick { get; set; }
        public string txtEmpID { get; set; }
        public string txtEmail { get; set; }
        public Nullable<bool> bitActive { get; set; } = true;
        public Nullable<bool> bitUseActiveDirectory { get; set; } = false;
        public string txtPassword { get; set; }
        public Nullable<DateTime> dtmLastLogin { get; set; } = DateTime.Now;
        public string txtInsertedBy { get; set; }
        public Nullable<DateTime> dtmInsertedDate { get; set; } = DateTime.Now;
        public string txtUpdatedBy { get; set; }
        public Nullable<DateTime> dtmUpdatedDate { get; set; } = DateTime.Now;
        public string txtGUID { get; set; } = Guid.NewGuid().ToString();
    }
}
