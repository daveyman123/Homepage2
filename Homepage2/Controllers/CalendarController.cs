﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Homepage2.Controllers
{
    public class CalendarController : Controller
    {
        // GET: Home
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        private DefaultConnection _context;

        public CalendarController()
        {
            _context = new DefaultConnection();
        }

        public JsonResult GetEvents()
        {
            using (DefaultConnection dc = new DefaultConnection())
            {
                var userID = User.Identity.GetUserId();
                var events = (from e in _context.Events
                             where e.userID == userID
                             select e).ToList();
                
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [HttpPost]
        public JsonResult SaveEvent(Event e)
        {
            e.userID = User.Identity.GetUserId();
            var status = false;
            using (DefaultConnection dc = new DefaultConnection())
            {
                if (e.EventID == 0 && e.Freq > 0)
                {
                    DateTime stDate = e.Start;
                    DateTime EnDate = e.End ?? e.Start;
                    int i = 0;
                    do
                    {
                        Event _e = new Event();
                        if (e.Freq == 1)
                        {
                            stDate = stDate.AddDays(7);
                            EnDate = EnDate.AddDays(7);
                            _e.Start = stDate;
                            _e.End = EnDate;
                        }
                        if (e.Freq == 2)
                        {
                            stDate = stDate.AddMonths(1);
                            EnDate = EnDate.AddMonths(1);
                            _e.Start = stDate;
                            _e.End = EnDate;
                        }
                        if (e.Freq == 3)
                        {
                            stDate = stDate.AddYears(1);
                            EnDate = EnDate.AddYears(1);
                            _e.Start = stDate;
                            _e.End = EnDate;
                        }

                        _e.Subject = e.Subject;
                        _e.Description = e.Description;
                        _e.IsFullDay = e.IsFullDay;
                        _e.ThemeColor = e.ThemeColor;
                        _e.Freq = e.Freq;
                        _e.userID = User.Identity.GetUserId();
                        dc.Events.Add(_e);

                        i++;
                    } while (i < 25);
                }

                if (e.EventID > 0)
                {
                    //Update the event
                    var v = dc.Events.Where(a => a.EventID == e.EventID).FirstOrDefault();
                    if (v != null)
                    {
                        v.Subject = e.Subject;
                        v.Start = e.Start;
                        v.End = e.End;
                        v.Description = e.Description;
                        v.IsFullDay = e.IsFullDay;
                        v.ThemeColor = e.ThemeColor;
                        v.Freq = e.Freq;
                        v.userID = e.userID;
                    }
                }
                else
                {

                    dc.Events.Add(e);
                }

                dc.SaveChanges();
                status = true;

            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            var status = false;
            using (DefaultConnection dc = new DefaultConnection())
            {
                var v = dc.Events.Where(a => a.EventID == eventID).FirstOrDefault();
                if (v != null)
                {
                    dc.Events.Remove(v);
                    dc.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }



    }
}