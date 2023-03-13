using Microsoft.AspNetCore.Mvc;

namespace NoteWebApp.Request
{
    public class TaskDateRequest
    {
        [FromQuery]
        public long dateFrom { get; set; }
        [FromQuery]
        public long dateTo { get; set; }
    }
}
