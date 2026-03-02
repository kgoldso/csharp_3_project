namespace _3_project.Models
{
    /// <summary>
    /// Represents one record from the CSV / database.
    /// </summary>
    public record Person
    {
        public int Id { get; init; }
        public DateTime Date { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string SurName { get; init; }
        public required string City { get; init; }
        public required string Country { get; init; }
    }
}
