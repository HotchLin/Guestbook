using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Guestbook.ViewModel;
using Guestbook.Models;
using System.Diagnostics;

namespace Guestbook.Controllers
{
	public class HomeController : Controller
	{
		private ModelContext db = new ModelContext();

		/// <summary>
		/// 首頁
		/// </summary>
		/// <returns></returns>
		public ActionResult Index()
		{
			HttpCookie cookie = Request.Cookies["memberid"];

			if (cookie != null)
			{
				int memberId = Convert.ToInt32(cookie.Values[0]);
				ViewData["username"] = db.Member.Find(memberId).Username;
			}

			var comment = db.Comment.OrderByDescending(x => x.CreateTime).ToList();

			db.Database.Log = (log) => Debug.WriteLine(log);

			return View(comment);
		}

		/// <summary>
		/// 註冊
		/// </summary>
		/// <param name="viewmodel"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Register(Register viewmodel)
		{

			var member = new Member()
			{
				Username = viewmodel.username,
				Email = viewmodel.email,
				Password = viewmodel.password,
				CreateTime = DateTime.Now
			};

			db.Member.Add(member);
			// 將Entity Framework 產生的 Sql 輸出到輸出的視窗
			db.Database.Log = (log) => Debug.WriteLine(log);

			db.SaveChanges();

			return View("Index");

		}

		/// <summary>
		/// 登入
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Login(string username, string password)
		{

			var member = db.Member.Where(x => x.Username == username && x.Password == password);

			if (member.Any())
			{
				int memberId = member.FirstOrDefault().MemberId;
				//產生一個Cookie
				HttpCookie cookie = new HttpCookie("memberid");
				//設定單值
				cookie.Value = memberId.ToString();
				//設定過期日
				cookie.Expires = DateTime.Now.AddDays(1);
				//寫到用戶端
				Response.Cookies.Add(cookie);
			}

			return RedirectToAction("Index");
		}


		/// <summary>
		/// 登出
		/// </summary>
		/// <returns></returns>
		public ActionResult Logout()
		{
			HttpCookie cookie = Request.Cookies["memberid"];
			cookie.Expires = DateTime.Now.AddDays(-1);
			Response.Cookies.Add(cookie);

			return RedirectToAction("Index");
		}

		/// <summary>
		/// 新增留言
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult PostComment(string message)
		{
			HttpCookie cookie = Request.Cookies["memberid"];
			if (cookie == null)
			{
				return Content("<script>alert('請先登入!');</script>");
			}
			int memberId = Convert.ToInt32(cookie.Values[0]);

			var comment = new Comment()
			{
				MemberId = memberId,
				Message = message,
				CreateTime = DateTime.Now,
				Member = db.Member.Find(memberId)
			};

			db.Comment.Add(comment);

			db.SaveChanges();

			return PartialView("Comment", comment);
		}

		/// <summary>
		/// 新增回復
		/// </summary>
		/// <param name="replyMessage"></param>
		/// <param name="commentId"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult PostCommentReply(string replyMessage, int commentId)
		{
			HttpCookie cookie = Request.Cookies["memberid"];
			if (cookie == null)
			{
				return Content("<script>alert('請先登入!');</script>");
			}

			int memberId = Convert.ToInt32(cookie.Values[0]);

			var commentReply = new CommentReply()
			{
				CommentId = commentId,
				MemberId = memberId,
				ReplyMessage = replyMessage,
				CreateTime = DateTime.Now,
				Member = db.Member.Find(memberId)

			};

			db.CommentReply.Add(commentReply);

			db.SaveChanges();

			return View("CommentReply", commentReply);
		}

		/// <summary>
		/// 刪除留言
		/// </summary>
		/// <param name="commentId"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult DeleteComment(int commentId)
		{
			// 刪除Comment也將CommentReply也一併刪除
			var comment = db.Comment.Include("CommentReply").Where(x => x.CommentId == commentId);

			db.Comment.RemoveRange(comment);

			db.Database.Log = (log) => Debug.WriteLine(log);

			db.SaveChanges();

			return Json(new { result = "Success" });
		}

		/// <summary>
		/// 編輯留言
		/// </summary>
		/// <param name="commentId"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult EditComment(int commentId, string message)
		{
			// 先查詢
			var commnent = db.Comment.Where(x => x.CommentId == commentId).Single();
			// 再修改內容
			commnent.Message = message;

			db.SaveChanges();

			return Content(commnent.Message);
		}


	}
}