﻿using CloudApiVietnam.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CloudApiVietnam.Controllers
{
    [Authorize]
    public class FormContentController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET alle FormContent
        public HttpResponseMessage Get()
        {
            try
            {
                var formContent = db.FormContent.ToList();
                return Request.CreateResponse(HttpStatusCode.OK, formContent);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        // GET specefiek FormContent
        public HttpResponseMessage Get(int id)
        {
            var formContent = db.FormContent.Where(f => f.Id == id).FirstOrDefault();
            if (formContent == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No FormContent found with id: " + id.ToString());
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, formContent);
            }
        }

        // POST een FormContent
        public HttpResponseMessage Post(FormContentBindingModel formContentBindingModel)
        {
            try
            {
                IsJSON isJson = IsValidJson(formContentBindingModel.Content); // Check of JSON klopt en maak resultaat object
                if (!isJson.Status) // als resultaat object status fals is return error
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "JSON in 'content' is not correct JSON: " + isJson.Error);
                }

                var headersCheck = ContentEqeulsHeaders(formContentBindingModel);
                if (!headersCheck.Status)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, headersCheck.Error);
                }

                FormContent formContent = new FormContent();
                formContent.Content = formContentBindingModel.Content;
                formContent.FormulierenId = formContentBindingModel.FormId;


                db.FormContent.Add(formContent);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        // PUT FormContent by Id
        public HttpResponseMessage Put(int id, [FromBody]FormContentBindingModel UpdateObject)
        {
            var formContent = db.FormContent.Where(f => f.Id == id).FirstOrDefault();

            if (formContent == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No FormContent found with id: " + id.ToString());
            }
            else
            {
                formContent.FormulierenId = UpdateObject.FormId;
                formContent.Content = UpdateObject.Content;
                return Request.CreateResponse(HttpStatusCode.OK, formContent);
            }
        }

        // DELETE FormContent 
        public HttpResponseMessage Delete(int id)
        {
            var formContent = db.FormContent.Where(f => f.Id == id).FirstOrDefault();

            if (formContent == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No FormContent found with id: " + id.ToString());
            }
            else
            {
                db.FormContent.Remove(formContent);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
        }


        private static IsJSON IsValidJson(string strInput)
        {
            IsJSON result = new IsJSON();
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    result.Status = true;
                    return result;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    result.Error = jex.Message;
                    result.Status = false;
                    return result;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    result.Error = ex.ToString();
                    result.Status = false;
                    return result;
                }
            }
            else
            {
                result.Status = false;
                result.Error = "JSON doesn't start or and with with '{/}' or '[/]' ";
                return result;
            }
        }

        private ContentEqeulsHeadersCheck ContentEqeulsHeaders(FormContentBindingModel formContentBindingModel)
        {
            ContentEqeulsHeadersCheck result = new ContentEqeulsHeadersCheck();
            var Formulier = db.Formulieren.Where(f => f.Id == formContentBindingModel.FormId).FirstOrDefault(); //Haalt bijbehorende formulier op

            var obj = JToken.Parse(formContentBindingModel.Content); //Maak object van mee gegeven content

            foreach (var item in obj) //loop door mee gegeven content
            {
                //Pak de propery naam
                string jsonName = item.First.Path.ToString();
                var splitPath = jsonName.Split('.');
                string propertyName = splitPath[1];

                if (!Formulier.FormTemplate.Contains(propertyName))
                {
                    result.Status = false;
                    result.Error = "'" + propertyName+ "'" + " is not found in the headers of the matching Formulier";
                    return result;
                }
            }
            result.Status = true;
            return result;
        }
    }
}
