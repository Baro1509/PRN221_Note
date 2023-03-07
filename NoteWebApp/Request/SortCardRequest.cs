namespace NoteWebApp.Request
{
    public class SortCardRequest
    {
        public Guid NoteId { get; set; }
        public byte SortType { get; set; }
        public bool? IsAsc { get; set; }

    }
}
