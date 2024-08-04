using ElectionProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace ElectionProject.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        election_listEntities1 db = new election_listEntities1();

        public ActionResult Login()
        {
            if (Session["National_ID"] == null)
                return View();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult Login(string national_id)
        {

            var user = db.voter_user.FirstOrDefault(u => u.national_id == national_id);
            if (user != null && user.first_login == true)
            {
                // Generate and send verification code
                Random random = new Random();
                int randomCode = random.Next(100000, 1000000);
                Session["ConfCode"] = randomCode.ToString();


                // Email settings
                string fromEmail = "techlearnhub.contact@gmail.com";
                string toEmail = "mohammaddfawareh@gmail.com";
                string subjectText = "Your Confirmation Code";
                string messageText = $"Your confirmation code is {randomCode}";

                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string smtpUsername = "techlearnhub.contact@gmail.com";
                string smtpPassword = "lyrlogeztsxclank";

                // Send the email
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(fromEmail);
                    mailMessage.To.Add(toEmail);
                    mailMessage.Subject = subjectText;
                    mailMessage.Body = messageText;
                    mailMessage.IsBodyHtml = false;

                    using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
                    {
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                        smtpClient.EnableSsl = true;

                        smtpClient.Send(mailMessage);

                        Session["National_ID"] = user.national_id.ToString();

                        ViewBag.Emailsent = "The code has been sent to your Email";

                        return RedirectToAction("VerifyCode");
                    }
                }
            }

            else if (user.first_login == false)
            {
                return RedirectToAction("LoginWithPassword");
            }
            else
            {
                ModelState.AddModelError("", "National ID not found.");
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session["National_ID"] = null;
            return RedirectToAction("Index", "Home");
        }




        public ActionResult VerifyCode()
        {
            return View();
        }

        // POST: User/VerifyCode
        [HttpPost]
        public ActionResult VerifyCode(string verificationCode)
        {
            var sentCode = (string) Session["ConfCode"];
            if (verificationCode == sentCode)
            {
                return RedirectToAction("ResetPassword");
            }

            ModelState.AddModelError("", "Invalid verification code.");
            return View();
        }



        // GET: User/ResetPassword
        public ActionResult ResetPassword()
        {
            return View();
        }

        // POST: User/ResetPassword
        [HttpPost]
        public ActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            if (newPassword == confirmPassword)
            {

                var nationalId = Session["National_ID"];
                var user = db.voter_user.FirstOrDefault(u => u.national_id == nationalId);
                if (user != null)
                {
                    user.password = newPassword;
                    user.first_login = false;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("LoginWithPassword");
                }
            }

            ModelState.AddModelError("", "Passwords do not match.");
            return View();
        }


        public ActionResult LoginWithPassword()
        {
            if (Session["National_ID"] == null)
                return View();
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public ActionResult LoginWithPassword(voter_user newUser)
        {
            Session["National_ID"] = newUser.national_id.ToString();
            var user = db.voter_user.FirstOrDefault(u => u.national_id == newUser.national_id && u.password == newUser.password);
            if (user != null)
            {

                return RedirectToAction("Circles");
            }

            return View();
        }

        public ActionResult Circles()
        {
            if (Session["National_ID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var nationalId = (string)Session["National_ID"];
            var user = db.voter_user.FirstOrDefault(u => u.national_id == nationalId);
            var userDistrict = user.district_id;

            var circles = db.districts.Find(userDistrict);

            ViewBag.Circle = circles.name;
            return View();
        }

        public ActionResult ListsType()
        {
            if (Session["National_ID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var nationalId = (string)Session["National_ID"];
            var user = db.voter_user.FirstOrDefault(u => u.national_id == nationalId);
            if (user.has_locally_voted)
            {
                ViewBag.display1 = "none";
                ViewBag.voted1 = "لقد قمت بالتصويت لهذه القائمة";
            }
            if (user.has_party_voted)
            {
                ViewBag.display2 = "none";
                ViewBag.voted2 = "لقد قمت بالتصويت لهذه القائمة";
            }
            return View();

        }

        public ActionResult ListsNames()
        {
            if (Session["National_ID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var nationalId = (string)Session["National_ID"];
            var user = db.voter_user.FirstOrDefault(u => u.national_id == nationalId);
            var userDistrict = user.district_id;
            var electionList = db.election_list
                .Where(list => list.district_id == userDistrict)
                .Include(list => list.candidates)  // Assuming 'candidates' is the navigation property
                .ToList();



            return View(electionList);

        }


        [HttpPost]
        public ActionResult SingleList(int? election_list_id)
        {
            if (Session["National_ID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var nationalId = Session["National_ID"];
            var user = db.voter_user.FirstOrDefault(u => u.national_id == nationalId);
            if (election_list_id == null)
            {
                user.has_locally_voted = true;
                db.voter_user.AddOrUpdate(user);
                db.SaveChanges();
                return RedirectToAction("ListsType");
            }
            ViewBag.lsitName = db.election_list.Find(election_list_id).name;
            var electionList = db.election_list.Find(election_list_id).candidates.ToList();
            Session["election_list_id"] = election_list_id;
            return View(electionList); // Pass the electionList to the view
        }

        [HttpPost]
        public ActionResult SaveVote(string[] candidates)
        {
            if (Session["National_ID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var nationalId = (string)Session["National_ID"];
            var user = db.voter_user.FirstOrDefault(u => u.national_id == nationalId);
            user.has_locally_voted = true;
            db.voter_user.AddOrUpdate(user);
            foreach (var candidate in candidates)
            {
                var c = db.candidates.Find(Convert.ToInt32(candidate));
                c.vote_count = c.vote_count + 1;
                db.candidates.AddOrUpdate(c);
            }
            var election_list_id = (int)Session["election_list_id"];
            var election_list = db.election_list.Find(election_list_id);
            election_list.vote_count += 1;
            db.election_list.AddOrUpdate(election_list);
            db.SaveChanges();
            return RedirectToAction("ListsType");
        }


        public ActionResult PartyListsNames()
        {
            if (Session["National_ID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var nationalId = (string)Session["National_ID"];
            var user = db.voter_user.FirstOrDefault(u => u.national_id == nationalId);
            var userDistrict = user.district_id;
            var electionList = db.election_list
                .Where(list => list.district_id == null)
                .Include(list => list.candidates)  // Assuming 'candidates' is the navigation property
                .ToList();



            return View(electionList);

        }

        public ActionResult PartySaveVote(int election_list_id)
        {
            if (Session["National_ID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var nationalId = (string)Session["National_ID"];
            var user = db.voter_user.FirstOrDefault(u => u.national_id == nationalId);
            user.has_party_voted = true;
            db.voter_user.AddOrUpdate(user);

            var c = db.election_list.Find((election_list_id));
            c.vote_count = c.vote_count + 1;
            db.Entry(c).State = EntityState.Modified;
            db.SaveChanges();


            return RedirectToAction("ListsType");
        }
    }
}
