using Microsoft.IdentityModel.Tokens;

namespace NoteWebApp.Request {
    public class UserRequest {
        public string FirstName { get; set; } = null!;

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public DateTime? Birthday { get; set; }

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool validation() {
            if (FirstName.IsNullOrEmpty() || 
                LastName.IsNullOrEmpty() || 
                Password.IsNullOrEmpty() || 
                Email.IsNullOrEmpty()) {
                return false;
            }
            return true;
        }
    }
}
