using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SinglePage.Sample01.ApplicationServices.Contracts;
using SinglePage.Sample01.ApplicationServices.Dtos.PersonDtos;
using SinglePage.Sample01.ApplicationServices.Services;
using SinglePage.Sample01.Frameworks.ResponseFrameworks.Contracts;

namespace SinglePage.Sample01.Controllers
{
    public class PersonController : Controller
    {
        private readonly IPersonService _personService;

        #region [- ctor -]
        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }
        #endregion

        #region [- Index() -]
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region [- GetAll() -]
        public async Task<IActionResult> GetAll()
        {
            Guard_PersonService();
            var getAllResponse = await _personService.GetAll();
            var response = getAllResponse.Value.GetPersonServiceDtos;
            return Json(response);
        }
        #endregion

        #region [- Get() -]
        public async Task<IActionResult> Get(GetPersonServiceDto dto)
        {
            Guard_PersonService();
            var getResponse = await _personService.Get(dto);
            var response = getResponse.Value;
            if (response is null)
            {
                return Json("NotFound");
            }
            return Json(response);
        }
        #endregion

        #region [- Post() -]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PostPersonServiceDto dto)
        {
            Guard_PersonService();
            var postedDto = new GetPersonServiceDto() { Email = dto.Email };
            var getResponse = await _personService.Get(postedDto);

            if (ModelState.IsValid && getResponse.Value is null)
            {
                var postResponse = await _personService.Post(dto);
                return postResponse.IsSuccessful ? Ok() : BadRequest();
            }
            else if (ModelState.IsValid && getResponse.Value is not null)
            {
                return Conflict(dto);
            }
            else
            {
                return BadRequest();
            }
        }
        #endregion

        #region [- Put() -]
        [HttpPost]
        public async Task<IActionResult> Put([FromBody] PutPersonServiceDto dto)
        {
            Guard_PersonService();


            var putDto = new GetPersonServiceDto() {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email };

            if (ModelState.IsValid)
            {
                var putResponse = await _personService.Put(dto);
                return putResponse.IsSuccessful ? Ok() : BadRequest();
            }
            else
            {
                return BadRequest();
            }
        }
        #endregion

        #region [- PersonServiceGuard() -]
        private ObjectResult Guard_PersonService()
        {
            if (_personService is null)
            {
                return Problem($" {nameof(_personService)} is null.");
            }

            return null;
        }
        #endregion

        #region [- Detail() -]
        public async Task<IActionResult> Detail([FromBody] PutPersonServiceDto dto)
        {
            Guard_PersonService();

            return Json(dto);
        }

        #endregion


        #region [- DeleteAll() -]
        [HttpDelete]
       
        public async Task<IActionResult> DeleteAll()
        {
            Guard_PersonService(); // بررسی صحت و موجودیت سرویس

            // تلاش برای حذف تمام رکوردها از سرویس
            var deleteAllResponse = await _personService.DeleteAll();

            // بررسی موفقیت عملیات حذف
            if (deleteAllResponse.IsSuccessful)
            {
                return Ok(new { Message = "All persons deleted successfully." });
            }
            else
            {
                return BadRequest(new { Message = "Delete all operation failed.", Reason = deleteAllResponse.Message });
            }
        }

        #endregion



        #region [- Delete() -]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeletePersonServiceDto dto)
        {
            Guard_PersonService(); // بررسی صحت و موجودیت سرویس

            // بررسی اگر DTO اطلاعات لازم را ندارد
            if (dto == null )
            {
                return BadRequest("Invalid person ID.");
            }

            // تلاش برای حذف شخص از سرویس
            var deleteResponse = await _personService.Delete(dto);

            // بررسی موفقیت عملیات حذف
            if (deleteResponse.IsSuccessful)
            {
                return Ok(new { Message = "Person deleted successfully." });
            }
            else
            {
                return BadRequest(new { Message = "Delete operation failed.", Reason = deleteResponse.Message });
            }
        }
        #endregion






    }
}
