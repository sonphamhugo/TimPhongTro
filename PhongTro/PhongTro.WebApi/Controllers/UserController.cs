﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PhongTro.Domain.Entities;
using PhongTro.Domain.Infracstucture;
using PhongTro.Model;
using PhongTro.Model.Core;
using PhongTro.Model.DTOs;
using PhongTro.WebApi.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PhongTro.WebApi.Controllers
{
    /// <summary>
    /// Controller is responsible for manage App's accounts
    /// </summary>
    [RoutePrefix("api/users")]
    public class UserController : BaseApiController
    {
        #region Constants
        const string GoogleTokenType = "urn:google:accesstoken";
        const string GoolgeProfileUri = "https://www.googleapis.com/oauth2/v2/userinfo?access_token=";
        const string GoogleCallbackUri = "/api/users/googlecallback";
        #endregion

        public UserController(IRepository repo) : base(repo) { }

        /// <summary>
        /// Callback method when login using Google authentication
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("googlecallback")]
        public async Task<IHttpActionResult> GoogleCallback()
        {
            var autheticationManager = HttpContext.Current.GetOwinContext().Authentication;

            //get access token to use in profile image request
            var accessToken = autheticationManager.User.Claims.Where(c => c.Type.Equals(GoogleTokenType)).Select(c => c.Value).FirstOrDefault();
            Uri apiRequestUri = new Uri(GoolgeProfileUri + accessToken);

            //request profile image
            var webClient = new System.Net.WebClient();
            var userData = webClient.DownloadString(apiRequestUri);
            dynamic result = JsonConvert.DeserializeObject(userData);
            var userEmail = result.email;

            // find user who registered with this email
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<PhongTroUserManager>();
            PhongTroUser user = await userManager.FindByNameAsync(Convert.ToString(userEmail));

            if (null != user) // if this Google account has been register to PhongTro
            {
                var response = await GenerateLocalAccessToken(user);
                return Ok(response);
            }
            else // return Google profile for registering new account
            {
                return Ok(userData);
            }
        }

        /// <summary>
        /// Login with Google account
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("google")]
        public IHttpActionResult ExternalLogin()
        {
            return new ChallengeResult("Google", GoogleCallbackUri, this.Request);
        }

        /// <summary>
        /// Log out the website when loging in using social account
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("logout")]
        public IHttpActionResult Logout()
        {
            var autheticationManager = HttpContext.Current.GetOwinContext().Authentication;
            autheticationManager.SignOut();
            return Ok();
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>
        /// Success: OkResult with a list of all users in form of RegisteringUserDTO.
        /// Fail: NotFoundResult.
        /// </returns>
        [Authorize(Roles = "Admin")]
        [Route("")]
        public IHttpActionResult GetUsers()
        {
            return Ok(_Repository.GetAllUsers());
        }

        /// <summary>
        /// Get user's data when successfully logging in
        /// </summary>
        /// <returns>User's data in form of a UserDTO</returns>
        [Authorize]
        [Route("info")]
        public async Task<IHttpActionResult> GetUsersInfo()
        {
            string username = User.Identity.Name;
            var user = await _Repository.FindUserByUserName(username);

            if (user != null)
            {
                return Ok(user);
            }

            return NotFound();
        }

        /// <summary>
        /// Get User by Identifier filter 
        /// </summary>
        /// <param name="Id">The identifier</param>
        /// <returns>
        /// IHttpActionResult (contains User in form of UserDTO if it is found)
        /// </returns>
        [Authorize(Roles = "Admin")]
        [Route("{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var user = await _Repository.FindUserById(Id);

            if (user != null)
            {
                return Ok(user);
            }

            return NotFound();
        }

        /// <summary>
        /// Get user data by a username
        /// </summary>
        /// <param name="username">Username of the user to be found</param>
        /// <returns>
        /// IHttpResult (contains User in form of UserDTO if it is found)
        /// </returns>
        [Authorize(Roles= "Admin")]
        [Route("{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await _Repository.FindUserByUserName(username);

            if (user != null)
            {
                return Ok(user);
            }

            return NotFound();

        }
        
        /// <summary>
        /// Action used to create new user
        /// </summary>
        /// <param name="registeringUser">Param contains user information for registration</param>
        /// <returns>An IHttpActionResult comes with user info in form of UserDTO</returns>
        [Route("")]
        public async Task<IHttpActionResult> CreateUser(RegisteringUserDTO registeringUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            Tuple<IdentityResult, UserDTO> addUserResult = await _Repository.CreateUser(registeringUser);
            var identityResult = addUserResult.Item1;

            if (!identityResult.Succeeded)
            {
                return GetErrorResult(identityResult);
            }

            var resultUser = addUserResult.Item2;
            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = resultUser.Id }));

            return Created(locationHeader, resultUser);
        }

        /// <summary>
        /// Action used by authentication user to change her password
        /// </summary>
        /// <param name="model">Param contains old password and the new one</param>
        /// <returns>
        /// An IHttpActionResult object
        /// </returns>
        [Authorize]
        [Route("passwords")]
        public async Task<IHttpActionResult> ChangePassword(ChangingPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await _Repository.ChangePassword(User.Identity.GetUserId(), model);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        /// <summary>
        /// Action is used to delete a user
        /// </summary>
        /// <param name="id">Identifier of user</param>
        /// <returns>
        /// IHttpActionResult object
        /// </returns>
        [Authorize(Roles="Admin")]
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            var appUser = await _Repository.FindUserById(id);

            if (appUser != null)
            {
                IdentityResult result = await _Repository.DeleteUser(id);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                return Ok();

            }

            return NotFound();

        }
        
        /// <summary>
        /// Action is used to assign roles to a specific user
        /// </summary>
        /// <param name="id">Identifier of the user</param>
        /// <param name="rolesToAssign">Roles to be assigned to the user</param>
        /// <returns>
        /// IHttpActionResult object
        /// </returns>
        [Authorize(Roles = "Admin")]
        [Route("{id:guid}/roles")]
        [HttpPut]
        public async Task<IHttpActionResult> AssignRolesToUser([FromUri] string id, [FromBody] string[] rolesToAssign)
        {

            var appUser = await _Repository.FindUserById(id);

            if (appUser == null)
            {
                return NotFound();
            }

            // Check if roles which will be assigned are exist in membership system.
            // If they does, return BadRequestResult.
            var rolesNotExists = _Repository.GetRolesNotExist(rolesToAssign);
            if (rolesNotExists.Count() > 0)
            {

                ModelState.AddModelError("", string.Format("Roles '{0}' does not exixts in the system", string.Join(",", rolesNotExists)));
                return BadRequest(ModelState);
            }

            // Remove all roles assigned to the user before
            IdentityResult removeResult = await _Repository.RemoveAllRoles(appUser.Id);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove user roles");
                return BadRequest(ModelState);
            }

            // Add new roles to the user
            IdentityResult addResult = await _Repository.AddRolesToUser(appUser.Id, rolesToAssign);
            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to add user roles");
                return BadRequest(ModelState);
            }

            return Ok();
        }


    }
}
