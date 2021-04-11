using Swetugg.Tix.User.Contract;
using System;
using System.Collections.Generic;

namespace Swetugg.Tix.User
{
    public interface IUserWithAuth
    {
        UserInfo UserInfo { get; }
        Guid? UserId { get; }
        bool HasPermissionForObject(string permissionName, object objectToAuthorize);
    }

    public class NonAuthorizedUser : IUserWithAuth
    {
        private readonly UserInfo _userInfo;


        public NonAuthorizedUser(UserInfo userInfo)
        {
            _userInfo = userInfo;
        }

        public UserInfo UserInfo => _userInfo;

        public Guid? UserId => null;

        public bool HasPermissionForObject(string permissionName, object objectToAuthorize)
        {
            return false;
        }
    }

    public class AuthorizedUser: IUserWithAuth
    {
        private ObjectAuthorizationManager _objectAuthorizationManager;
        private UserInfo _userInfo;

        public AuthorizedUser(UserInfo userInfo, IEnumerable<PermissionClaim> claims)
        {
            _objectAuthorizationManager = new ObjectAuthorizationManager(claims);
            _userInfo = userInfo;
        }

        public UserInfo UserInfo => _userInfo;
        public Guid? UserId => _userInfo.UserId.Value;

        public bool HasPermissionForObject(string permissionName, object objectToAuthorize)
        {
            return _objectAuthorizationManager.HasPermissionForObject(permissionName, objectToAuthorize);
        }
    }
}
