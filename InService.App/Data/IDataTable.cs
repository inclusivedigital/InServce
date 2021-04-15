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
using SQLite;

namespace InService.App.Data
{
    class IDataTable<T> where T : new()
    {
        public string DBPath { get; protected set; }

        public IDataTable(string path) => DBPath = path;

        public IDataTable() => DBPath = $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)}/{nameof(T).ToLower()}.db";

        SQLiteConnection _Conn;
        public virtual SQLiteConnection Conn
        {
            get
            {
                if (_Conn == null)
                {
                    _Conn = new SQLiteConnection(DBPath);
                    _Conn.CreateTable<T>();
                }
                return _Conn;
            }
        }

        SQLiteAsyncConnection _AsyncConn;
        public virtual SQLiteAsyncConnection AsyncConn
        {
            get
            {
                if (_AsyncConn == null)
                {
                    Conn.CreateTable<T>();
                    _AsyncConn = new SQLiteAsyncConnection(DBPath);
                }
                return _AsyncConn;
            }
        }

        public TableQuery<T> Rows => Conn.Table<T>();

        public AsyncTableQuery<T> RowsAsync => AsyncConn.Table<T>();

        public virtual void Insert(T item) => Conn.Insert(item);

        public virtual Task InsertAsync(T item) => AsyncConn.InsertAsync(item);

        public virtual void Update(T item) => Conn.Update(item);

        public virtual void Insert(List<T> items) => Conn.InsertAll(items);

        public virtual Task InsertAsync(List<T> items) => AsyncConn.InsertAllAsync(items);

        public virtual void Truncate() => Conn.DeleteAll<T>();
    }
}