using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PhongTro.Domain.Infracstucture;
using PhongTro.Model.Core;
using PhongTro.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PhongTro.WebApi.Controllers
{
    /// <summary>
    /// Class allows the administrator to manage roles in membership system
    /// </summary>
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/roles")]
    public class RolesController : BaseApiController
    {
        public RolesController(IRepository repo) : base(repo) { }

        /// <summary>
        /// Get a role by its identifier
        /// </summary>
        /// <param name="Id">The identifier of role</param>
        /// <returns>A RoleDTO object</returns>
        [Route("{id:guid}", Name = "GetRoleById")]
        public async Task<IHttpActionResult> GetRole(string Id)
        {
            var role = await _Repository.FindRole(Id);

            if (role != null)
            {
                return Ok(role);
            }

            return NotFound();

        }

        /// <summary>
        /// Get all role in membership system
        /// </summary>
        /// <returns>
        /// An IdentityRole list
        /// </returns>
        [Route("", Name = "GetAllRoles")]
        public IHttpActionResult GetAllRoles()
        {
            var roles = _Repository.GetAllRoles();

            return Ok(roles);
        }

        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="model">A CreatingRole model</param>
        /// <returns></returns>
        [Route("")]
        public async Task<IHttpActionResult> Create(CreatingRoleDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _Repository.CreateRole(model);
            var identityResult = result.Item1;

            if (!identityResult.Succeeded)
            {
                return GetErrorResult(identityResult);
            }

            var role = result.Item2;
            Uri locationHeader = new Uri(Url.Link("GetRoleById", new { id = role.Id }));

            return Created(locationHeader, role);

        }

        /// <summary>
        /// Delete a role by supplying its identifier
        /// </summary>
        /// <param name="Id">The role's identifier</param>
        /// <returns>
        /// Success: OkResult
        /// Fail: BadRequestResult comes with Error content
        /// </returns>
        [Route("{id:guid}")]
        public async Task<IHttpActionResult> DeleteRole(string Id)
        {

            var role = await _Repository.FindRole(Id);

            if (role != null)
            {
                IdentityResult result = await _Repository.DeleteRole(Id);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                return Ok();
            }

            return NotFound();

        }
    }
}