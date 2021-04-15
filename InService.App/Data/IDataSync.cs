using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using InService.App.Auth;
using Newtonsoft.Json;

namespace InService.App.Data
{
    abstract class IDataSync
    {
        public HttpClient HttpClient { get; protected set; }
        public event EventHandler<DataSyncEventArgs> ProgressChanged;
        public event EventHandler DataSyncCompleted;
        public event EventHandler<DataSyncEventArgs> DataSyncFailed;
        public int ProgressMax { get; protected set; }
        public bool IsBusy { get; protected set; }
        public bool SyncSucceeded { get; set; }

        protected virtual void OnProgressChanged(DataSyncEventArgs e) => ProgressChanged?.Invoke(this, e);

        protected virtual void OnDataSyncFailed(DataSyncEventArgs e)
        {
            IsBusy = false;
            SyncSucceeded = false;
            DataSyncFailed?.Invoke(this, e);
        }

        protected virtual void OnDataSynCompleted(DataSyncEventArgs e)
        {
            IsBusy = false;
            SyncSucceeded = true;
            DataSyncCompleted?.Invoke(this, e);
        }

        public IDataSync(HttpClient client) => HttpClient = client;
        public IDataSync() => HttpClient = SessionManager.GetHttpClient();

        public abstract Task<bool> Download();
        public abstract Task<bool> Upload();
        public abstract Task Sync();

        protected virtual async Task<bool> Download<T>(IDataTable<T> DB, string API, int Progress, bool truncate = true) where T : new()
        {
            try
            {
                var response = await HttpClient.GetAsync(SessionManager.GetAPIURL(API));
                if (response != null && response.IsSuccessStatusCode)
                {
                    var responseMsg = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<List<T>>(responseMsg);
                    if (truncate) DB.Truncate();
                    var type = typeof(T);
                    if (type == typeof(UserExamination) || type == typeof(UserExaminationDetail))
                    {
                        foreach (var item in data) DB.Conn.Insert(item);
                    }
                    else
                        foreach (var item in data) await DB.AsyncConn.InsertOrReplaceAsync(item);
                    OnProgressChanged(new DataSyncEventArgs() { Progress = Progress, Message = $"Loading {API}.." });
                    return true;
                }
                else
                {
                    OnDataSyncFailed(new DataSyncEventArgs() { Progress = Progress, Message = response?.ReasonPhrase, State = response });
                    return false;
                }
            }
            catch (Exception ex)
            {
                OnDataSyncFailed(new DataSyncEventArgs() { Progress = Progress, Message = ex.Message });
                return false;
            }
        }

        protected virtual async Task<List<T>> Download<T>(string API, int Progress) where T : new()
        {
            try
            {
                var response = await HttpClient.GetAsync(SessionManager.GetAPIURL(API));
                if (response != null && response.IsSuccessStatusCode)
                {
                    var responseMsg = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<List<T>>(responseMsg);
                    OnProgressChanged(new DataSyncEventArgs() { Progress = Progress, Message = $"Loading {API}.." });
                    return data;
                }
                else
                {
                    OnDataSyncFailed(new DataSyncEventArgs() { Progress = Progress, Message = response?.ReasonPhrase, State = response });
                    return null;
                }
            }
            catch (Exception ex)
            {
                OnDataSyncFailed(new DataSyncEventArgs() { Progress = Progress, Message = ex.Message });
                return null;
            }
        }

        protected async Task<(bool, T)> Upload<T>(T item, string API, int Progress)
        {
            try
            {
                var json = JsonConvert.SerializeObject(item);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await HttpClient.PostAsync(SessionManager.GetAPIURL(API), content);
                if (response != null && response.IsSuccessStatusCode)
                {
                    var responseMsg = await response.Content.ReadAsStringAsync();
                    var respItem = JsonConvert.DeserializeObject<T>(responseMsg);
                    OnProgressChanged(new DataSyncEventArgs() { Progress = Progress });
                    return (true, respItem);
                }
                else
                {
                    OnDataSyncFailed(new DataSyncEventArgs() { Progress = Progress, Message = response?.ReasonPhrase, State = response });
                    return (false, item);
                }
            }
            catch (Exception ex)
            {
                OnDataSyncFailed(new DataSyncEventArgs() { Progress = Progress, Message = ex.Message });
                return (false, item);
            }
        }

        protected async Task<(bool, List<T>)> Upload<T>(List<T> items, string API, int Progress)
        {
            try
            {
                var json = JsonConvert.SerializeObject(items);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var req = new HttpRequestMessage(HttpMethod.Post, SessionManager.GetAPIURL(API))
                {
                    Content = content,
                };
                var response = await HttpClient.SendAsync(req);
                if (response != null && response.IsSuccessStatusCode)
                {
                    List<T> data = null;
                    try
                    {
                        var responseMsg = await response.Content.ReadAsStringAsync();
                        data = JsonConvert.DeserializeObject<List<T>>(responseMsg);
                    }
                    catch { }
                    OnProgressChanged(new DataSyncEventArgs() { Progress = Progress });
                    return (true, data);
                }
                else
                {
                    OnDataSyncFailed(new DataSyncEventArgs() { Progress = Progress, Message = response?.ReasonPhrase, State = response });
                    return (false, null);
                }
            }
            catch (Exception ex)
            {
                OnDataSyncFailed(new DataSyncEventArgs() { Progress = Progress, Message = ex.Message });
                return (false, null);
            }
        }

        public void Complete()
        {
            IsBusy = false;
            OnDataSynCompleted(null);
        }
    }

    class DataSyncEventArgs : EventArgs
    {
        public int Progress;
        public string Message;
        public object State;
    }
}