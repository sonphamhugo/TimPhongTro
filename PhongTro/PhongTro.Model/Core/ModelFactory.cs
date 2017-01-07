using Microsoft.AspNet.Identity.EntityFramework;
using PhongTro.Domain.Entities;
using PhongTro.Domain.Infracstucture;
using PhongTro.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhongTro.Model.Core
{
    /// <summary>
    /// Class is reponsible for convert between Entites and DTO
    /// </summary>
    public class ModelFactory
    {
        private PhongTroUserManager _AppUserManager;

        public ModelFactory(PhongTroUserManager userManager)
        {
            _AppUserManager = userManager;
        }

        public PhongTroUser ConvertToAppUser(RegisteringUserDTO registeringUser)
        {
            return new PhongTroUser()
            {
                UserName = registeringUser.Username,
                Email = registeringUser.Email,
                FirstName = registeringUser.FirstName,
                LastName = registeringUser.LastName,
                DateOfBirth = registeringUser.DateOfBirth,
                PhoneNumber = registeringUser.Phone,
            };
        }

        /// <summary>
        /// Helper method converts PhongTroUser object to UserDTO object
        /// </summary>
        /// <param name="user">The PhongTroUser to be converted</param>
        /// <returns>
        /// A UserDTO object
        /// </returns>
        public UserDTO ConvertFromAppUser(PhongTroUser user)
        {
            return new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Roles = _AppUserManager.GetRolesAsync(user.Id).Result,
                Claims = _AppUserManager.GetClaimsAsync(user.Id).Result
            };
        }

        /// <summary>
        /// Helper method converts IdentityRole system object to RoleDTO object
        /// </summary>
        /// <param name="role">The role to be converted</param>
        /// <returns>
        /// The RoleDTO object
        /// </returns>
        public RoleDTO ConvertFromIdentityRole(IdentityRole role)
        {
            return new RoleDTO()
            {
                Id = role.Id,
                Name = role.Name
            };
        }

        /// <summary>
        /// Convert an Post entity to DTO object
        /// </summary>
        /// <param name="post">Entity to be converted</param>
        /// <returns></returns>
        public PostDTO ConvertToPostDTO(Post post)
        {
            return new PostDTO
            {
                Id = post.PostID.ToString(),
                Address = post.Address,
                Price = post.Price,
                NumberLodgers = post.NumberLodgers,
                Description = post.Description,
                PostDate = post.PostDate,
                LastUpdate = post.LastUpdate,
                TotalPoint = post.TotalPoint,
                NumberReviewers = post.NumberReviewers,
                PhongTroUserID = post.PhongTroUserID.ToString(),
                Images = post.Images.Select(img => img.Url).ToArray()
            };
        }

        /// <summary>
        /// Convert a Comment entity to a DTO object
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public CommentDTO ConvertToCommentDTO(Comment comment)
        {
            return new CommentDTO()
            {
                CommentId = comment.CommentID.ToString(),
                UserId = comment.PhongTroUserID,
                FirstName = comment.PhongTroUser.FirstName,
                LastName = comment.PhongTroUser.LastName,
                PostId = comment.PostID.ToString(),
                Content = comment.Content,
                DateComment = comment.DateComment
            };
        }

        public RateDTO ConvertToRateDTO(Post post)
        {
            return new RateDTO()
            {
                PostId = post.PostID.ToString(),
                NumberReviewers = post.NumberReviewers,
                TotalPoint = post.TotalPoint
            };
        }
    }
}
