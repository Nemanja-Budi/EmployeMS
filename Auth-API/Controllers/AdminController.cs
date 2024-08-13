using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ADMitroSremEmploye.Models.Domain;
using System.Threading.Tasks;
using System.Collections.Generic;
using ADMitroSremEmploye.Models.DTOs;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.IdentityModel.Tokens;
using ADMitroSremEmploye.Repositories.Member_repository;
using AutoMapper;
using ADMitroSremEmploye.Models.DTOs.Filters;

namespace ADMitroSremEmploye.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMemberRepository memberRepository;
        private readonly IMapper mapper;

        public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMemberRepository memberRepository, IMapper mapper)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.memberRepository = memberRepository;
            this.mapper = mapper;
        }

        [HttpGet("get-members")]
        public async Task<ActionResult<IEnumerable<MemberViewDto>>> GetMembers(
           [FromQuery] MemberFilterDto memberFilterDto,
           string? sortBy = null,
           bool isAscending = true,
           int pageNumber = 1,
           int pageSize = 1000)
        {
            var (totalCount, members) = await memberRepository.GetMembersAsync(memberFilterDto, sortBy, isAscending, pageNumber, pageSize);

            return Ok(new { TotalCount = totalCount, Members = mapper.Map<IEnumerable<MemberViewDto>>(members) });

        }

        [HttpGet("get-member/{id}")]
        public async Task<ActionResult<MemberAddEditDto>> GetMember(string id)
        {
            var member = await memberRepository.GetMemberAsync(id);

            return Ok(member);
        }


        [HttpPost("add-edit-member")]
        public async Task<IActionResult> AddEditMember(MemberAddEditDto memberAddEditDto)
        {
            var result = await memberRepository.AddEditMemberAsync(memberAddEditDto);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("errors", error.Description);
                }
                return BadRequest(ModelState);
            }

            var message = string.IsNullOrEmpty(memberAddEditDto.Id)
                ? $"{memberAddEditDto.UserName} has been created"
                : $"{memberAddEditDto.UserName} has been updated";

            var title = string.IsNullOrEmpty(memberAddEditDto.Id)
                ? "Member Created"
                : "Member Edited";

            return Ok(new JsonResult(new { title, message }));
        }

        [HttpPut("lock-member/{id}")]
        public async Task<IActionResult> LockMember(string id)
        {
            var result = await memberRepository.LockMemberAsync(id);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("errors", error.Description);
                }
                return BadRequest(ModelState);
            }

            return NoContent();
        }

        [HttpPut("unlock-member/{id}")]
        public async Task<IActionResult> UnlockMember(string id)
        {
            var result = await memberRepository.UnlockMemberAsync(id);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("errors", error.Description);
                }
                return BadRequest(ModelState);
            }

            return NoContent();
        }

        [HttpDelete("delete-member/{id}")]
        public async Task<IActionResult> DeleteMember(string id)
        {
            var result = await memberRepository.DeleteMemberAsync(id);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("errors", error.Description);
                }
                return BadRequest(ModelState);
            }

            return NoContent();
        }

        [HttpGet("get-application-roles")]
        public async Task<ActionResult<string[]>> GetApplicationRoles()
        {
            return Ok(await roleManager.Roles.Select(x => x.Name).ToListAsync());
        }
    }
}
