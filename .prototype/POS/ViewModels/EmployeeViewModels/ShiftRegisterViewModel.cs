using CommunityToolkit.Mvvm.ComponentModel;
using OpenTK.Graphics.OpenGL;
using POS.Models;
using POS.Services.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.ViewModels.EmployeeViewModels
{
    public class ShiftRegisterViewModel : ObservableObject
    {
        private DateTime today = DateTime.Now;
        public DateTime startOfWeek;


        IDao dao = new PostgreSqlDao();
        public List<int> RegisteredDayList = new List<int>();
        public List<int> FullRegisteredList = new List<int>();

        public ShiftRegisterViewModel()
        {
            startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1);
            getRegisterShift(startOfWeek);
            getFullRegisteredShift(startOfWeek);

        }

        public void getRegisterShift(DateTime startOfWeek)
        {
            RegisteredDayList = dao.getRegisterShift(CurrentUser.username, startOfWeek);
        }
        public void getFullRegisteredShift(DateTime startOfWeek)
        {
            FullRegisteredList = dao.getFullRegisteredShift(startOfWeek);
        }
        public void RegisterShift()
        {
            foreach (var x in RegisteredDayList)
            {
                dao.setRegisterShift(CurrentUser.username, startOfWeek.AddDays(x % 8 - 1), x / 8);
            }

        }
        public void DeleteShift(int index)
        {
            dao.deleteRegisterShift(startOfWeek.AddDays(index % 8 - 1), index / 8);

        }

        public void checkin()
        {
            dao.checkin();

        }

        public void checkout()
        {
            dao.checkout();
        }





    }
}
