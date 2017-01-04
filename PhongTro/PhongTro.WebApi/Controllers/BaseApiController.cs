using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PhongTro.Domain.Infracstucture;
using PhongTro.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace PhongTro.WebApi.Controllers
{
    /// <summary>
    /// Base class for controllers 
    /// </summary>
    public class BaseApiController : ApiController
    {
        private IRepository _repository;

        protected IRepository _Repository
        {
            get
            {
                return _repository;
            }
            set
            {
                _repository = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="repo">The implement of IRepository interface</param>
        public BaseApiController(IRepository repo)
        {
            _Repository = repo;
        }

        /// <summary>
        /// Determine the type of result
        /// </summary>
        /// <param name="result">Object contains the result</param>
        /// <returns>An IHttpActionResult object</returns>
        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}