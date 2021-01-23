using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using QuanLyQuanCafe.ADO;
using QuanLyQuanCafe.DTO;
namespace QuanLyQuanCafe
{
    public partial class fAdmin : Form
    {
        BindingSource foodList = new BindingSource();
        BindingSource tableList = new BindingSource();
        BindingSource categoryList = new BindingSource();
        BindingSource accountList = new BindingSource();

        public Account loginAccount;

        public fAdmin()
        {
            InitializeComponent();
            Load();
            
            
        }
       
       
        #region Methods

        List<Food> SearchFooByName(string name)
        {
            List<Food> listFood =FoodDAO.Instance.SearchFoodByName(name);

            return listFood;
        }

        void Load()
        {
            dtgvFood.DataSource = foodList;
            dtgvTable.DataSource = tableList;
            dtgvCategory.DataSource = categoryList;
            dtgvAccount.DataSource = accountList;

            LoadDateTimePickerBill();
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
            LoadListFood();
            LoadListTable();
            LoadListCategory();
            LoadAccount();
            LoadCategoryIntoCombobox(cbFoodCategory);
            LoadStatusIntoCombobox(cbTableStatus);
            AddFoodBinding();
            AddTableBinding();
            AddCategoryBinding();
            AddAccountBinding();
        }

        void AddAccountBinding()
        {
            txbUserName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "UserName", true, DataSourceUpdateMode.Never));
            txbDisplayName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "DisplayName", true, DataSourceUpdateMode.Never));
            numericUpDown1.DataBindings.Add(new Binding("Value", dtgvAccount.DataSource, "TYPE", true, DataSourceUpdateMode.Never));
        }
        void LoadAccount()
        {
            accountList.DataSource = AccountDAO.Instance.GetListAccount();
        }

        void LoadDateTimePickerBill()
        {
            DateTime today = DateTime.Now;
            dtpkFromDate.Value = new DateTime(today.Year, today.Month, 1);
            dtpkToDate.Value = dtpkFromDate.Value.AddMonths(1).AddDays(-1);
        }
        
        void LoadListBillByDate(DateTime checkIn, DateTime checkOut)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetListBillByDate(checkIn, checkOut);
        }

        void AddFoodBinding()
        {
            txbFoodName.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txbFoodID.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "ID", true, DataSourceUpdateMode.Never));
            nmFoodPrice.DataBindings.Add(new Binding("Value", dtgvFood.DataSource, "Price", true, DataSourceUpdateMode.Never));
        }

        void AddTableBinding()
        {
            txbTableName.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txbTableID.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "ID", true, DataSourceUpdateMode.Never));
        }

        void AddCategoryBinding()
        {
            txbCategoryName.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txbCategoryID.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "ID", true, DataSourceUpdateMode.Never));
        }

        void LoadCategoryIntoCombobox(ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategory();
            cb.DisplayMember = "Name";
        }

        void LoadStatusIntoCombobox(ComboBox cb)
        {
            cb.DataSource = TableDAO.Instance.GetListTable();
            cb.DisplayMember = "Status";
        }

       public void LoadListFood()
        {
            foodList.DataSource = FoodDAO.Instance.GetListFood();
        }

        void LoadListTable()
        {
            tableList.DataSource = TableDAO.Instance.GetListTable();
        }

        void LoadListCategory()
        {
            categoryList.DataSource = CategoryDAO.Instance.GetListCategory();
        }

        void AddAccount(string UserName, string displayName, int type)
        {
            if (AccountDAO.Instance.InsertAccount(UserName, displayName, type))
            {
                //if (loginAccount.UserName.Equals(UserName))
               // {
               //     MessageBox.Show("Bạn không được xóa tài khoản của chính mình. Are you OK?");
               //     return;
               // }
                MessageBox.Show("Thêm tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Thêm tài khoản thất bại");
            }
            LoadAccount();
        }

        void EditAccount(string UserName, string displayName, int type)
        {
            if (AccountDAO.Instance.UpdateAccount(UserName, displayName, type))
            {
                MessageBox.Show("Cập nhật tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Cập nhật tài khoản thất bại");
            }
            LoadAccount();
        }

        void DeleteAccount(string UserName)
        {
            if(loginAccount.UserName.Equals(UserName))
            {
                MessageBox.Show("Bạn không được xóa tài khoản của chính mình. Are you OK?");
                return;
            }
            if (AccountDAO.Instance.DeleteAccount(UserName))
            {
                MessageBox.Show("Xóa tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Xóa tài khoản thất bại");
            }
            LoadAccount();
        }

        void ResetPass(string userName)
        {

            if (AccountDAO.Instance.ResetPassword(userName))
            {
                MessageBox.Show("Đặt lại mật khẩu thành công");
            }
            else
            {
                MessageBox.Show("Đặt lại mật khẩu thất bại");
            }
        }
        #endregion

        #region Event

        

        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)numericUpDown1.Value;

            AddAccount(userName, displayName, type);
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;

           DeleteAccount(userName);
        }

        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            string displayName = txbDisplayName.Text;
            int type = (int)numericUpDown1.Value;

            EditAccount(userName, displayName, type);
        }

        private void txbResetPassword_Click(object sender, EventArgs e)
        {
            string userName = txbUserName.Text;
            ResetPass(userName);
        }

        private void btnShowAccount_Click(object sender, EventArgs e)
        {
            LoadAccount();
        }


        private void btnSeachFood_Click(object sender, EventArgs e)
        {
            foodList.DataSource = SearchFooByName(txbSearchFoodName.Text);
        }

        private void txbTableID_TextChanged(object sender, EventArgs e)
        {
            if (dtgvTable.SelectedCells.Count > 0)
            {
                int id = (int)dtgvTable.SelectedCells[0].OwningRow.Cells["id"].Value;

                Table table = TableDAO.Instance.GetStatusByID(id);

                cbTableStatus.SelectedItem = table;

                int index = -1;
                int i = 0;
                foreach (Table item in cbTableStatus.Items)
                {
                    if (item.ID == table.ID)
                    {
                        index = i;
                        break;
                    }
                    i++;
                }
                cbTableStatus.SelectedIndex = index;
            }
        }//Giúp binding combobox

        private void txbFoodID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtgvFood.SelectedCells.Count > 0)
                {
                    int id = (int)dtgvFood.SelectedCells[0].OwningRow.Cells["idCategory"].Value;

                    Category category = CategoryDAO.Instance.GetCategoryByID(id);

                    cbFoodCategory.SelectedItem = category;

                    int index = -1;

                    int i = 0;

                    foreach (Category item in cbFoodCategory.Items)
                    {
                        if (item.ID == category.ID)
                        {
                            index = i;
                            break;
                        }
                        i++;
                    }
                    cbFoodCategory.SelectedIndex = index;
                }
            }
            catch { }
        }//Giúp binding combobox

        private void btnShowFood_Click(object sender, EventArgs e)
        {
            LoadListFood();
        }//Food(thức ăn)

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;

            if (FoodDAO.Instance.InsertFood(name, categoryID, price))
            {
                MessageBox.Show("Thêm món thành công.");//món
                LoadListFood();
                if (insertFood != null)
                    insertFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi thêm thức ăn.");//thức ăn
            }
        }

        private void btnEditFood_Click(object sender, EventArgs e)
        {
            string name = txbFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;
            int id = Convert.ToInt32(txbFoodID.Text);

            if (FoodDAO.Instance.UpdateFood(id, name, categoryID, price))
            {
                MessageBox.Show("Sửa món thành công.");//món
                LoadListFood();
                if (updateFood != null)
                    updateFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi sửa thức ăn .");//thức ăn
            }
        }

        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txbFoodID.Text);

            if (FoodDAO.Instance.DeleteFood(id))
            {
                MessageBox.Show("Xóa món thành công.");//món
                LoadListFood();
                if (deleteFood != null)
                    deleteFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi xóa thức ăn.");//thức ăn
            }
        }

        private void btnViewBill_Click(object sender, EventArgs e)
        {
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
        }

        private event EventHandler insertFood;
        public event EventHandler InsertFood
        {
            add { insertFood += value; }
            remove { insertFood -= value; }
        }

        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add { deleteFood += value; }
            remove { deleteFood -= value; }
        }

        private event EventHandler updateFood;
        public event EventHandler UpdateFood
        {
            add { updateFood += value; }
            remove { updateFood -= value; }
        }

        private event EventHandler insertCategory;
        public event EventHandler InsertCategory
        {
            add { insertCategory += value; }
            remove { insertCategory -= value; }
        }

        private event EventHandler deleteCategory;
        public event EventHandler DeleteCategory
        {
            add { deleteCategory += value; }
            remove { deleteCategory -= value; }
        }

        private event EventHandler updateCategory;
        public event EventHandler UpdateCategory
        {
            add { updateCategory += value; }
            remove { updateCategory -= value; }
        }

        private event EventHandler insertTable;
        public event EventHandler InsertTable
        {
            add { insertTable += value; }
            remove { insertTable -= value; }
        }

        private event EventHandler deleteTable;
        public event EventHandler DeleteTable
        {
            add { deleteTable += value; }
            remove { deleteTable -= value; }
        }

        private event EventHandler updateTable;
        public event EventHandler UpdateTable
        {
            add { updateTable += value; }
            remove { updateTable -= value; }
        }
        //load bàn ăn lên
         private void btnShowTable_Click(object sender, EventArgs e)
          {
               LoadListTable();
          }//TableFood(bàn ăn)

         private void btnAddTable_Click(object sender, EventArgs e)
         {
             string name = txbTableName.Text;
             string status = cbTableStatus.Text;

             if(TableDAO.Instance.InsertTable(name,status))
             {
                 MessageBox.Show("Thêm bàn thành công");
                 LoadListTable();
                 if (insertTable != null)
                     insertTable(this, new EventArgs());
             }
             else
             {
                 MessageBox.Show("Có lỗi khi thêm bàn");
             }
         }

         private void btnEditTable_Click(object sender, EventArgs e)
        {
              string name = txbTableName.Text;
              string status = cbTableStatus.Text;
              int id = Convert.ToInt32(txbTableID.Text); 

             if(TableDAO.Instance.UpdateTable(id, name, status))
             {
                  MessageBox.Show("Sửa bảng thành công.");
                  LoadListTable();
                if (updateTable != null)
                    updateTable(this, new EventArgs());
             }
             else
            {
                MessageBox.Show("Có lỗi khi sửa bảng");
            }
        }

         private void btnDeleteTable_Click(object sender, EventArgs e)
         {
         
             //int idBill = ((txbFoodID.SelectedText as BillInfo).BillID);
             //int idTable = (cbTableStatus.SelectedItem as Table).ID;
             
            
             // if(MessageBox.Show("Quý khách đã sử dụng bàn "+ txbTableName.Text +" rồi hay chưa?","Thông báo",MessageBoxButtons.OKCancel)==DialogResult.OK)
             //{
             //      MessageBox.Show("Rất xin lỗi quý khách vì chức năng xóa bàn khi đã sử dụng vẫn chưa cập nhập. Mong quý khách thông cảm! Thành thật xin lỗi quý khách!!!");
             //}
             //else
            // {
                 int id = Convert.ToInt32(txbTableID.Text);
                 if (TableDAO.Instance.DeleteTable(id))
                {                   
                   MessageBox.Show("Xóa bảng thành công.");
                   LoadListTable();
                    if (deleteTable != null)
                       deleteTable(this, new EventArgs());
                 }
                  else
                 {
                    MessageBox.Show("Có lỗi khi xóa bảng");
                 }
       //  }
             
                     
        }

         private void btnShowCategory_Click(object sender, EventArgs e)
         {
             LoadListCategory();
         }//Category(danh mục)

         private void btnAddCategory_Click(object sender, EventArgs e)
         {
             string name = txbCategoryName.Text;


             if (CategoryDAO.Instance.InsertCategory(name))
             {
                 MessageBox.Show("Thêm danh mục thành công");
                 LoadListCategory();
                 if (insertCategory != null)
                     insertCategory(this, new EventArgs());
             }
             else
             {
                 MessageBox.Show("Có lỗi khi thêm danh mục");
             }
         }

         private void btnEditCategory_Click(object sender, EventArgs e)
         {
             string name = txbCategoryName.Text;
             int id = Convert.ToInt32(txbCategoryID.Text);

             if (CategoryDAO.Instance.UpdateCategory(id, name))
             {
                 MessageBox.Show("Sửa danh mục thành công");
                 LoadListCategory();
                 if (updateCategory != null)
                     updateCategory(this, new EventArgs());
             }
             else
             {
                 MessageBox.Show("Có lỗi khi sửa danh mục");
             }
         }

         private void btnDeleteCategory_Click(object sender, EventArgs e)
         {
             if (MessageBox.Show("Bạn có chắc là mình đã xóa toàn bộ thức ăn trong mục '" + txbCategoryName.Text + "' chưa?", "Thông báo", MessageBoxButtons.OKCancel) == DialogResult.OK)
             {
                 int idCategory = Convert.ToInt32(txbCategoryID.Text);
                 if (CategoryDAO.Instance.DeleteCategory(idCategory))
                 {
                     
                     MessageBox.Show("Xóa danh mục thành công");
                     LoadListCategory();
                     
                     if (deleteCategory != null)
                         deleteCategory(this, new EventArgs());
                 }
                 else
                 {
                     MessageBox.Show("Có lỗi khi xóa danh mục");
                 }
           }
             else
            {
                MessageBox.Show("Bạn vui lòng qua mục 'Thức Ăn' để xóa thức ăn trong danh mục '" + txbCategoryName.Text + "' có Mã danh mục là " + txbCategoryID.Text + " .Trân trọng!");
           }

         }
        #endregion

         private void fAdmin_Load(object sender, EventArgs e)
         {

         }

         private void btnFirstBillPage_Click(object sender, EventArgs e)
         {
             txbPageBill.Text = "1";
         }

         private void btnLastBillPage_Click(object sender, EventArgs e)
         {
             int sumRecord = BillDAO.Instance.GetNumBillListByDate(dtpkFromDate.Value, dtpkToDate.Value);

             int lastPage = sumRecord / 10;

             if (sumRecord % 10 != 0)
                 lastPage++;
             txbPageBill.Text = lastPage.ToString();
         }



         private void txbPageBill_TextChanged(object sender, EventArgs e)
         {
             dtgvBill.DataSource = BillDAO.Instance.GetListBillByDateAndDate(dtpkFromDate.Value, dtpkToDate.Value, Convert.ToInt32(txbPageBill.Text));
         }

         private void btnPrevioursBillPage_Click(object sender, EventArgs e)
         {
             int page = Convert.ToInt32(txbPageBill.Text);

             if (page > 1)
                 page--;

             txbPageBill.Text = page.ToString();
         }

         private void button1_Click(object sender, EventArgs e)
         {
             int page = Convert.ToInt32(txbPageBill.Text);
             int sumRecord = BillDAO.Instance.GetNumBillListByDate(dtpkFromDate.Value, dtpkToDate.Value);

             if (page < sumRecord)
                 page++;

             txbPageBill.Text = page.ToString();
         }

         private void rpViewer_Load(object sender, EventArgs e)
         {

         }
    }
}
