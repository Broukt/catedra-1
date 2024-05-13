namespace ebooks_dotnet7_api
{
    public class EditEbookDto
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public int Price { get; set; }
    }
}