using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhongTro.Model.DTOs;
using PhongTro.Domain.Infracstucture;
using PhongTro.Domain.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace PhongTro.Model.Core
{
    /// <summary>
    /// Class implements IRepository
    /// </summary>
    public class PhongTroRepository : IRepository
    {
        private PhongTroUserManager _userManager;
        private PhongTroRoleManager _roleManager;
        private PhongTroDbContext _dbContext;
        private ModelFactory _modelFactory;

        const bool OK = true;
        const bool Fail = false;

        const string RoleAdmin = "Admin";

        /// <summary>
        /// Default constructor with dependencies which will be inject later
        /// </summary>
        /// <param name="userManager">The application's User Manager</param>
        /// <param name="roleManager">The application's Role Manager</param>
        public PhongTroRepository(PhongTroDbContext dbContext, PhongTroUserManager userManager, PhongTroRoleManager roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _modelFactory = new ModelFactory(_userManager);
        }

        #region User

        public IEnumerable<UserDTO> GetAllUsers()
        {
            return _userManager.Users.ToList().Select(u => _modelFactory.ConvertFromAppUser(u));
        }

        public async Task<UserDTO> FindUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            return null == user ? null : _modelFactory.ConvertFromAppUser(user);
        }

        public async Task<UserDTO> FindUserByUserName(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            return null == user ? null : _modelFactory.ConvertFromAppUser(user);
        }

        public async Task<Tuple<IdentityResult, UserDTO>> CreateUser(RegisteringUserDTO registeringUser)
        {
            var user = _modelFactory.ConvertToAppUser(registeringUser);
            
            IdentityResult identityResult = await _userManager.CreateAsync(user, registeringUser.Password);

            var userResult = new UserDTO();

            if (identityResult.Succeeded)
            {
                var rolesAssigned = registeringUser.Roles.Except(new string[] { RoleAdmin });
                var rolesNotExists = GetRolesNotExist(rolesAssigned.ToArray());

                if ((rolesNotExists.Count() > 0) || (rolesAssigned.Count() == 0))
                {
                    identityResult = new IdentityResult(string.Format("Roles '{0}' does not exixts in the system", string.Join(",", rolesNotExists)));
                    await _userManager.DeleteAsync(user);
                }
                else
                {
                    identityResult = _userManager.AddToRoles(user.Id, rolesAssigned.ToArray());
                    userResult = _modelFactory.ConvertFromAppUser(user);
                }
            }
            
            return new Tuple<IdentityResult, UserDTO>(identityResult, userResult);
        }

        public async Task<IdentityResult> ChangePassword(string id, ChangingPasswordDTO model)
        {
            return await _userManager.ChangePasswordAsync(id, model.OldPassword, model.NewPassword);
        }

        public async Task<IdentityResult> DeleteUser(string id)
        {
            var appUser = await _userManager.FindByIdAsync(id);

            return await _userManager.DeleteAsync(appUser);
        }

        public IEnumerable<string> GetRolesNotExist(string[] roles)
        {
            return roles.Except(_roleManager.Roles.Select(x => x.Name)).ToArray();
        }

        public async Task<IdentityResult> RemoveAllRoles(string userId)
        {
            var currentRoles = await _userManager.GetRolesAsync(userId);
            return await _userManager.RemoveFromRolesAsync(userId, currentRoles.ToArray());
        }

        public async Task<IdentityResult> AddRolesToUser(string userId, string[] rolesToAssign)
        {
            return await _userManager.AddToRolesAsync(userId, rolesToAssign);
        }


        #endregion

        #region Role

        public async Task<RoleDTO> FindRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            
            return role == null? null : _modelFactory.ConvertFromIdentityRole(role);
        }

        public IEnumerable<RoleDTO> GetAllRoles()
        {
            return _roleManager.Roles.ToList().Select(r => _modelFactory.ConvertFromIdentityRole(r));
        }

        public async Task<Tuple<IdentityResult, RoleDTO>> CreateRole(CreatingRoleDTO model)
        {
            var role = new IdentityRole { Name = model.Name };
            var result = await _roleManager.CreateAsync(role);

            return new Tuple<IdentityResult, RoleDTO>(result, _modelFactory.ConvertFromIdentityRole(role));
        }

        public async Task<IdentityResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            return await _roleManager.DeleteAsync(role);
        }

        #endregion

        #region Post

        public IEnumerable<PostDTO> GetAllPosts()
        {
            return _dbContext.Posts.ToList().Select(p => _modelFactory.ConvertToPostDTO(p));
        }

        public async Task<PostDTO> GetPostById(string id)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.PostID.ToString() == id);
            
            return null == post? null : _modelFactory.ConvertToPostDTO(post);
        }

        public async Task<PostDTO> CreatePost(CreatingPostDTO model)
        {
            var user = await _userManager.FindByIdAsync(model.PhongTroUserID);

            if (null != user)
            {
                var post = new Post()
                {
                    PostID = Guid.NewGuid(),
                    Address = model.Address,
                    Price = model.Price,
                    NumberLodgers = model.NumberLodgers,
                    Description = model.Description,
                    PhongTroUserID = user.Id,
                };
                
                try
                {
                    _dbContext.Posts.Add(post);

                    Array.ForEach(model.Images, img =>
                    {
                        _dbContext.BoardingHouseImages.Add(new BoardingHouseImage()
                        {
                            Post = post,
                            Url = img
                        });
                    });
                    
                    _dbContext.SaveChanges();

                    return _modelFactory.ConvertToPostDTO(post);
                }
                catch (Exception e)
                {
                    string content = e.ToString();
                    return null;
                }
            }

            return null;
        }

        public IEnumerable<PostDTO> GetPostsByUser(string id)
        {
            return _dbContext.Posts
                .Where(p => p.PhongTroUserID == id)
                .ToList()
                .Select(p => _modelFactory.ConvertToPostDTO(p));
        }

        public async Task<PostDTO> UpdatePost(string postId, CreatingPostDTO model)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.PostID.ToString() == postId);

            if (null != postId)
            {
                post.Address = model.Address;
                post.Price = model.Price;
                post.NumberLodgers = model.NumberLodgers;
                post.Description = model.Description;
                post.LastUpdate = DateTime.Now;

                try
                {
                    _dbContext.SaveChanges();

                    return _modelFactory.ConvertToPostDTO(post);
                }
                catch (Exception e)
                {
                    return null;
                }
            }

            return null;
        }

        public async Task<PostDTO> GetPostOfUserById(string userId, string postId)
        {
            var post = await _dbContext.Posts
                .FirstOrDefaultAsync(p => p.PhongTroUserID == userId && p.PostID.ToString() == postId);

            return null == post ? null : _modelFactory.ConvertToPostDTO(post);
        }

        public async Task<bool> DeletePost(string postId)
        {
            var post = await _dbContext.Posts
                .FirstOrDefaultAsync(p => p.PostID.ToString() == postId);

            if (postId != null)
            {
                try
                {
                    _dbContext.Posts.Remove(post);
                    _dbContext.SaveChanges();

                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }

            }

            return false;
        }

        public IEnumerable<PostDTO> GetFavouritePostsByUser(string id)
        {
            return _dbContext.FavouritePosts
                            .Include(p => p.Post)
                            .Where(p => p.PhongTroUserID.ToString() == id)
                            .ToList()
                            .Select(p => _modelFactory.ConvertToPostDTO(p.Post));
        }

        public async Task<bool> AddFavouritePost(string userId, string postId)
        {
            var post = await _dbContext.FavouritePosts
                .FirstOrDefaultAsync(p => p.PostID.ToString() == postId && p.PhongTroUserID == userId);

            if (null == post)
            {
                try
                {
                    _dbContext.FavouritePosts.Add(new FavouritePost()
                    {
                        FavouritePostID = Guid.NewGuid(),
                        PhongTroUserID = userId,
                        PostID = Guid.Parse(postId)
                    });
                    _dbContext.SaveChanges();

                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return false;
        }

        public async Task<bool> RemoveFavouritePost(string userId, string postId)
        {
            var post = await _dbContext.FavouritePosts
                .FirstOrDefaultAsync(p => p.PostID.ToString() == postId && p.PhongTroUserID == userId);

            if (null != post)
            {
                try
                {
                    _dbContext.FavouritePosts.Remove(post);
                    _dbContext.SaveChanges();

                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return false;
        }

        public async Task<RateDTO> RatePost(string postId, float value)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.PostID.ToString() == postId);

            // update the rate values
            post.NumberReviewers += 1;
            post.TotalPoint += value;

            try
            {
                _dbContext.SaveChanges();
                return _modelFactory.ConvertToRateDTO(post);
            }
            catch(Exception e)
            {
                return null;
            }
        }

        #endregion

        #region Comment

        public async Task<CommentDTO> CreateComment(CreatingCommentDTO model)
        {
            var comment = new Comment()
            {
                CommentID = Guid.NewGuid(),
                PhongTroUserID = model.UserId,
                PostID = Guid.Parse(model.PostId),
                Content = model.Content
            };

            try
            {
                _dbContext.Comments.Add(comment);
                _dbContext.SaveChanges();

                comment.PhongTroUser = await _userManager.FindByIdAsync(comment.PhongTroUserID);
                return _modelFactory.ConvertToCommentDTO(comment);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public IEnumerable<CommentDTO> GetAllCommentsByPost(string postId)
        {
            return _dbContext.Comments
                .Include(c => c.PhongTroUser)
                .Where(c => c.PostID.ToString() == postId)
                .ToList()
                .Select(c => _modelFactory.ConvertToCommentDTO(c));
        }

        public async Task<bool> CheckCommentOwner(string userId, string commentId)
        {
            var comment = await _dbContext.Comments
                .FirstOrDefaultAsync(c => c.PhongTroUserID == userId && c.CommentID.ToString() == commentId);

            return comment != null;
        }

        public async Task<bool> DeleteComment(string commentId)
        {
            var comment = await _dbContext.Comments
                .FirstOrDefaultAsync(c => c.CommentID.ToString() == commentId);

            try
            {
                _dbContext.Comments.Remove(comment);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        #endregion
    }
}
