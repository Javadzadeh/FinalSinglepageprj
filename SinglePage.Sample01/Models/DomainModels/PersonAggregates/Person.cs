using System.ComponentModel.DataAnnotations;

namespace SinglePage.Sample01.Models.DomainModels.PersonAggregates
{
    public class Person
    {
        
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
    }
}
