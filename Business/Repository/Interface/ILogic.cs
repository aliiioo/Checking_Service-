using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
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


    }
}
