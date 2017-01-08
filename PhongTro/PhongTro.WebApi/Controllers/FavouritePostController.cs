using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using PhongTro.Model.Core;
using System.Threading.Tasks;

namespace PhongTro.WebApi.Controllers
{
    /// <summary>
    /// Controller used to manage a user's favourite posts
    /// </summary>
    [RoutePrefix("api/posts/favourites")]
    public class FavouritePostController : BaseApiController
    {
        public FavouritePostController(IRepository repo) : base(repo) { }

        /// <summary>
        /// Get al favourite posts of a user
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Lodger")]
        [Route("")]
        public IHttpActionResult GetAllFavouritePosts()
        {
            var id = User.Identity.GetUserId();
            var posts = _Repository.GetFavouritePostsByUser(id);

            return Ok(posts);
        }

        /// <summary>
        /// Get favourite posts with paging
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Lodger")]
        [Route("{pageIndex:int}/{pageSize:int}")]
        public IHttpActionResult GetFavouritePosts(int PageIndex, int PageSize)
        {
            var id = User.Identity.GetUserId();
            var posts = _Repository.GetFavouritePosts(id, PageIndex, PageSize);

            return Ok(posts);
        }

        /// <summary>
        /// Add a post to user's favourite list
        /// </summary>
        /// <param name="postId">Identifier of the post</param>
        /// <returns></returns>
        [Authorize(Roles = "Lodger")]
        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> AddFavouritePost([FromBody] string postId)
        {
            var post = _Repository.GetPostById(postId);
            if (null == post)
            {
                return NotFound();
            }

            var userId = User.Identity.GetUserId();
            bool result = await _Repository.AddFavouritePost(userId, postId);
            if (true == result)
            {
                return Ok();
            }

            return InternalServerError();
        }

        /// <summary>
        /// Remove a post from user's favourite list
        /// </summary>
        /// <param name="Id">Identifier of the post</param>
        /// <returns></returns>
        [Authorize(Roles = "Lodger")]
        [Route("{id:guid}")]
        [HttpDelete]
        public async Task<IHttpActionResult> RemoveFavouritePost(string Id)
        {
            var post = _Repository.GetPostById(Id);
            if (null == post)
            {
                return NotFound();
            }

            var userId = User.Identity.GetUserId();
            bool result = await _Repository.RemoveFavouritePost(userId, Id);
            if (true == result)
            {
                return Ok();
            }

            return InternalServerError();
        }
    }
}