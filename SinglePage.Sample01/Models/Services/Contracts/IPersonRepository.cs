using SinglePage.Sample01.Models.DomainModels.PersonAggregates;

namespace SinglePage.Sample01.Models.Services.Contracts;

public interface IPersonRepository : IRepository<Person, IEnumerable<Person>>
{
    //Task<IResponse<Person>> SelectByNationalCode(string nationalCode);
}