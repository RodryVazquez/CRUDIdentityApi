using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Service
{
    /// <summary>
    ///
    /// </summary>
    public class UserService
    {
        /// <summary>
        /// Usermanager Identity
        /// </summary>
        private UserManager<ApplicationUser> _userManager;

        /// <summary>
        ///
        /// </summary>
        public UserService()
        {
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        }

        /// <summary>
        /// Consulta a todos los usuarios
        /// </summary>
        /// <returns></returns>
        public IQueryable<UserModel> GetAllUsers()
        {
            return _userManager.Users
                   .Where(r => r.Active)
                   .Select(p => new UserModel()
                                {
                                     UserId = p.Id,
                                     UserName = p.UserName,
                                     Email = p.Email,
                                     FirstName = p.FirstName,
                                     LastName = p.LastName,
                                     LastName2 = p.LastName2
                                });
        }

        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public async Task<UserModel> RegisterUser(UserModel userModel)
        {
            var user = new ApplicationUser()
            {
                UserName = userModel.UserName,
                Email = userModel.Email,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                LastName2 = userModel.LastName2,
                Active = true
            };
            var result = await _userManager.CreateAsync(user, userModel.Password);
            if (!result.Succeeded)
            {
                ManageIdentityErrors(result);
            }
            var createdUser = await _userManager.FindByEmailAsync(userModel.Email);

            return new UserModel()
            {
                UserName = createdUser.UserName,
                Email = createdUser.Email,
                UserId = createdUser.Id,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                LastName2 = createdUser.LastName2
            };
        }

        /// <summary>
        /// Actualizamos un usuario existente
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public async Task UpdateUser(UserModel userModel)
        {
            var user = await _userManager.FindByIdAsync(userModel.UserId);
            if (user == null)
            {
                throw new ApplicationException("El usuario que intentas actualizar no existe.");
            }
            user.UserName = userModel.UserName;
            user.Email = userModel.Email;
            user.FirstName = userModel.FirstName;
            user.LastName = userModel.LastName;
            user.LastName2 = userModel.LastName2;

            //si necesita cambiar el password
            if (userModel.IsChangePassword) 
            {
                var resultPass = await _userManager.ChangePasswordAsync(user.Id, userModel.Password, userModel.NewPassword);
                if (!resultPass.Succeeded)
                {
                    ManageIdentityErrors(resultPass);
                } 
            }
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) 
            {
                ManageIdentityErrors(result);
            }
        }

        /// <summary>
        /// Eliminamos un usuario
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public async Task DeleteUser(UserModel userModel) 
        {
            var user = await _userManager.FindByIdAsync(userModel.UserId);
            if (user == null) 
            {
                throw new ApplicationException("El usuario que intentas eliminar no existe.");
            }

            user.Active = false;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) 
            {
                ManageIdentityErrors(result);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void ManageIdentityErrors( IdentityResult result) 
        {
            List<string> errors = new List<string>();
            foreach (var error in result.Errors)
            {
                errors.Add(error);
            }
            var messageError = String.Join(", ", errors);
            throw new ApplicationException(messageError);
        }
    }
}



/*
 UPDATE USER
 * Without change password
   {
      "UserId": "7181eba2-80d5-4380-a106-e62297255357",
      "UserName": "Rodrigo.Vazquez",
      "Email": "xxxxxxx",
      "FirstName": "Rodrigo Aaron",
      "LastName": "Vazquez",
      "LastName2": "Vazquez",
      "IsChangePassword": false
   }
 * With change password
   {
      "UserId": "7181eba2-80d5-4380-a106-e62297255357",
      "UserName": "Rodrigo.Vazquez",
      "Email": "rodrigo@rodrigovazquez.com.mx",
      "FirstName": "Luis Carlos",
      "LastName": "Trejo",
      "LastName2": "Andazola",
      "Password":123456,
      "NewPassword":123456789,
      "IsChangePassword": true
   } 
 
  
 *DELETE USER
  {
    "UserId": "7181eba2-80d5-4380-a106-e62297255357"
  }
  
  
 *REGISTER USER
  {
     "UserName": "Rodrigo.Vazquez",
     "Email": "rodrigo@rodrigovazquez.com.mx",
     "FirstName": "Rodrigo Aaron",
     "LastName": "Vazquez",
     "LastName2": "Vazquez",
     "Password":123456
  }
  Response
  {
    "UserId": "be084c4e-7233-44f1-ba43-c890ed55812e"
    "UserName": "Rodrigo.Vazquez"
    "Password": null
    "NewPassword": null
    "Email": "rodrigo@rodrigovazquez.com.mx"
    "FirstName": "Rodrigo Aaron"
    "LastName": "Vazquez"
    "LastName2": "Vazquez"
    "IsChangePassword": false
  } 
 */