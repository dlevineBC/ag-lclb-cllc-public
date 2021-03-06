﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class SurveyController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly DataAccess db;
        public SurveyController(DataAccess db, IConfiguration configuration)
        {
            Configuration = configuration;
            this.db = db;
        }
        [HttpGet("getActive")]
        public JsonResult GetActive()
        {
            
            return Json(db.GetSurveys());
        }

        [HttpGet("getSurvey")]
        public string GetSurvey(string surveyId)
        {
            return db.GetSurvey(surveyId);
        }

        [HttpGet("create")]
        public JsonResult Create(string name)
        {
            db.StoreSurvey(name, "{}");
            return Json("Ok");
        }

        [HttpGet("changeName")]
        public JsonResult ChangeName(string id, string name)
        {
            db.ChangeName(id, name);
            return Json("Ok");
        }

        [HttpPost("changeJson")]
        public string ChangeJson([FromBody]ViewModels.ChangeSurvey model)
        {
            db.StoreSurvey(model.Id, model.Json);
            return db.GetSurvey(model.Id);
        }

        [HttpGet("delete")]
        public JsonResult Delete(string id)
        {
            db.DeleteSurvey(id);
            return Json("Ok");
        }

        [HttpPost("post")]
        public JsonResult PostResult([FromBody]ViewModels.PostSurveyResult model)
        {
            db.PostResults(model.postId, model.surveyResult);
            return Json("Ok");
        }

        [HttpGet("results")]
        public JsonResult GetResults(string postId)
        {
            return Json(db.GetResults(postId));
        }
    }
}
