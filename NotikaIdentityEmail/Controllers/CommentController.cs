using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly EmailContext _emailContext;
        private readonly UserManager<AppUser> _userManager;

        public CommentController(EmailContext emailContext, UserManager<AppUser> userManager = null)
        {
            _emailContext = emailContext;
            _userManager = userManager;
        }

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> UserComments()
        {
            var values = await _emailContext.Comments.Include(x => x.AppUser).ToListAsync();
            return View(values);
        }

        public async Task<IActionResult> UserCommentsByUserId()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var comments = await _emailContext.Comments.Where(y => y.AppUserId == user.Id ).ToListAsync();
            return View(comments);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserCommentsList()
        {
            var values = await _emailContext.Comments.Include(x => x.AppUser).ToListAsync();
            return View(values);
        }

        [HttpGet]
        public  PartialViewResult CreateComment()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(Comment comment)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name!);
            comment.AppUserId = user.Id;
            comment.CommentDate = DateTime.Now;

            using (var client = new HttpClient())
            {
                var apikey = "";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);

                try
                {
                    var translateRequestBody = new
                    {
                        inputs = comment.CommentDetail
                    };
                    var translatejson = JsonSerializer.Serialize(translateRequestBody);
                    var translateContent = new StringContent(translatejson,Encoding.UTF8,"application/json");

                    var translateResponse = await client.PostAsync("https://api-inference.huggingface.co/models/Helsinki-NLP/opus-mt-tr-en", translateContent);
                    var translateResponseString = await translateResponse.Content.ReadAsStringAsync();

                    string EnglishText = comment.CommentDetail;
                    if (translateResponseString.TrimStart().StartsWith("["))
                    {
                        var translateDoc = JsonDocument.Parse(translateResponseString);
                        EnglishText = translateDoc.RootElement[0].GetProperty("translation_text").GetString()!;
                    }
                    var Toxicrequestbody = new
                    {
                        inputs = comment.CommentDetail
                    };
                    var toxicjson = JsonSerializer.Serialize(Toxicrequestbody);
                    var toxiccontent = new StringContent(toxicjson, Encoding.UTF8, "application/json");
                    var toxicresponse = await client.PostAsync(
                    "https://router.huggingface.co/hf-inference/models/unitary/multilingual-toxic-xlm-roberta",
                    toxiccontent);

                    if (toxicresponse.IsSuccessStatusCode)
                    {
                        var ToxicresponseString = await toxicresponse.Content.ReadAsStringAsync();
                        if (ToxicresponseString.TrimStart().StartsWith("["))
                        {
                            var doc = JsonDocument.Parse(ToxicresponseString);
                            foreach (var item in doc.RootElement[0].EnumerateArray())
                            {
                                string label = item.GetProperty("label").GetString();
                                double score = item.GetProperty("score").GetDouble();
                                if (score > 0.5)
                                {
                                    comment.CommentStatus = "Toksik Yorum";
                                    break;
                                }
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(comment.CommentStatus))
                    {
                        comment.CommentStatus = "Yorum Onaylandı";
                    }
                }


               catch (Exception ex)
                {
                    comment.CommentStatus = "Yorum Onaylandı";
                }
            }
             _emailContext.Comments.Add(comment);
            _emailContext.SaveChanges();
            TempData["Success"] = "Yorum Başarıyla Eklenmiştir";
            return RedirectToAction("UserComments");
        }


        public IActionResult CommentStatusChangeToToxic(int id)
        {
            var value = _emailContext.Comments.Find(id);
            if (value == null) return NotFound();

            value.CommentStatus = "Toxic Yorum";
            _emailContext.SaveChanges();
            return Ok();
        }

        public IActionResult CommentStatusChangeToPassive(int id)
        {
            var value = _emailContext.Comments.Find(id);
            if (value == null) return NotFound();

            value.CommentStatus = "Pasif Yorum";
            _emailContext.SaveChanges();
            return Ok();
        }

        public IActionResult CommentStatusChangeToActive(int id)
        {
            var value = _emailContext.Comments.Find(id);
            if (value == null) return NotFound();

            value.CommentStatus = "Yorum onaylandı";
            _emailContext.SaveChanges();
            return Ok();
        }

        public IActionResult DeleteComment(int id)
        {
            var value = _emailContext.Comments.Find(id);
            if (value == null) return NotFound();

            _emailContext.Comments.Remove(value);
            _emailContext.SaveChanges();
            return Ok();
        }

    }
}
