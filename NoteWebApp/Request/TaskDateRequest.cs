using Microsoft.AspNetCore.Mvc;

namespace NoteWebApp.Request {
    public class TaskDateRequest {
        [FromHeader]
        public DateTime DateFrom { get; set; }
        [FromHeader]
        public DateTime DateTo { get; set; }
    }
}
