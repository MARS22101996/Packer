namespace TicketService.WEB.Models
{
    public class PageApiModel
    {
        public PageApiModel(int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;

            TotalPages = count / pageSize;
        }

        public int PageNumber { get; set; }

        public int TotalPages { get; set; }
    }
}