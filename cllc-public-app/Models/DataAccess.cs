﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Models
{
    public class DataAccess
    {
        MongoClient _client;
        IMongoDatabase _db;

        const string SURVEY_COLLECTION = "Surveys";
        const string RESULT_COLLECTION = "Results";
        const string USER_COLLECTION = "Users";
        const string JURISDICTION_COLLECTION = "Jurisdiction";

        public DataAccess(string connectionString, string databaseName)
        {
            _client = new MongoClient(connectionString);
            _db = _client.GetDatabase(databaseName);
        }

        public void AddJurisdiction(Jurisdiction jurisdiction)
        {
            // create a new jurisdiction.           
            _db.GetCollection<Models.Jurisdiction>(JURISDICTION_COLLECTION).InsertOne(jurisdiction);
        }

        public Dictionary<string, string> GetSurveys()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            List<ChangeSurvey> surveys = _db.GetCollection<Models.ChangeSurvey>(SURVEY_COLLECTION).Find(new BsonDocument()).ToList();

            foreach (ChangeSurvey survey in surveys)
            {
                result.Add(survey.Id.ToString(), survey.Json);
            }

            return result;
        }

        /// <summary>
        /// Returns a specific jurisdiction
        /// </summary>
        /// <param name="name">The name of the jurisdiction</param>
        /// <returns>The jurisdiction, or null if it does not exist.</returns>
        public Jurisdiction GetJurisdictionByName(string name)
        {
            Jurisdiction jurisdiction = _db.GetCollection<Models.Jurisdiction>(JURISDICTION_COLLECTION).Find(x => x.Name == name).FirstOrDefault();
            return jurisdiction;
        }



        public Dictionary<string, List<string>> GetResults()
        {
            Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();

            List<ChangeSurvey> surveys = _db.GetCollection<Models.ChangeSurvey>(SURVEY_COLLECTION).Find(new BsonDocument()).ToList();

            foreach (ChangeSurvey survey in surveys)
            {
                List<string> surveyResults = new List<string>();
                List<PostSurveyResult> items = _db.GetCollection<Models.PostSurveyResult>(RESULT_COLLECTION).Find(x => x.surveyId == survey.Id).ToList();
                foreach (PostSurveyResult item in items)
                {
                    surveyResults.Add(item.surveyResult);
                }
                results.Add(survey.Name, surveyResults);
            }

            return results;
        }

        public string GetSurvey(string surveyId)
        {
            return GetSurveys()[surveyId];
        }

        public void StoreSurvey(string surveyId, string jsonString)
        {
            ChangeSurvey survey = _db.GetCollection<Models.ChangeSurvey>(SURVEY_COLLECTION).Find(x => x.Name == surveyId).FirstOrDefault();
            if (survey == null)
            {
                survey = new ChangeSurvey();
                survey.Name = surveyId;
            }
            survey.Json = jsonString;
        }

        public void ChangeName(string id, string name)
        {
            var survey = _db.GetCollection<Models.ChangeSurvey>(SURVEY_COLLECTION).Find(x => x.Name == name);
        }

        public void DeleteSurvey(string surveyId)
        {
            var survey = _db.GetCollection<Models.ChangeSurvey>(SURVEY_COLLECTION).DeleteOne(x => x.Name == surveyId);
        }

        public void PostResults(string postId, string resultJson)
        {            
            // create a new result.
            PostSurveyResult psr = new PostSurveyResult();
            psr.postId = postId;
            psr.surveyResult = resultJson;
            _db.GetCollection<Models.PostSurveyResult>(RESULT_COLLECTION).InsertOne(psr);           
        }

        /// <summary>
        /// Get survey results for a given survey
        /// </summary>
        /// <param name="postId">The name of a survey</param>
        /// <returns></returns>
        public List<string> GetResults(string postId)
        {
            List<string> result = new List<string>();
            List<Models.PostSurveyResult> items = _db.GetCollection<Models.PostSurveyResult>(RESULT_COLLECTION).Find(x => x.postId == postId).ToList();
            foreach (var item in items)
            {
                result.Add(item.surveyResult);
            }
            return result;
        }

        public User GetUserBySmUserId(string SmUserId)
        {
            User result =_db.GetCollection<Models.User>(USER_COLLECTION).Find(e => e.SmUserId == SmUserId).FirstOrDefault();
            return result;
        }

        public User GetUserByGuid(string Guid)
        {
            User result = _db.GetCollection<Models.User>(USER_COLLECTION).Find(e => e.Guid == Guid).FirstOrDefault();
            return result;
        }

        public void SaveUser(User user)
        {
            _db.GetCollection<Models.User>(USER_COLLECTION).ReplaceOne(e => e.Id == user.Id, user);
        }

        public void UpdateJurisdiction(Jurisdiction jurisdiction)
        {
            _db.GetCollection<Models.Jurisdiction>(JURISDICTION_COLLECTION).ReplaceOne(e => e.Id == jurisdiction.Id, jurisdiction);

        }
    }
}