﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DTO
{
    public class Account
    {
        public Account(string userName, string displayName, int type, string password = null)
        {
            this.UserName = userName;
            this.DisplayName = displayName;
            this.Type = type;
            this.Password = password;
        }
        public Account(DataRow row)
        {
            this.UserName = row["UserName"].ToString();
            this.DisplayName = row["DisplayName"].ToString();
            this.Type= (int)row["TYPE"];
            this.Password = row["PassWord"].ToString();
        }

        private int type;

        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        private string password;

        public string Password
        {
          get { return password; }
          set { password = value; }
        }
        private string displayName;

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }
        private string userName;

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
    }
}
