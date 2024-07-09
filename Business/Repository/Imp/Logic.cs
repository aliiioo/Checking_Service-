using business.Repository.Interface;
using business.Statics;
using Business.Repository.Interface;
using Business.Statics;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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

    }
}
