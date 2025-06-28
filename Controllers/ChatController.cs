using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChatApp.Data;
using System.Linq;

namespace ChatApp.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetMessages(string receiver)
        {
            var currentUser = User.Identity?.Name;

            if (string.IsNullOrEmpty(receiver) || string.IsNullOrEmpty(currentUser))
                return BadRequest();

            var messages = _context.Messages
                .Where(m =>
                    (m.SenderId == currentUser && m.ReceiverId == receiver) ||
                    (m.SenderId == receiver && m.ReceiverId == currentUser)
                )
                .OrderBy(m => m.Timestamp)
                .Select(m => new
                {
                    m.SenderId,
                    m.ReceiverId,
                    m.Content,
                    Timestamp = m.Timestamp.ToLocalTime().ToString("HH:mm"),
                    m.IsSeen
                })
                .ToList();

            return Json(messages);
        }
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users.Select(u => u.UserName).ToList();
            return Json(users);
        }
    }
}
