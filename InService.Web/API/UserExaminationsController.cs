using InService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using InService.Lib.Data;
using Newtonsoft.Json;

namespace InService.Web.API
{
    public class UserExaminationsController : SysController
    {
        [Authorize]
        public IEnumerable<IUserExamination> Get()
        {
            var exams = new List<IUserExamination>();
            foreach (var item in CurrentUser.UserExaminations.OrderByDescending(c => c.CreationDate)) exams.Add(item.IUserExamination);
            return exams;
        }
        //[Authorize]
        //public IUserExamination Post([FromBody] IUserExamination examination)
        //{
        //    var currentExam = DB.UserExaminations.FirstOrDefault(c => c.ID == examination.ID);
        //    if (currentExam == null)
        //    {
        //        this.Request.ToString();
        //        currentExam = new UserExamination
        //        {
        //            CreationDate = examination.CreationDate,
        //            EndTime = examination.EndTime,
        //            Latitude = examination.Latitude,
        //            Longitude = examination.Longitude,
        //            StartTime = examination.StartTime,
        //            UserID = examination.UserID,
        //            ExaminationID = examination.ExaminationID,
        //        };

        //        DB.UserExaminations.Add(currentExam);
        //        var result = JsonConvert.DeserializeObject<List<IUserExaminationDetail>>(examination.DetailsJson);
        //        if (result != null)
        //        {
        //            foreach (var item in result)
        //            {
        //                if (item.QuestionID > 0 && item.AnswerID > 0)
        //                {
        //                    currentExam.UserExaminationDetails.Add(new UserExaminationDetail
        //                    {
        //                        QuestionID = item.QuestionID,
        //                        AnswerID = item.AnswerID,
        //                    });
        //                }
        //            }
        //        }
        //        DB.SaveChanges();
        //    }
        //    var Examination = currentExam.IUserExamination;
        //    Examination.ID = examination.ID;
        //    return Examination;
        //}


        [Authorize]
        public FUserExamination Post(FUserExamination fUserExamination)
        {
            var Items = fUserExamination.Details;
            var Invoice = fUserExamination.Examination;
            var inv = DB.UserExaminations.FirstOrDefault(c => c.ID == fUserExamination.Examination.ID);
            if (inv != null) return inv.FUserExamination;
            inv = new UserExamination
            {
                CreationDate = Invoice.CreationDate,
                EndTime = Invoice.EndTime,
                Latitude = Invoice.Latitude,
                Longitude = Invoice.Longitude,
                StartTime = Invoice.StartTime,
                UserID = Invoice.UserID,
                ExaminationID = Invoice.ExaminationID,
                ID = Invoice.ID,
            };
            DB.UserExaminations.Add(inv);
            foreach (var item in Items)
            {
                inv.UserExaminationDetails.Add(new UserExaminationDetail
                {
                    QuestionID = item.QuestionID,
                    AnswerID = item.AnswerID,
                    ExaminationID = item.ExaminationID,
                    Name = item.Name,
                    ID = item.ID,
                });
            }
            DB.SaveChanges();
            return inv.FUserExamination;
        }
    }
}
