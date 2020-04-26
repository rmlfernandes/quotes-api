namespace QuotesApi.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using QuotesApi.Data;
    using QuotesApi.Models;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuotesController : ControllerBase
    {
        private readonly QuotesDbContext context;

        public QuotesController(QuotesDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public IActionResult Get([FromQuery] string sort)
        {
            IQueryable<Quote> quotes;

            switch (sort)
            {
                case "desc":
                    quotes = this.context.Quotes.OrderByDescending(q => q.CreatedAt);
                    break;

                case "asc":
                    quotes = this.context.Quotes.OrderBy(q => q.CreatedAt);
                    break;

                default:
                    quotes = this.context.Quotes;
                    break;
            }

            return this.Ok(quotes);
        }

        [HttpGet("[action]")]
        public IActionResult PagingQuote([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var quotes = this.context.Quotes;
            var currentPageNumber = pageNumber ?? 1;
            var currentPageSize = pageSize ?? 5;

            return this.Ok(quotes.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize));
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult SearchQuote([FromQuery] string type)
        {
            var quotes = this.context.Quotes.Where(q => q.Type.StartsWith(type));

            return this.Ok(quotes);
        }

        [HttpGet("[action]")]
        public IActionResult MyQuotes()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var quotes = this.context.Quotes.Where(q => q.UserId == userId);

            return this.Ok(quotes);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var quote = this.context.Quotes.Find(id);

            if (quote == null)
            {
                return this.NotFound();
            }

            return this.Ok(quote);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Quote quote)
        {
            quote.UserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            this.context.Add(quote);

            this.context.SaveChanges();

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Quote quote)
        {
            var entity = this.context.Quotes.Find(id);

            if (entity == null)
            {
                return this.NotFound();
            }

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId != entity.UserId)
            {
                this.Forbid();
            }

            entity.Title = quote.Title;
            entity.Author = quote.Author;
            entity.Description = quote.Description;
            entity.Type = quote.Type;
            entity.CreatedAt = quote.CreatedAt;

            this.context.SaveChanges();

            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var entity = this.context.Quotes.Find(id);

            if (entity == null)
            {
                return this.NotFound();
            }

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId != entity.UserId)
            {
                this.Forbid();
            }

            this.context.Quotes.Remove(entity);

            this.context.SaveChanges();

            return this.NoContent();
        }
    }
}
