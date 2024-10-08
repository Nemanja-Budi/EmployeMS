﻿using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using AutoMapper;

namespace ADMitroSremEmploye.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AnnualLeave, AnnualLeaveDto>().ReverseMap();
            CreateMap<Employe, EmployeDto>().ReverseMap();
            CreateMap<EmployeChild, EmployeChildDto>().ReverseMap();
            CreateMap<AuditLog, AuditLogDto>().ReverseMap();
            CreateMap<EmployeSalary, EmployeSalaryDto>().ReverseMap();
            CreateMap<User, ViewUserDto>().ReverseMap();
            CreateMap<Bank, EmployeBank>().ReverseMap();
            CreateMap<Bank, BankDto>().ReverseMap();
        }
    }
}
