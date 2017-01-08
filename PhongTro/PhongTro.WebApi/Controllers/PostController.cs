using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PhongTro.Model.Core;
using System.Web.Http;
using PhongTro.Model.DTOs;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace PhongTro.WebApi.Controllers
{
    /// <summary>
    /// Controller manages Posts and their Comments
    /// </summary>
    [RoutePrefix("api/posts")]
    public class PostController : BaseApiController
    {
        public PostController(IRepository repo) : base(repo) { }

        /// <summary>
        /// Action get all post from database
        /// </summary>
        /// <returns>IHttpResult with list of PostDTO objetcs </returns>
        [AllowAnonymous]
        [Route("")]
        public IHttpActionResult GetAllPosts()
        {
            var posts = _Repository.GetAllPosts();

            return Ok(posts);
        }

        [AllowAnonymous]
        [Route("{pageIndex:int}/{pageSize:int}")]
        public IHttpActionResult GetPosts(int PageIndex, int PageSize)
        {
            var posts = _Repository.GetPosts(PageIndex, PageSize);

            return Ok(posts);
        }

        /// <summary>
        /// Get a post after searching its identifier
        /// </summary>
        /// <param name="Id">Identifier of the post to be searched</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("{id:guid}", Name = "GetPostById")]
        public async Task<IHttpActionResult> GetPost(string Id)
        {
            var post = await _Repository.GetPostById(Id);

            if (null != post)
            {
                return Ok(post);
            }

            return NotFound();
        }

        /// <summary>
        /// Get all the comments of a post
        /// </summary>
        /// <param name="PostId">Identifier of the post</param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("{postId:guid}/comments")]
        public async Task<IHttpActionResult> GetAllCommentsByPost(string PostId)
        {
            var post = await _Repository.GetPostById(PostId);

            if (null == post)
            {
                return NotFound();
            }

            var comments = _Repository.GetAllCommentsByPost(PostId);

            return Ok(comments);

        }

        /// <summary>
        /// Get comments with paging
        /// </summary>
        /// <param name="PostId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("{postId:guid}/comments/{pageIndex:int}/{pageSize:int}")]
        public async Task<IHttpActionResult> GetCommentsByPost(string PostId, int PageIndex, int PageSize)
        {
            var post = await _Repository.GetPostById(PostId);

            if (null == post)
            {
                return NotFound();
            }

            var comments = _Repository.GetComments(PostId, PageIndex, PageSize);

            return Ok(comments);

        }

        /// <summary>
        /// Rate a post with a value
        /// </summary>
        /// <param name="PostId">Identifier of the post</param>
        /// <param name="value">Rating value</param>
        /// <returns></returns>
        [Authorize(Roles = "Lodger")]
        [Route("{postId:guid}/rates")]
        [HttpPost]
        public async Task<IHttpActionResult> RatePost([FromUri] string PostId, [FromBody] float value)
        {
            if (value < 0 || (value > 10))
            {
                return BadRequest("Value must be in range of 1 to 10");
            }

            var post = await _Repository.GetPostById(PostId);

            if (null == post)
            {
                return NotFound();
            }

            var result = await _Repository.RatePost(PostId, value);
            if (null != result)
            {
                return Ok(result);
            }

            return InternalServerError();
        }

        /// <summary>
        /// Add a comment to a post
        /// </summary>
        /// <param name="PostId">Identifier of the post</param>
        /// <param name="model">Object contains comment data</param>
        /// <returns></returns>
        [Authorize(Roles = "Lodger")]
        [Route("{postId:guid}/comments")]
        public async Task<IHttpActionResult> CreateComment([FromUri] string PostId, [FromBody] CreatingCommentDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            model.UserId = User.Identity.GetUserId();
            model.PostId = PostId;
            var commentResult = await _Repository.CreateComment(model);

            if (null == commentResult)
            {
                return InternalServerError();
            }

            return Created("", commentResult);
        }

        /// <summary>
        /// Delete a comment of a user on a specific post
        /// </summary>
        /// <param name="PostId">Identifier of the post</param>
        /// <param name="CommentId">Identifier of the comment</param>
        /// <returns>An IHtppActionResult</returns>
        [Authorize(Roles = "Lodger")]
        [Route("{postId:guid}/comments/{commentId:guid}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteComment(string PostId, string CommentId)
        {
            var userId = User.Identity.GetUserId();
            bool check = await _Repository.CheckCommentOwner(userId, CommentId);

            if (false == check)
            {
                return NotFound();
            }

            var result = await _Repository.DeleteComment(CommentId);

            if (true == result)
            {
                return Ok();
            }

            return InternalServerError();

        }

        /// <summary>
        /// Get all posts owned by a user
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Landlord")]
        [Route("ownposts")]
        public IHttpActionResult GetPostByUser()
        {
            var id = User.Identity.GetUserId();
            var posts = _Repository.GetPostsByUser(id);

            return Ok(posts);
        }

        /// <summary>
        /// Get all posts owned by a user
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Landlord")]
        [Route("ownposts/{pageIndex:int}/{pageSize:int}")]
        public IHttpActionResult GetPostByUser(int PageIndex, int PageSize)
        {
            var id = User.Identity.GetUserId();
            var posts = _Repository.GetPostsByUser(id, PageIndex, PageSize);

            return Ok(posts);
        }

        /// <summary>
        /// Create a new post
        /// </summary>
        /// <param name="model">Object contains post data</param>
        /// <returns>An IHttpActionResult</returns>
        [Authorize(Roles = "Landlord")]
        [Route("ownposts")]
        public async Task<IHttpActionResult> CreatePost(CreatingPostDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            model.PhongTroUserID = User.Identity.GetUserId();
            var resultPost = await _Repository.CreatePost(model);

            if (null == resultPost)
            {
                return InternalServerError();
            }

            Uri locationHeader = new Uri(Url.Link("GetPostById", new { id = resultPost.Id }));

            return Created(locationHeader, resultPost);
        }

        /// <summary>
        /// Update an existing post
        /// </summary>
        /// <param name="PostId">Identifier of the post</param>
        /// <param name="model">Contains post data</param>
        /// <returns>An IHttpActionResult object</returns>
        [Authorize(Roles = "Landlord")]
        [Route("ownposts/{postId:guid}")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdatePost([FromUri] string PostId, [FromBody] CreatingPostDTO model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var post = await _Repository.GetPostOfUserById(userId, PostId);

            if (null == post)
            {
                return NotFound();
            }

            var resultPost = await _Repository.UpdatePost(PostId, model);

            if (null == resultPost)
            {
                return InternalServerError();
            }

            return Ok(resultPost);
        }

        /// <summary>
        /// Delete a post owned by a user
        /// </summary>
        /// <param name="Id">Identifier of the post to be deleted</param>
        /// <returns></returns>
        [Authorize(Roles = "Landlord")]
        [Route("ownposts/{id:guid}")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeletePost(string Id)
        {
            var userId = User.Identity.GetUserId();
            var post = _Repository.GetPostOfUserById(userId, Id);

            if (null == post)
            {
                return NotFound();
            }

            bool result = await _Repository.DeletePost(Id);

            if (true == result)
            {
                return Ok();
            }

            return InternalServerError();
        }
    }
}