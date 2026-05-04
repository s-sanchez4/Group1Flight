using System.Linq.Expressions;

namespace Group1Flight.Models.DataLayer.Repositories
{
    public class QueryOptions<T> where T : class
    {
        // For Include() statements (e.g., "Airline")
        public string[] Includes { get; set; } = Array.Empty<string>();

        // For Where() clauses (e.g., f => f.FlightId == id)
        public Expression<Func<T, bool>>? Where { get; set; }

        // For OrderBy() clauses
        public Expression<Func<T, object>>? OrderBy { get; set; }

        // Helper properties to check if these are set
        public bool HasWhere => Where != null;
        public bool HasOrderBy => OrderBy != null;
    }
}