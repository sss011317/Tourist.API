using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tourist.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Address { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public ICollection<Order> Orders { get; set; }
        //綁定4張表的對應關係，使用virtual重載4張表，從程式碼的角度建立模型關係
        //UserRoles用戶角色
        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; }
        //Claims用戶權限申明
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
        //Logins用戶的第三方登入訊息
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
        //Tokens用戶登入的session
        public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; }
    }
}
