using NJsonApi.Infrastructure;

namespace NJsonApi.Web.MVC5.HelloWorld.Models
{
    public class Person : MetaData
    {
        public Person()
        {
            StaticPersistentStore.GetNextId();
        }

        public Person(string firstname, string lastname, string twitter) : this()
        {
            FirstName = firstname;
            LastName = lastname;
            Twitter = twitter;
        }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Twitter { get; set; }
    }
}