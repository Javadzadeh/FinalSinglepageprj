using SinglePage.Sample01.Frameworks.ResponseFrameworks.Contracts;

namespace SinglePage.Sample01.Models.Services.Contracts
{
    public interface IRepository<T, TCollection>
    {
        Task<IResponse<TCollection>> SelectAll();
        Task<IResponse<T>> Select(T obj);
        Task<IResponse<T>> Insert(T obj);
        Task<IResponse<T>> Update(T obj);
        Task<IResponse<T>> Delete(T obj);
        Task<IResponse<TCollection>> DeleteAll();
        Task<IResponse<T>> Detail(T obj);
    }
}
