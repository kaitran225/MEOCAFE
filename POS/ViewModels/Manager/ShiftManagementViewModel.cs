using CommunityToolkit.Mvvm.ComponentModel;
using POS.Models;
using POS.Services.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.ViewModels.Manager
{
    public class ShiftManagementViewModel : ObservableObject
    {
        private DateTime today = DateTime.Now;
        private DateTime startOfWeek;
        public List<ShiftStatistic> Shiftstatistics = new List<ShiftStatistic> { };

        IDao dao = new PostgreSqlDao();
        public List<string> FullRegisteredList = new List<string>();

        public ShiftManagementViewModel()
        {
            startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1);
            getShiftstatistics();
            getAll(startOfWeek);
        }
        void getShiftstatistics()
        {
            dao.getShiftstatistics(Shiftstatistics);
        }
        public void getAll(DateTime startOfWeek)
        {
            FullRegisteredList.Clear(); 
            for (int i = 0; i < 7; i++)
            {
                for (int shift = 1; shift < 5; shift++)
                {
                    FullRegisteredList.Add(
                    dao.getFullRegisteredShiftForManager(startOfWeek.AddDays(i), shift));
                }
            }
        }


    }
}

