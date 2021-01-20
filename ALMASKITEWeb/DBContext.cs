using ALMASKITEWeb.Models;
using System.Data.Entity;

namespace ALMASKITEWeb
{
    public class DBContext : DbContext
    {

        /* USER ACCOUNTS **************************************************************************************************************************************/

        public DbSet<OperatorModel> OperatorModel { get; set; }
        public DbSet<OperatorPrivilegeDataManagementModel> OperatorPrivilegeDataManagementModel { get; set; }

        /******************************************************************************************************************************************************/
    }
}