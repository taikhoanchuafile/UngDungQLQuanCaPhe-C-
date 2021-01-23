﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DTO
{
    public class Bill
    {
        public Bill(int id, DateTime? datecheckin, DateTime? datecheckout,int idtable, int status, int dicount = 0)
        {
            this.ID=id;
            this.DateCheckIn = datecheckin;
            this.DateCheckOut = datecheckout;
            this.Idtable = idtable;
            this.Status = status;
            this.Discount = discount;
        }

        public Bill(DataRow row)
        {
            this.ID = (int)row["id"];
            this.DateCheckIn = (DateTime?)row["dateCheckin"];
            var deteCheckOutTemp = row["dateCheckOut"];
            if(dateCheckOut.ToString()!="")
                this.DateCheckOut = (DateTime?)deteCheckOutTemp;
            this.Idtable = (int)row["idTable"];
            this.Status = (int)row["status"];

            if (row["discount"].ToString() != "")
                this.Discount = (int)row["discount"];
        }
        private int idtable;

        public int Idtable
        {
            get { return idtable; }
            set { idtable = value; }
        }
        private int discount;

        public int Discount
        {
            get { return discount; }
            set { discount = value; }
        }

        private int status;

        public int Status
        {
            get { return status; }
            set { status = value; }
        }
        private DateTime? dateCheckOut;

        public DateTime? DateCheckOut
        {
            get { return dateCheckOut; }
            set { dateCheckOut = value; }
        }
        private DateTime? dateCheckIn;

        public DateTime? DateCheckIn
        {
            get { return dateCheckIn; }
            set { dateCheckIn = value; }
        }
        private int iD;

        public int ID
        {
            get { return iD; }
            set { iD = value; }
        }
    }
}
