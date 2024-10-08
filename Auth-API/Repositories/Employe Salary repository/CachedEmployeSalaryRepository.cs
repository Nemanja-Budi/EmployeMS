﻿using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Models.DTOs;
using ADMitroSremEmploye.Models.DTOs.Filters;
using ADMitroSremEmploye.Repositories.Employe_repository;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;

namespace ADMitroSremEmploye.Repositories.Employe_Salary_repository
{
    public class CachedEmployeSalaryRepository : IEmployeSalaryRepository
    {
        private readonly SQLEmployeSalaryRepository decorated;
        private readonly IMemoryCache memoryCache;
        private static readonly object cacheKeysLock = new object();
        private static readonly HashSet<string> employeSalaryCacheKeys = new HashSet<string>();
        public CachedEmployeSalaryRepository(SQLEmployeSalaryRepository decorated, IMemoryCache memoryCache)
        {
            this.decorated = decorated;
            this.memoryCache = memoryCache;
        }

        public async Task<(int totalCount, IEnumerable<EmployeSalary>)> GetAllEmployeSalarysAsync(EmployeSalaryFilterDto filterDto, CommonFilterDto commonFilterDto)
        {
            string cacheKey = $"employe-salary-" +
                $"{filterDto.FirstName}-" +
                $"{filterDto.LastName}-" +
                $"{filterDto.BankName}-" +
                $"{filterDto.CalculationMonth}-" +
                $"{commonFilterDto.SortBy}-" +
                $"{commonFilterDto.IsAscending}-" +
                $"{commonFilterDto.PageNumber}-" +
                $"{commonFilterDto.PageSize}";

            lock (cacheKeysLock)
            {
                employeSalaryCacheKeys.Add(cacheKey);
            }

            if (!memoryCache.TryGetValue(cacheKey, out (int totalCount, IEnumerable<EmployeSalary>) cachedResult))
            {
                cachedResult = await decorated.GetAllEmployeSalarysAsync(filterDto, commonFilterDto);

                memoryCache.Set(cacheKey, cachedResult, TimeSpan.FromMinutes(2));
            }

            return cachedResult;
        }

        public async Task<Employe?> GetEmployeByIdAsync(Guid employeId)
        {
            string key = $"employe-salarys-employe-{employeId}";

            lock (cacheKeysLock)
            {
                employeSalaryCacheKeys.Add(key);
            }

            return await memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return await decorated.GetEmployeByIdAsync(employeId);
            });
        }

        public async Task<EmployeSalary?> GetEmployeSalaryById(Guid employeSalaryId)
        {
            string key = $"employe-salarys-{employeSalaryId}";

            lock (cacheKeysLock)
            {
                employeSalaryCacheKeys.Add(key);
            }

            return await memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return await decorated.GetEmployeSalaryById(employeSalaryId);
            });
        }

        public async Task<List<EmployeSalary>?> GetEmployeSalarysByEmployeIdAsync(Guid employeId)
        {
            string key = $"employe-salarys-by-employeId-{employeId}";

            lock (cacheKeysLock)
            {
                employeSalaryCacheKeys.Add(key);
            }

            if (!memoryCache.TryGetValue(key, out List<EmployeSalary>? cachedSalaries))
            {
                cachedSalaries = await decorated.GetEmployeSalarysByEmployeIdAsync(employeId);

                if (cachedSalaries != null)
                {
                    memoryCache.Set(key, cachedSalaries, TimeSpan.FromMinutes(5));
                }
            }

            return cachedSalaries;
        }

        public async Task<decimal> GetGrandTotalSalaryAsync(int month, int year)
        {
            string key = $"total-by-banks-{month}-{year}";

            lock (cacheKeysLock)
            {
                employeSalaryCacheKeys.Add(key);
            }

            return await memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                return await decorated.GetGrandTotalSalaryAsync(month, year);
            });
        }

        public async Task<IEnumerable<BankSalaryDto>?> GetSalariesByBankAsync(int month, int year)
        {
            string key = $"salary-by-banks-{month}-{year}";

            lock (cacheKeysLock)
            {
                employeSalaryCacheKeys.Add(key);
            }

            return await memoryCache.GetOrCreateAsync(key, async entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));

                var result = await decorated.GetSalariesByBankAsync(month, year);

                return result ?? Enumerable.Empty<BankSalaryDto>();
            });
        }

        public async Task<EmployeSalary> AddEmployeSalaryAsync(EmployeSalary employeSalary)
        {
            RemoveRelatedCache();

            return await decorated.AddEmployeSalaryAsync(employeSalary);
        }

        public async Task<bool> DeleteEmployeSalaryByEmployeSalaryIdAsync(Guid employeSalaryId)
        {
            RemoveRelatedCache();

            return await decorated.DeleteEmployeSalaryByEmployeSalaryIdAsync(employeSalaryId);
        }

        public async Task<bool> DeleteEmployeSalarysByEmployeIdAsync(Guid employeId)
        {
            RemoveRelatedCache();

            return await decorated.DeleteEmployeSalarysByEmployeIdAsync(employeId);
        }

        public async Task SaveEmployeSalaryAsync()
        {
            await decorated.SaveEmployeSalaryAsync();
        }

        private void RemoveRelatedCache()
        {
            lock (cacheKeysLock)
            {
                foreach (var key in employeSalaryCacheKeys)
                {
                    memoryCache.Remove(key);
                }
            }
            employeSalaryCacheKeys.Clear();
        }
    }
}
