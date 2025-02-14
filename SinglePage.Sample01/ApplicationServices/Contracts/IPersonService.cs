using SinglePage.Sample01.ApplicationServices.Dtos.PersonDtos;

namespace SinglePage.Sample01.ApplicationServices.Contracts;

public interface IPersonService : 
    IService<PostPersonServiceDto, GetPersonServiceDto, GetAllPersonServiceDto, PutPersonServiceDto, DeletePersonServiceDto, DeleteAllPersonServiceDto>
{
}