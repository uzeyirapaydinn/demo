using System.Collections.Generic;
using QuickCode.Demo.SmsManagerModule.Application.Models;
using System.Threading.Tasks;

namespace QuickCode.Demo.SmsManagerModule.Application.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<DLResponse<T>> InsertAsync(T value);
        Task<DLResponse<bool>> UpdateAsync(T value);
        Task<DLResponse<bool>> DeleteAsync(T value);
        Task<DLResponse<List<T>>> ListAsync(int? pageNumber = null, int? pageSize = null);
        Task<DLResponse<int>> CountAsync();
    }
}
