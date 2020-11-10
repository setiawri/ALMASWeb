using System.Data.Entity;
using ALMASWeb.Models;

namespace ALMASWeb
{
    public class DBContext : DbContext
    {
        /* ROOT ***********************************************************************************************************************************************/

        public DbSet<ActivityLogsModel> ActivityLogsModel { get; set; }
        public DbSet<InventoryGroupModel> InventoryGroupModel { get; set; }
        public DbSet<InventoryGroupAccessModel> InventoryGroupAccessModel { get; set; }
        public DbSet<WarehouseModel> WarehouseModel { get; set; }
        public DbSet<WarehouseAccessModel> WarehouseAccessModel { get; set; }

        /* USER ACCOUNTS **************************************************************************************************************************************/

        public DbSet<OperatorModel> OperatorModel { get; set; }
        public DbSet<OperatorPrivilegeDataManagementModel> OperatorPrivilegeDataManagementModel { get; set; }

        /******************************************************************************************************************************************************/
    }
}