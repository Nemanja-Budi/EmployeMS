using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ADMitroSremEmploye.Repositories.Member_repository
{
    public class SQLMemberRepository: IMemberRepository
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public SQLMemberRepository(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<(int totalCount, IEnumerable<MemberViewDto>)> GetMembersAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {

            var query = userManager.Users
                .Where(x => x.UserName != SD.AdminUserName);

            if (!string.IsNullOrEmpty(filterOn) && !string.IsNullOrEmpty(filterQuery))
            {
                switch (filterOn.ToLower())
                {
                    case "username":
                        query = query.Where(x => x.UserName.Contains(filterQuery));
                        break;
                    case "firstname":
                        query = query.Where(x => x.FirstName.Contains(filterQuery));
                        break;
                    case "lastname":
                        query = query.Where(x => x.LastName.Contains(filterQuery));
                        break;
                    default:
                        break;
                }
            }

            var memberTotalLength = await query.CountAsync();

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "username":
                        query = isAscending ? query.OrderBy(x => x.UserName) : query.OrderByDescending(x => x.UserName);
                        break;
                    case "firstname":
                        query = isAscending ? query.OrderBy(x => x.FirstName) : query.OrderByDescending(x => x.FirstName);
                        break;
                    case "lastname":
                        query = isAscending ? query.OrderBy(x => x.LastName) : query.OrderByDescending(x => x.LastName);
                        break;
                    default:
                        break;
                }
            }

            var paginatedMembers = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(member => new MemberViewDto
                {
                    Id = member.Id,
                    UserName = member.UserName,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    DateCreated = member.DataCreated,
                    IsLocked = userManager.IsLockedOutAsync(member).GetAwaiter().GetResult(),
                    Roles = userManager.GetRolesAsync(member).GetAwaiter().GetResult()
                })
                .ToListAsync();

            return (memberTotalLength, paginatedMembers);

        }

        public async Task<MemberAddEditDto?> GetMemberAsync(string id)
        {
            var member = await userManager.Users
               .Where(x => x.UserName != SD.AdminUserName && x.Id == id)
               .Select(m => new MemberAddEditDto
               {
                   Id = m.Id,
                   UserName = m.UserName,
                   FirstName = m.FirstName,
                   LastName = m.LastName,
                   Roles = string.Join(",", userManager.GetRolesAsync(m).GetAwaiter().GetResult())
               }).FirstOrDefaultAsync();
            return member;
        }

        public async Task<IdentityResult> AddEditMemberAsync(MemberAddEditDto memberAddEditDto)
        {
            User user;

            if (string.IsNullOrEmpty(memberAddEditDto.Id))
            {
                if (string.IsNullOrEmpty(memberAddEditDto.Password) || memberAddEditDto.Password.Length < 6)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "Password must be at least 6 characters" });
                }

                user = new User
                {
                    FirstName = memberAddEditDto.FirstName.ToLower(),
                    LastName = memberAddEditDto.LastName.ToLower(),
                    UserName = memberAddEditDto.UserName.ToLower(),
                    Email = memberAddEditDto.UserName.ToLower(),
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, memberAddEditDto.Password);
                if (!result.Succeeded) return result;
            }
            else
            {
                if (!string.IsNullOrEmpty(memberAddEditDto.Password))
                {
                    if (memberAddEditDto.Password.Length < 6)
                    {
                        return IdentityResult.Failed(new IdentityError { Description = "Password must be at least 6 characters" });
                    }
                }

                if (await IsAdminUserIdAsync(memberAddEditDto.Id))
                {
                    return IdentityResult.Failed(new IdentityError { Description = SD.SuperAdminChangeNotAllowed });
                }

                user = await userManager.FindByIdAsync(memberAddEditDto.Id);

                if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

                user.FirstName = memberAddEditDto.FirstName.ToLower();
                user.LastName = memberAddEditDto.LastName.ToLower();
                user.UserName = memberAddEditDto.UserName.ToLower();

                if (!string.IsNullOrEmpty(memberAddEditDto.Password))
                {
                    await userManager.RemovePasswordAsync(user);
                    await userManager.AddPasswordAsync(user, memberAddEditDto.Password);
                }
            }

            var userRoles = await userManager.GetRolesAsync(user);

            await userManager.RemoveFromRolesAsync(user, userRoles);

            foreach (var role in memberAddEditDto.Roles.Split(",").ToArray())
            {
                var roleToAdd = await roleManager.Roles.FirstOrDefaultAsync(r => r.Name == role);

                if (roleToAdd != null)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }

            return IdentityResult.Success;
        }

        private async Task<bool> IsAdminUserIdAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            return user != null && user.UserName.Equals(SD.AdminUserName, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<IdentityResult> LockMemberAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            if (await IsAdminUserIdAsync(userId))
            {
                return IdentityResult.Failed(new IdentityError { Description = SD.SuperAdminChangeNotAllowed });
            }

            return await userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(5));
        }

        public async Task<IdentityResult> UnlockMemberAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            if (await IsAdminUserIdAsync(userId))
            {
                return IdentityResult.Failed(new IdentityError { Description = SD.SuperAdminChangeNotAllowed });
            }

            return await userManager.SetLockoutEndDateAsync(user, null);
        }

        public async Task<IdentityResult> DeleteMemberAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            if (await IsAdminUserIdAsync(userId))
            {
                return IdentityResult.Failed(new IdentityError { Description = SD.SuperAdminChangeNotAllowed });
            }

            return await userManager.DeleteAsync(user);
        }
    }
}

