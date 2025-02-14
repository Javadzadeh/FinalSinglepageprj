using Microsoft.EntityFrameworkCore;
using SinglePage.Sample01.Frameworks.ResponseFrameworks;
using SinglePage.Sample01.Frameworks.ResponseFrameworks.Contracts;
using SinglePage.Sample01.Models.DomainModels.PersonAggregates;
using SinglePage.Sample01.Models.Services.Contracts;
using System.Net;
using SinglePage.Sample01.Frameworks;
using SinglePage.Sample01.ApplicationServices.Dtos.PersonDtos;

namespace SinglePage.Sample01.Models.Services.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ProjectDbContext _projectDbContext;

        #region [- ctor -]
        public PersonRepository(ProjectDbContext projectDbContext)
        {
            _projectDbContext = projectDbContext;
        } 
        #endregion

        #region [- Insert() -]
        public async Task<IResponse<Person>> Insert(Person model)
        {
            try
            {
                if (model is null)
                {
                    return new Response<Person>(false, HttpStatusCode.UnprocessableContent,ResponseMessages.NullInput, null);
                }
                await _projectDbContext.AddAsync(model);
                 //_projectDbContext.Add(model);
                _projectDbContext.SaveChanges();
                var response = new Response<Person>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, model);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region [- SelectAll() -]
        public async Task<IResponse<IEnumerable<Person>>> SelectAll()
        {
            try
            {
                var persons = await _projectDbContext.Person.AsNoTracking().ToListAsync();
                return persons is null ? 
                    new Response<IEnumerable<Person>>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.NullInput, null) :
                    new Response<IEnumerable<Person>>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation,persons);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region [- Select() -]
        public async Task<IResponse<Person>> Select(Person person)
        {
            try
            {
                var responseValue = new Person();
                if (person.Id.ToString() == "")
                {
                    //responseValue = await _projectDbContext.Person.FindAsync(person.Email);
                    responseValue = await _projectDbContext.Person.Where(c=>c.Email==person.Email).SingleOrDefaultAsync();
                }
                else
                {
                    responseValue = await _projectDbContext.Person.FindAsync(person.Id);
                }
                return responseValue is null ?
                     new Response<Person>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.NullInput, null) :
                     new Response<Person>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, responseValue);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region - Update -
        public async Task<IResponse<Person>> Update(Person obj)
        {
            var response = new Response<Person>();

            try
            {
                // یافتن رکورد مورد نظر در پایگاه داده
                //var existingPerson = await _projectDbContext.Person.FindAsync(obj.Id);

                if (obj == null)
                {
                    // اگر رکورد پیدا نشد
                    response.IsSuccessful = false;
                    response.Message = "Person not found.";
                    return response;
                }

               


                // ذخیره تغییرات
                _projectDbContext.Person.Update(obj);
                await _projectDbContext.SaveChangesAsync();

                // تنظیم پاسخ موفق
                response.IsSuccessful = true;
                response.Message = "Person updated successfully.";
                response.Value = obj;
            }
            catch (Exception ex)
            {
                // مدیریت خطا
                response.IsSuccessful = false;
                response.Message = $"An error occurred: {ex.Message}";
            }

            return response;
        }

        #endregion


        #region [- Detail() -]
        public async Task<IResponse<Person>> Detail(Person person)
        {
            try
            {
                Person responseValue = null;

                // جستجوی فرد بر اساس ID
                if (person.Id !=null )
                {
                    responseValue = await _projectDbContext.Person.FindAsync(person.Id);
                }
                // یا جستجوی فرد بر اساس Email
                else if (!string.IsNullOrEmpty(person.Email))
                {
                    responseValue = await _projectDbContext.Person
                        .Where(c => c.Email == person.Email)
                        .SingleOrDefaultAsync();
                }

                return responseValue == null
                    ? new Response<Person>(false, HttpStatusCode.UnprocessableContent, ResponseMessages.NullInput, null)
                    : new Response<Person>(true, HttpStatusCode.OK, ResponseMessages.SuccessfullOperation, responseValue);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region [- Delete() -]
        public async Task<IResponse<Person>> Delete(Person obj)
        {
            var response = new Response<Person>();

            try
            {
                // اگر شی null باشد
                if (obj == null)
                {
                    response.IsSuccessful = false;
                    response.Message = "Person not found.";
                    return response;
                }

                // جستجو برای پیدا کردن رکورد بر اساس ID
                var existingPerson = await _projectDbContext.Person.FindAsync(obj.Id);

                if (existingPerson == null)
                {
                    response.IsSuccessful = false;
                    response.Message = "Person not found.";
                    return response;
                }

                // حذف رکورد از دیتابیس
                _projectDbContext.Person.Remove(existingPerson);

                // ذخیره تغییرات در دیتابیس
                await _projectDbContext.SaveChangesAsync();

                response.IsSuccessful = true;
                response.Message = "Person deleted successfully.";
            }
            catch (Exception ex)
            {
                // مدیریت خطا
                response.IsSuccessful = false;
                response.Message = $"An error occurred: {ex.Message}";
            }

            return response;
        }


        #endregion

        #region -DeleteAll() - 
        public async Task<IResponse<IEnumerable<Person>>> DeleteAll()
        {
            var response = new Response<IEnumerable<Person>>();

            try
            {
                // دریافت تمام اشخاص از پایگاه داده
                var persons = await _projectDbContext.Person.ToListAsync();

                if (persons == null || !persons.Any())
                {
                    response.IsSuccessful = false;
                    response.Message = "No persons found to delete.";
                    return response;
                }

                // حذف تمام اشخاص از دیتابیس
                _projectDbContext.Person.RemoveRange(persons);

                // ذخیره تغییرات در دیتابیس
                await _projectDbContext.SaveChangesAsync();

                response.IsSuccessful = true;
                response.Message = "All persons deleted successfully.";
                response.Value = persons;
            }
            catch (Exception ex)
            {
                // مدیریت خطا
                response.IsSuccessful = false;
                response.Message = $"An error occurred: {ex.Message}";
            }

            return response;
        }

        #endregion

    }
}
