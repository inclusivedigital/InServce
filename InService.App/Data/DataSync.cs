using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using InService.App.Auth;
using InService.Lib.Data;

namespace InService.App.Data
{
    class DataSync : IDataSync
    {
        public DataSync() { ProgressMax = 22; }

        public bool SyncAll { get; set; } = true;

        public override Task Sync()
        {
            throw new NotImplementedException();
        }

        public async override Task<bool> Download()
        {
            int Progress = 0;
            try
            {
                if (SyncAll)
                {
                    if (!await Download(Answer.DB, "answers", Progress++)) return false;
                    if (!await Download(Article.DB, "articles", Progress++)) return false;
                    //  if (!await Download(Attachment.DB, "attachments", Progress++)) return false;
                    if (!await Download(Attachment.DB, "icons", Progress++)) return false;
                    if (!await Download(Branch.DB, "branches", Progress++)) return false;
                    if (!await Download(CoursePrice.DB, "coursePrices", Progress++)) return false;
                    if (!await Download(Course.DB, "courses", Progress++)) return false;
                    if (!await Download(CropCategory.DB, "cropCategories", Progress++)) return false;
                    if (!await Download(Crop.DB, "crops", Progress++)) return false;
                    if (!await Download(Currency.DB, "currencies", Progress++)) return false;
                    if (!await Download(ExaminationPrice.DB, "examinationPrices", Progress++)) return false;
                    if (!await Download(Examination.DB, "examinations", Progress++)) return false;
                    if (!await Download(Instruction.DB, "instructions", Progress++)) return false;
                    if (!await Download(LivestockCategory.DB, "livestockCategories", Progress++)) return false;
                    if (!await Download(Livestock.DB, "livestocks", Progress++)) return false;
                    if (!await Download(Module.DB, "modules", Progress++)) return false;
                    if (!await Download(NonValueChain.DB, "nonValueChains", Progress++)) return false;
                    if (!await Download(PaymentMethod.DB, "paymentMethods", Progress++)) return false;

                    //Progress = 18;
                    if (!await Download(Payment.DB, "payments", Progress++)) return false;
                    if (!await Download(Question.DB, "questions", Progress++)) return false;
                    if (!await Download(ValueChain.DB, "valueChains", Progress++)) return false;
                    if (!await Download(UserExamination.DB, "userExaminations", Progress++)) return false;
                    if (!await Download(UserExaminationDetail.DB, "userExaminationDetails", Progress++)) return false;
                    if (!await Download(Notice.DB, "notices", Progress++)) return false;
                }
                if (!await SyncExams())
                {
                    IsBusy = false;
                    return false;
                }
                OnDataSynCompleted(new DataSyncEventArgs());
                SessionManager.LastDataSyncDate = DateTime.Now;
                SessionManager.SyncAttachments = true;
                return true;
            }
            catch (Exception ex)
            {
                OnDataSyncFailed(new DataSyncEventArgs { State = ex, Progress = Progress });
            }
            return false;
        }

        public override Task<bool> Upload()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SyncExams()
        {
            var examinations = await UserExamination.DB.RowsAsync.Where(c => c.IsSynced == null || c.IsSynced == false).ToListAsync();
            if (examinations.Any())
            {
                var fInvoices = new List<FUserExamination>();
                foreach (var item in examinations.ToList())
                {
                    await item.LoadUserExaminationDetails();
                    if (item.UserExaminationDetails.Count == 0)
                    {
                        UserExamination.DB.Conn.Delete<UserExamination>(item.ID);
                        examinations.Remove(item);
                        continue;
                    }

                    var fExaminations = new FUserExamination { Examination = item.IUserExamination, Details = item.UserExaminationDetails.Select(i => i.IUserExaminationDetail).ToList() };
                    var (success, rExaminations) = await Upload(fExaminations, "userExaminations", 22);
                    if (success) item.IsSynced = true;
                    else break;
                }
                await UserExamination.DB.AsyncConn.UpdateAllAsync(examinations);
            }
            SessionManager.SyncInvoices = false;
            return true;
        }
    }
}