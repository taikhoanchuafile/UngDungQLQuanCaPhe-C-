using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.ADO
{
    public class AccountDAO
    {
        private static AccountDAO instance;

        public static AccountDAO Instance
        {
            get { if(instance==null) instance=new AccountDAO() ;return AccountDAO.instance; }
            private set { instance = value; }
        }

        private AccountDAO() { }
        
        public bool Login(string userName, string passWord)
        {   
            //mã hóa mật khẩu
            byte[] temp = ASCIIEncoding.ASCII.GetBytes(passWord);
            byte[] hasData = new MD5CryptoServiceProvider().ComputeHash(temp);

            string hasPass = "";
            foreach(byte item in hasData)
            {
                hasPass += item;
            }

            //var list = hasData.ToString();//Giúp không thể đưa CSDL lên lại được
            //list.Reverse();
            // Gõ MD5-->Add thư viện vào:System.Security.Cryptography
            string query = "USP_Login @userName , @passWord";// F9 chỗ nà để xem dãy số của hasPass

            DataTable result = DataProvider.Instance.ExcuteQuery(query, new object[] { userName, hasPass});//{userName,list (nếu xài)/});


            return result.Rows.Count > 0;
        }

        public bool UPdateAccount(string userName, string displayName, string pass, string newPass)
        {
            int result = DataProvider.Instance.ExecuteNonQuery("EXEC USP_UpdateAccount @userName , @displayName , @password , @newPassword", new object[]{userName,displayName,pass,newPass});

            return result > 0;
        }

        public DataTable GetListAccount()
        {
            return DataProvider.Instance.ExcuteQuery("SELECT UserName ,DisplayName ,TYPE FROM dbo.Account");
        }

        public Account GetAccountByUserName(string userName)
        {
            DataTable data = DataProvider.Instance.ExcuteQuery("SELECT * FROM Account WHERE UserName = '" + userName + "'");

            foreach(DataRow item in data.Rows )
            {
                return new Account(item);
            }

            return null;
        }

        public bool InsertAccount(string name, string displayName, int type)
        {
            string query = string.Format("INSERT INTO dbo.Account(UserName, DisplayName, Type, PassWord) VALUES(N'{0}',N'{1}',{2}, N'{3}')", name, displayName, type, "1962026656160185351301320480154111117132155");
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool UpdateAccount(string name, string displayName, int type)
        {
            string query = string.Format("UPDATE dbo.Account SET DisplayName = N'{1}', TYPE = {2} WHERE UserName = N'{0}'",name,displayName,type);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool DeleteAccount(string name)
        {

            string query = string.Format("DELETE dbo.Account WHERE UserName = N'{0}'",name);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool ResetPassword(string name)
        {
            string query = string.Format("UPDATE dbo.Account SET PassWord = N'1962026656160185351301320480154111117132155' WHERE UserName = N'{0}'", name);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

      //  nonquery: trả số dòng, thường dùng trong insert,update,delete
        // excutequery: trả về dòng và kết quả
        // excuteSalary: trả về kiểu như count(),sum,,.....
        
    }
}
