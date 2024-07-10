using Infrastructure.DTOs;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repository.Interface
{
    public interface ILogic
    {
        public string[] InputReder();
        public bool UploadFiles(IFormFile formfile);
        public void CreateFiles(List<UserInfo> users);
        public InputDetail? ValidUser(string lines);
        public (List<InputDetail> UsersDetail, Dictionary<int, string> UsersId, int ErrorLine) UserDetailsOfLines(string[] lines);
        public List<type> GetStatus(int recordsCount, TimeSpan firstIn, TimeSpan lastOut, TimeSpan workTime);
        public TimerDto GetTimes(IGrouping<(int id,DateTime Date),InputDetail> user);
        public List<string> GetRecords(IGrouping<(int id, DateTime Date), InputDetail> user);
        public UserInfo CreateUserInfo(IGrouping<(int id, DateTime Date), InputDetail> user, TimerDto times, List<string> records, int row,List<type> status);
        public AbsenceDto CheckAbsence(int totalLine,
            IGrouping<(int id, DateTime Date), InputDetail> userNext
            , int lineCounter,DateTime currentDate,Dictionary<int,string> absence,Dictionary<int,string> usersId,int row,bool lastUser);
        public List<UserInfo> SaveUserInOut(IEnumerable<IGrouping<(int id, DateTime Date), InputDetail>> UserDetails, DateTime CurrentDate, Dictionary<int, string> UserId);



    }
}
