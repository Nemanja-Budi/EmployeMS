using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using ADMitroSremEmploye.Models.DTOs.Filters;
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

        public async Task<(int totalCount, IEnumerable<MemberViewDto>)> GetMembersAsync(MemberFilterDto memberFilterDto ,string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {

            var usersQuery = userManager.Users
            .Where(x => x.UserName != SD.AdminUserName);

            // Filtriranje prema filterDto
            if (!string.IsNullOrEmpty(memberFilterDto.UserName))
            {
                usersQuery = usersQuery.Where(x => x.UserName.Contains(memberFilterDto.UserName));
            }

            if (!string.IsNullOrEmpty(memberFilterDto.FirstName))
            {
                usersQuery = usersQuery.Where(x => x.FirstName.Contains(memberFilterDto.FirstName));
            }

            if (!string.IsNullOrEmpty(memberFilterDto.LastName))
            {
                usersQuery = usersQuery.Where(x => x.LastName.Contains(memberFilterDto.LastName));
            }

            // Sortiranje
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "username":
                        usersQuery = isAscending ? usersQuery.OrderBy(x => x.UserName) : usersQuery.OrderByDescending(x => x.UserName);
                        break;
                    case "firstname":
                        usersQuery = isAscending ? usersQuery.OrderBy(x => x.FirstName) : usersQuery.OrderByDescending(x => x.FirstName);
                        break;
                    case "lastname":
                        usersQuery = isAscending ? usersQuery.OrderBy(x => x.LastName) : usersQuery.OrderByDescending(x => x.LastName);
                        break;
                    default:
                        break;
                }
            }

            // Paginacija i dohvati korisnike
            var users = await usersQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var members = new List<MemberViewDto>();

            foreach (var user in users)
            {
                var memberToAdd = new MemberViewDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateCreated = user.DataCreated,
                    IsLocked = await userManager.IsLockedOutAsync(user),
                    Roles = await userManager.GetRolesAsync(user)
                };

                members.Add(memberToAdd);
            }

            // Ukupan broj članova
            var memberTotalLength = await usersQuery.CountAsync();

            return (memberTotalLength, members);

        }

        public async Task<MemberAddEditDto?> GetMemberAsync(string id)
        {
            var user = await userManager.Users
                .Where(x => x.UserName != SD.AdminUserName && x.Id == id)
                .FirstOrDefaultAsync();

            var member = new MemberAddEditDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = string.Join(",", await userManager.GetRolesAsync(user))
            };

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

