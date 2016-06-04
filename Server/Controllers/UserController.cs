using Server.Models;
using Server.Service;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Server.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        /// <summary>
        ///
        /// </summary>
        private readonly UserService _userService;

        /// <summary>
        /// 
        /// </summary>
        public UserController()
        {
            _userService = new UserService();
        }

        [HttpGet]
        [Route("get")]
        public IHttpActionResult GetUsers() 
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> CreateUser(UserModel userModel)
        {
            if (ModelState.IsValid && userModel != null)
            {
                try
                {
                    var createdUser = await _userService.RegisterUser(userModel);
                    return Ok(createdUser);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest("El usuario, email y password son campos requeridos.");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IHttpActionResult> UpdateUser(UserModel userModel)
        {
            if (ModelState.IsValid && userModel != null)
            {
                try
                {
                    await _userService.UpdateUser(userModel);
                    return Ok(new { success = "usuario actualizado con exito." });
                }
                catch (ApplicationException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest("El usuario, email y password son campos requeridos.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("delete")]
        public async Task<IHttpActionResult> DeleteUser(UserModel userModel) 
        {
            if (ModelState.IsValid && userModel != null) 
            {
                try
                {
                    await _userService.DeleteUser(userModel);
                    return Ok(new { success = "usuario eliminado con exito." });
                }
                catch (ApplicationException ex) 
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest("La informacion del usuario es requerida.");
        }
    }
}