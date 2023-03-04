using System.Security;

namespace NoteWebApp {
    public class ResponseEntity<T> {
        private T data;
        private int status;
        private string message;

        public ResponseEntity(T data, int status, string message) {
            this.data = data;
            this.status = status;
            this.message = message;
        }

        public T Data { get { return data; } }
        public int Status { get { return status; } }
        public string Message { get { return message; } }
    }
}
