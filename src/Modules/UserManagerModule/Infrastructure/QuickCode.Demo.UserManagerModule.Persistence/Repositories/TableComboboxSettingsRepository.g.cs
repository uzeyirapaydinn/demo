//------------------------------------------------------------------------------ 
// <auto-generated> 
// This code was generated by QuickCode. 
// Runtime Version:1.0
// 
// Changes to this file may cause incorrect behavior and will be lost if 
// the code is regenerated. 
// </auto-generated> 
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using QuickCode.Demo.UserManagerModule.Application.Models;
using QuickCode.Demo.UserManagerModule.Domain.Entities;
using QuickCode.Demo.UserManagerModule.Application.Interfaces.Repositories;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuickCode.Demo.UserManagerModule.Persistence.Contexts;
using QuickCode.Demo.UserManagerModule.Application.Dtos;

namespace QuickCode.Demo.UserManagerModule.Persistence.Repositories
{
    public partial class TableComboboxSettingsRepository : ITableComboboxSettingsRepository
    {
        private readonly WriteDbContext _writeContext;
        private readonly ReadDbContext _readContext;
        private readonly ILogger<TableComboboxSettingsRepository> _logger;
        public TableComboboxSettingsRepository(ILogger<TableComboboxSettingsRepository> logger, WriteDbContext writeContext, ReadDbContext readContext)
        {
            _writeContext = writeContext;
            _readContext = readContext;
            _logger = logger;
        }

        public async Task<DLResponse<TableComboboxSettings>> InsertAsync(TableComboboxSettings value)
        {
            var returnValue = new DLResponse<TableComboboxSettings>(value, "Success");
            try
            {
                await _writeContext.TableComboboxSettings.AddAsync(value);
                await _writeContext.SaveChangesAsync();
                returnValue = new DLResponse<TableComboboxSettings>(value, "Success");
            }
            catch (SqlException ex)
            {
                _logger.LogError("{repoName} SqlException {error}", "TableComboboxSettings Insert", ex.Message);
                if (ex.Number.Equals(2627))
                {
                    returnValue.Code = 999;
                }
                else
                {
                    returnValue.Code = 998;
                }

                returnValue.Message = ex.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError("{repoName} Exception {error}", "TableComboboxSettings Insert", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<bool>> UpdateAsync(TableComboboxSettings value)
        {
            var returnValue = new DLResponse<bool>(false, "Success");
            try
            {
                _writeContext.Set<TableComboboxSettings>().Update(value);
                await _writeContext.SaveChangesAsync();
                returnValue.Value = true;
            }
            catch (SqlException ex)
            {
                _logger.LogError("{repoName} SqlException {error}", "TableComboboxSettings Update", ex.Message);
                if (ex.Number.Equals(2627))
                {
                    returnValue.Code = 999;
                }
                else
                {
                    returnValue.Code = 998;
                }

                returnValue.Message = ex.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError("{repoName} Exception {error}", "TableComboboxSettings", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<bool>> DeleteAsync(TableComboboxSettings value)
        {
            var returnValue = new DLResponse<bool>(false, "Success");
            try
            {
                _writeContext.TableComboboxSettings.Remove(value);
                await _writeContext.SaveChangesAsync();
                returnValue.Value = true;
            }
            catch (SqlException ex)
            {
                _logger.LogError("{repoName} SqlException {error}", "TableComboboxSettings Delete", ex.Message);
                if (ex.Number.Equals(2627))
                {
                    returnValue.Code = 999;
                }
                else
                {
                    returnValue.Code = 998;
                }

                returnValue.Message = ex.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError("{repoName} Exception {error}", "TableComboboxSettings Delete", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<TableComboboxSettings>> GetByPkAsync(string tableName)
        {
            var returnValue = new DLResponse<TableComboboxSettings>();
            try
            {
                var result =
                    from table_combobox_settings in _readContext.TableComboboxSettings
                    where table_combobox_settings.TableName.Equals(tableName)select table_combobox_settings;
                returnValue.Value = await result.FirstAsync();
                if (returnValue.Value == null)
                {
                    returnValue.Code = 404;
                    returnValue.Message = $"Not found in TableComboboxSettings";
                    return returnValue;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{repoName} Exception {error}", "TableComboboxSettings GetByPk", ex.Message);
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<List<TableComboboxSettings>>> ListAsync(int? pageNumber = null, int? pageSize = null)
        {
            var returnValue = new DLResponse<List<TableComboboxSettings>>();
            try
            {
                if (pageNumber < 1)
                {
                    returnValue.Code = 404;
                    returnValue.Message = "Page Number must be greater than 1";
                }
                else
                {
                    if (pageNumber != null)
                    {
                        var skip = ((pageNumber - 1) * pageSize);
                        var take = pageSize;
                        returnValue.Value = await _readContext.TableComboboxSettings.Skip(skip.Value).Take(take.Value).ToListAsync();
                    }
                    else
                    {
                        returnValue.Value = await _readContext.TableComboboxSettings.ToListAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }

        public async Task<DLResponse<int>> CountAsync()
        {
            var returnValue = new DLResponse<int>();
            try
            {
                returnValue.Value = await _readContext.TableComboboxSettings.CountAsync();
            }
            catch (Exception ex)
            {
                returnValue.Code = 404;
                returnValue.Message = ex.ToString();
            }

            return returnValue;
        }
    }
}