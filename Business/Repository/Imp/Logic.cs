using business.Repository.Interface;
using business.Statics;
using Business.Repository.Interface;
using Business.Statics;
using Infrastructure.DTOs;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Business.Repository.Imp
{
    public class Logic : ILogic
    {
        private readonly IValidator _validator;

        public Logic(IValidator validator)
        {
            _validator = validator;
        }
        public void CreateFiles(List<UserInfo> users)
        {
            var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "Out.json");
            string jsonString = JsonSerializer.Serialize(users);
            System.IO.File.WriteAllText(FilePath, jsonString);
        }

        public string[] InputReder()
        {
            var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "text.txt");
            string[] lines;
            using (StreamReader streamReader = new StreamReader(FilePath, Encoding.UTF8))
            {
                var Reader = streamReader.ReadToEnd();
                lines = Reader.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            }
            return lines;
        }

        public InputDetail? ValidUser(string lines)
        {
            var User = SplitLines.SplitLine(lines);
            // Validate Inpute
            if (User.Count() != 4)
            {
                return null;
            }
            var Id = _validator.ValidId(User[0]);
            if (Id == null)
            {
                return null;
            }
            var IsNameVaildate = _validator.ValidNname(User[1]);
            if (!IsNameVaildate)
            {
                return null;
            }
            var Date = _validator.ValidDate(User[2]);
            if (Date == null)
            {
                return null;
            }
            var Time = _validator.ValidTime(User[3]);
            if (Time == null)
            {
                return null;
            }
            var UserDetail = new InputDetail()
            {
                Id = Id.Value,
                Name = User[1],
                Date = Date.Value,
                Time = Time.Value
            };
            return UserDetail;
        }

        public bool UploadFiles(IFormFile formfile)
        {
            var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "text.txt");
            bool IsValid = formfile.ContentType.Equals(MediaTypeNames.Text.Plain);
            if (IsValid)
            {
                using (var stream = new FileStream(FilePath, FileMode.Create))
                {
                    formfile.CopyTo(stream);
                }
                return true;
            }
            return false;

        }

        public (List<InputDetail> UsersDetail, Dictionary<int, string> UsersId, int ErrorLine) UserDetailsOfLines(string[] lines)
        {
            var UserIds = new Dictionary<int, string>();
            var UserDetail = new List<InputDetail>();
            for (int i = 0; i < lines.Count(); i++)
            {
                var User = ValidUser(lines[i]);
                if (User == null)
                {
                    return (null, null, i + 1);
                }
                UserDetail.Add(User);
                UserIds.TryAdd(User.Id, User.Name);
            };
            return (UserDetail, UserIds, 0);
        }

        public List<type> GetStatus(int recordsCount, TimeSpan firstIn, TimeSpan lastOut, TimeSpan workTime)
        {
            var Status = new List<type>();

            // Erorr
            if (recordsCount % 2 != 0)
            {
                Status.Add(type.error);
                return Status;
            }
            // True status
            if (firstIn > LawOfTime.MaxOfIn)
            {
                Status.Add(type.delay);
            }
            if (lastOut < LawOfTime.MinOfOut)
            {
                Status.Add(type.hourly_leave);
            }
            else if (workTime >= LawOfTime.StandarTime)
            {
                if (recordsCount > 2)
                {
                    Status.Add(type.hourly_leave);
                }
                else
                {
                    Status.Add(type.Normal);
                }
            }
            else
            {
                Status.Add(type.hourly_leave);
            }
            return Status;
        }

        public TimerDto GetTimes(IGrouping<(int id, DateTime Date), InputDetail> user)
        {
            var In = user.First().Time;
            var Out = user.First().Time;
            var WorkTime = TimeSpan.Zero;

            for (int i = 1; i < user.Count(); i++)
            {
                var Person = user.ElementAt(i);

                // Even For Arrive In
                if (i % 2 == 0)
                {
                    In = Person.Time;
                }

                // Odd For Exit ( Calcute Worktime ) Out - In
                if (i % 2 != 0)
                {
                    if (Person.Time > LawOfTime.MinOfIn)
                    {
                        if (In < LawOfTime.MinOfIn)
                        {
                            In = LawOfTime.MinOfIn;
                        }
                        if (Person.Time > LawOfTime.MaxOfOut)
                        {
                            Out = LawOfTime.MaxOfOut;
                        }
                        else
                        {
                            Out = Person.Time;
                        }
                    }
                    else
                    {
                        Out = In;
                    }
                    WorkTime += Out - In;

                }
            }

            var Times = new TimerDto()
            {
                FirstIn = user.First().Time,
                LastOut = Out,
                WorkTime = WorkTime
            };

            return Times;




        }

        public List<string> GetRecords(IGrouping<(int id, DateTime Date), InputDetail> user)
        {
            var Recoreds = new List<string>();
            for (int i = 0; i < user.Count(); i++)
            {
                Recoreds.Add(user.ElementAt(i).Time.ToString());
            }
            return Recoreds;

        }


        public UserInfo CreateUserInfo(IGrouping<(int id, DateTime Date), InputDetail> user, TimerDto times, List<string> records, int row, List<type> status)
        {
            var UserInfo = new UserInfo();
            UserInfo.Id = user.First().Id;
            UserInfo.Day = ShamsiDayOfWeek.GetDayShamsi(user.Key.Date.DayOfWeek.ToString());
            UserInfo.Name = user.First().Name;
            UserInfo.FirstArrive = times.FirstIn;
            UserInfo.LastCheckout = times.LastOut;
            UserInfo.Record.AddRange(records);
            UserInfo.row = row;
            UserInfo.WorkTime = times.WorkTime;
            UserInfo.Status.AddRange(status);
            UserInfo.DateTime = user.Key.Date;
            return UserInfo;
        }


        public AbsenceDto CheckAbsence(int totalLine,
            IGrouping<(int id, DateTime Date), InputDetail> userNext
            , int lineCounter, DateTime currentDate, Dictionary<int, string> absence, Dictionary<int, string> usersId, int row, bool lastUser)
        {
            var ListUsersInfo = new List<UserInfo>();

            if (lastUser == true || currentDate != userNext.Key.Date)
            {
                var AbsenceCount = absence.Count;

                for (int j = 0; j < AbsenceCount; j++)
                {
                    UserInfo User = new UserInfo();
                    User.Id = absence.ElementAt(j).Key;
                    User.Day = ShamsiDayOfWeek.GetDayShamsi(currentDate.Date.DayOfWeek.ToString());
                    User.Name = absence.ElementAt(j).Value;
                    User.FirstArrive = LawOfTime.ZeroTime;
                    User.LastCheckout = LawOfTime.ZeroTime;
                    User.Record.Add("0:0");
                    User.row = row++;
                    User.WorkTime = LawOfTime.ZeroTime;
                    User.Status.Add(type.daily_leave);
                    User.DateTime = currentDate.Date;
                    ListUsersInfo.Add(User);
                }
                absence.Clear();
                absence = new Dictionary<int, string>(usersId);
                if (lineCounter + 1 <= totalLine)
                {
                    currentDate = userNext.Key.Date;
                }
                return new AbsenceDto()
                {
                    Absence = absence,
                    CurrentDate = currentDate,
                    ListUserInfo = ListUsersInfo,
                    Row = row,
                };
                // Date Have Chaneged Absence mush Write
            }
            return null;
        }

        public List<UserInfo> SaveUserInOut(IEnumerable<IGrouping<(int id, DateTime Date), InputDetail>> UserDetails, DateTime CurrentDate, Dictionary<int,string> UserId)
        {

            var UsersInfos = new List<UserInfo>();
            var Absence = new Dictionary<int, string>(UserId);
            int Row = 0;
            int TotalLine = UserDetails.Count();

            for (int ii = 0; ii < TotalLine; ii++)
            {
                var User = UserDetails.ElementAt(ii);
                Row++;

                if (CurrentDate == User.Key.Date)
                {
                    Absence.Remove(User.First().Id);
                }
                if (ii + 1 <= TotalLine)
                {
                    var AbsencsDtos = new AbsenceDto();
                    if (ii + 1 == TotalLine)
                    {
                        // Last Day Absense
                        AbsencsDtos = CheckAbsence(TotalLine, UserDetails.ElementAt(ii), ii, CurrentDate, Absence, UserId, Row, true);
                    }
                    else
                    {
                        AbsencsDtos = CheckAbsence(TotalLine, UserDetails.ElementAt(ii + 1), ii, CurrentDate, Absence, UserId, Row, false);
                    }
                    if (AbsencsDtos != null)
                    {
                        CurrentDate = AbsencsDtos.CurrentDate;
                        Absence = AbsencsDtos.Absence;
                        UsersInfos.AddRange(AbsencsDtos.ListUserInfo);
                        Row = AbsencsDtos.Row;
                    }
                }
                var Records = GetRecords(User);
                var Times = GetTimes(User);
                var Status = GetStatus(Records.Count, Times.FirstIn, Times.LastOut, Times.WorkTime);
                var UserInfos = CreateUserInfo(User, Times, Records, Row, Status);
                UsersInfos.Add(UserInfos);
            }
            return UsersInfos;

        }






    }
}
