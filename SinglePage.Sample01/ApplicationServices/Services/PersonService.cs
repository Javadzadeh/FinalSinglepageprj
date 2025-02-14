using SinglePage.Sample01.ApplicationServices.Contracts;
using SinglePage.Sample01.ApplicationServices.Dtos.PersonDtos;
using SinglePage.Sample01.Frameworks;
using SinglePage.Sample01.Frameworks.ResponseFrameworks;
using SinglePage.Sample01.Frameworks.ResponseFrameworks.Contracts;
using SinglePage.Sample01.Models.DomainModels.PersonAggregates;
using SinglePage.Sample01.Models.Services.Contracts;
using System.Net;

namespace SinglePage.Sample01.ApplicationServices.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;

        #region [- ctor -]
        public PersonService(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }
        #endregion
        
        #region [- GetAll() -]
        public async Task<IResponse<GetAllPersonServiceDto>> GetAll()
        {
            var selectAllResponse = await _personRepository.SelectAll();

            if (selectAllResponse is null)
            {
                return new Response<GetAllPersonServiceDto>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.NullInput, null);
            }

            if (!selectAllResponse.IsSuccessful)
            {
                return new Response<GetAllPersonServiceDto>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.Error, null);
            }

            var getAllPersonDto = new GetAllPersonServiceDto(){GetPersonServiceDtos = new List<GetPersonServiceDto>()};

            foreach (var item in selectAllResponse.Value)
            {
                var personDto = new GetPersonServiceDto()
                {
                    Id = item.Id,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Email = item.Email
                };
                getAllPersonDto.GetPersonServiceDtos.Add(personDto);
            }

            var response = new Response<GetAllPersonServiceDto>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, getAllPersonDto);
            return response;
        }
        #endregion

        #region [- Get() -]
        public async Task<IResponse<GetPersonServiceDto>> Get(GetPersonServiceDto dto)
        {
            var person = new Person()
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email
            };
            var selectResponse = await _personRepository.Select(person);

            if (selectResponse is null)
            {
                return new Response<GetPersonServiceDto>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.NullInput, null);
            }

            if (!selectResponse.IsSuccessful)
            {
                return new Response<GetPersonServiceDto>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.Error, null);
            }
            var getPersonServiceDto = new GetPersonServiceDto()
            {
                Id = selectResponse.Value.Id,
                FirstName = selectResponse.Value.FirstName,
                LastName = selectResponse.Value.LastName,
                Email = selectResponse.Value.Email
            };
            var response = new Response<GetPersonServiceDto>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, getPersonServiceDto);
            return response;
        } 
        #endregion
        
        #region [- Post() -]
        public async Task<IResponse<PostPersonServiceDto>> Post(PostPersonServiceDto dto)
        {
            if (dto is null)
            {
                return new Response<PostPersonServiceDto>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.NullInput, null);
            }
            var postedPerson = new Person()
            {
                Id = new Guid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email
            };
            var insertedResponse = await _personRepository.Insert(postedPerson);

            if (!insertedResponse.IsSuccessful)
            {
                return new Response<PostPersonServiceDto>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.Error, dto);
            }

            var response = new Response<PostPersonServiceDto>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, dto);
            return response;
        }
        #endregion

        #region - Put -
        public async Task<IResponse<PutPersonServiceDto>> Put(PutPersonServiceDto dto)
        {
            try
            {
                if (dto == null || dto.Id == Guid.Empty)
                {
                    return new Response<PutPersonServiceDto>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.NullInput, null);
                }
                var postedPerson = new Person()
                {
                    Id = dto.Id == Guid.Empty ? new Guid() : dto.Id,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email
                };

                var updateResponse = await _personRepository.Update(postedPerson);
               

                var responseDto = new PutPersonServiceDto
                {
                    Id = postedPerson.Id,
                    FirstName = postedPerson.FirstName,
                    LastName = postedPerson.LastName,
                    Email = postedPerson.Email
                };

                return new Response<PutPersonServiceDto>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, responseDto);
            }
            catch (Exception ex)
            {
                return new Response<PutPersonServiceDto>(false, HttpStatusCode.InternalServerError, ex.Message, null);
            }
        }

        #endregion


        #region - Delete -
        public async Task<IResponse<DeletePersonServiceDto>> Delete(DeletePersonServiceDto dto)
        {
            try
            {
                // بررسی اینکه آیا DTO معتبر است
                if (dto == null || dto.Id == Guid.Empty)
                {
                    return new Response<DeletePersonServiceDto>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.NullInput, null);
                }

                // جستجو برای شخص بر اساس ID
                var personToDelete = await _personRepository.Select(new Person { Id = dto.Id });

                // اگر شخص پیدا نشد، بازگشت پیام خطا
                if (personToDelete is null || !personToDelete.IsSuccessful)
                {
                    return new Response<DeletePersonServiceDto>(false, HttpStatusCode.NotFound, "Person not found", null);
                }

                // حذف شخص از دیتابیس
                var deleteResponse = await _personRepository.Delete(personToDelete.Value);

                // اگر حذف با موفقیت انجام شد
                if (deleteResponse.IsSuccessful)
                {
                    return new Response<DeletePersonServiceDto>(true, HttpStatusCode.OK, "Person deleted successfully", dto);
                }

                // اگر حذف شکست خورد
                return new Response<DeletePersonServiceDto>(false, HttpStatusCode.InternalServerError, ResponseMessages.Error, dto);
            }
            catch (Exception ex)
            {
                // در صورت بروز استثنا، پیام خطا بازگشت داده می‌شود
                return new Response<DeletePersonServiceDto>(false, HttpStatusCode.InternalServerError, ex.Message, null);
            }
        }

        #endregion


        #region - DeleteAll() -
        public async Task<IResponse<GetAllPersonServiceDto>> DeleteAll()
        {
            try
            {
                // دریافت تمام اشخاص
                var selectAllResponse = await _personRepository.SelectAll();

                if (selectAllResponse == null || !selectAllResponse.IsSuccessful)
                {
                    return new Response<GetAllPersonServiceDto>(false, HttpStatusCode.UnprocessableContent, "No persons found", null);
                }

                // حذف همه اشخاص
                foreach (var person in selectAllResponse.Value)
                {
                    var deleteResponse = await _personRepository.Delete(person);
                    if (!deleteResponse.IsSuccessful)
                    {
                        return new Response<GetAllPersonServiceDto>(false, HttpStatusCode.InternalServerError, "Failed to delete some persons", null);
                    }
                }

                return new Response<GetAllPersonServiceDto>(true, HttpStatusCode.OK, "All persons deleted successfully", null);
            }
            catch (Exception ex)
            {
                return new Response<GetAllPersonServiceDto>(false, HttpStatusCode.InternalServerError, ex.Message, null);
            }
        }

    }
    #endregion
}

