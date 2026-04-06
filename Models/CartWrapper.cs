namespace Group1Flight.Models
{
    public class CartWrapper
    {
        private readonly IHttpContextAccessor _accessor;
        private const string CookieName = "SelectedFlights";

        public CartWrapper(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        // Mechanical logic to get IDs from the cookie
        public List<int> GetSelectedIds()
        {
            var cookie = _accessor.HttpContext?.Request.Cookies[CookieName];
            if (string.IsNullOrEmpty(cookie)) return new List<int>();

            return cookie.Split(',')
                         .Select(id => int.TryParse(id, out int val) ? val : 0)
                         .Where(id => id > 0)
                         .ToList();
        }

        // Logic to add to cookie (The 14-day rule)
        public void AddToCart(int id)
        {
            var ids = GetSelectedIds();
            if (!ids.Contains(id)) ids.Add(id);

            var options = new CookieOptions { Expires = DateTime.Now.AddDays(14), IsEssential = true, Path = "/" };
            _accessor.HttpContext?.Response.Cookies.Append(CookieName, string.Join(",", ids), options);
        }
    }
}