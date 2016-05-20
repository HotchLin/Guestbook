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

		public ActionResult Index()
		{
			var comment = db.Comment.OrderByDescending(x => x.CreateTime).ToList();


			


			return View(comment);
		}

		/// <summary>
		/// 註冊
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Register(Register viewmodel)
		{
			var member = new Member()
			{
				Uesrname = viewmodel.username,
				Email = viewmodel.email,
				Password  = viewmodel.password,
				CreateTime= DateTime.Now
			};

			db.Member.Add(member);
			db.Database.Log = (log) => Debug.WriteLine(log);
			db.SaveChanges();

			return View("Index");
		}

		public ActionResult PostComment(string message)
		{
			var comment = new Comment()
			{
				MemberId = 1,
				Message = message,
				CreateTime= DateTime.Now
			};

			db.Comment.Add(comment);
			db.SaveChanges();

			//int commentid= comment.CommentId


			return PartialView("Comment", comment);
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}