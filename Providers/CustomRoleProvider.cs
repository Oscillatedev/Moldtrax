using Moldtrax.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Moldtrax.Providers
{
    public class CustomRoleProvider : RoleProvider
    {
        private MoldtraxDbContext dataContext;

        protected MoldtraxDbContext DbContext
        {
            get
            {
                return dataContext ?? (dataContext = new MoldtraxDbContext());
            }
        }

        //public override bool IsUserInRole(string username, string roleName)
        //{
        //    var user = DbContext.Ezy_Users.SingleOrDefault(u => u.UserID == username);
        //    if (user == null)
        //        return false;
        //    //var roleInfo = DbContext.Roles.Where(x => x.Name == roleName);
        //    var roleInfo = DbContext..Roles.Find(user.AuthorizationType);
        //    if (roleInfo == null)
        //        return false;
        //    return (roleInfo.Name == roleName);
        //}

        public override string[] GetRolesForUser(string username)
        {
            var user = DbContext.Ezy_Users.Where(x => x.UserID == username).FirstOrDefault();
            if (user == null)
            {
                return new string[] { };
            }
            else
            {
                ////var roles = DbContext.UserRoles.Where(x => x.UserId == user.ID).Select(x => x.RoleId).ToArray();
                //var selectedrole = (from role in dataContext.Roles where roles.Contains(role.ID) select role);
                //var roletoUser = selectedrole.Select(x => x.Name).ToArray();
                var selectedrole = DbContext.Roles.Where(x => x.ID == user.AuthorizationType).Select(x => x.Name).ToArray();
                return selectedrole;
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {

            return DbContext.Roles.Select(r => r.Name).ToArray();
        }
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }
        public override string ApplicationName { get; set; }

    }
}