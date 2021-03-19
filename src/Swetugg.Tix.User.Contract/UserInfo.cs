using System;

namespace Swetugg.Tix.User.Contract
{
    public enum UserStatus
    {
        None = 0,
        Created = 1,
        Validated = 2,
        Deleted = 100
    }

    public class UserInfo
    {
        public Guid? UserId { get; set; }
        public UserStatus Status { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string IssuerIdentifier { get; set; }
    }


}
