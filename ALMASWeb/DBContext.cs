using System.Data.Entity;
using ALMASWeb.Models;

namespace ALMASWeb
{
    public class DBContext : DbContext
    {
        /* ROOT ***********************************************************************************************************************************************/

        public DbSet<ActivityLogsModel> ActivityLogsModel { get; set; }

        /* USER ACCOUNTS **************************************************************************************************************************************/

        public DbSet<OperatorModel> OperatorModel { get; set; }
        public DbSet<OperatorPrivilegePayrollModel> OperatorPrivilegePayrollModel { get; set; }

        /******************************************************************************************************************************************************/
    }
}