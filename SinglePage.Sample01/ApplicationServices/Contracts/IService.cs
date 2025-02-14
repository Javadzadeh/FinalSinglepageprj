using SinglePage.Sample01.Frameworks.ResponseFrameworks.Contracts;

namespace SinglePage.Sample01.ApplicationServices.Contracts
{
    public interface IService<TPost, TGet, TGetAll, TUpdate, TDelete ,TDeleteAll>
    {
        Task<IResponse<TGetAll>> GetAll();
        Task<IResponse<TGet>> Get(TGet dto);
        Task<IResponse<TPost>> Post(TPost dto);
        Task<IResponse<TUpdate>> Put(TUpdate dto);
        Task<IResponse<TDelete>> Delete(TDelete dto);
        Task<IResponse<TGetAll>> DeleteAll();
    }
}
