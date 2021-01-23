using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.ADO
{
    public class TableDAO
    {
        private static TableDAO instance;

        public static TableDAO Instance
        {
            get { if (instance == null) instance = new TableDAO(); return TableDAO.instance; }
            private set { TableDAO.instance = value; }
        }

        public static int TableWidth = 120;
        public static int TableHeight = 120;

        private TableDAO() { }

        public void SwitchTable(int id1, int id2)
        {
            DataProvider.Instance.ExcuteQuery("USP_SwitchTabel @idTable1 , @idTable2", new object[] { id1, id2 });
        }
        public List<Table> LoadTableList()
        {
            List<Table> tablelist = new List<Table>();

            DataTable data = DataProvider.Instance.ExcuteQuery("USP_GetTableList");

            foreach (DataRow item in data.Rows)
            {
                Table table = new Table(item);
                tablelist.Add(table);
            }

            return tablelist;
        }

        public List<Table> GetListTable()
        {
            List<Table> tablelist = new List<Table>();

            DataTable data = DataProvider.Instance.ExcuteQuery("SELECT * FROM TableFood");

            foreach (DataRow item in data.Rows)
            {
                Table table = new Table(item);
                tablelist.Add(table);
            }

            return tablelist;
        }

        public Table GetStatusByID(int id)
        {
            Table table = null;
            string query = "SELECT * FROM TableFood WHERE id =" + id;
            DataTable data = DataProvider.Instance.ExcuteQuery(query);
            foreach (DataRow item in data.Rows)
            {
                table = new Table(item);
                return table;
            }

            return table;
        }

        public bool InsertTable(string name, string status)
        {
            string query = string.Format("INSERT dbo.TableFood(name,status) VALUES (N'{0}',N'{1}')", name, status);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool UpdateTable(int idTable, string name, string status)
        {
            string query = string.Format("UPDATE dbo.TableFood SET name =N'{0}', status=N'{1}' WHERE id = {2} ", name, status, idTable);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool DeleteTable( int idTable)
        {

               // BillInfoDAO.Instance.DeleteBillInfoByBillID(idTable);

                //BillDAO.Instance.DeleteBillByTableID(idTable);

            string query = string.Format("exec USP_DeleteTableFoodByID @id = {0}",idTable);
                int result = DataProvider.Instance.ExecuteNonQuery(query);

                return result > 0;
                 
       
          
        }

    }
}
           // BillInfoDAO.Instance.DeleteBillInfoByFoodID(idFood);
           // string query = string.Format("DELETE dbo.Food WHERE id = {0}",idFood);
            //int result = DataProvider.Instance.ExecuteNonQuery(query);

            //return result > 0;