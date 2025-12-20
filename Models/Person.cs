namespace _3_project.Models
{
    /// <summary>
    /// Represents one record from the CSV / database.
    /// </summary>
    public class Person
    {
        public int Id {get; set;}
        public DateTime Date {get; set;}
        public string FirstName {get; set;} = null!;
        public string LastName {get; set;} = null!;
        public string SurName {get; set;} = null!;
        public string City {get; set;} = null!;
        public string Country {get; set;} = null!;

    }
}