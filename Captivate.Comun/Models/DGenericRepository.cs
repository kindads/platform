using Captivate.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using DapperExtensions;
using DapperExtensions.Sql;
using System.Configuration;


namespace Captivate.Common.Models
{
    public abstract class DGenericRepository<T> :
    IDapperGenericRepository<T> where T : class , new()
    {        
        private readonly string _connectionString;
        public DGenericRepository(string connectionStringName = "KindAdsDefaultConnection")
        {
            _connectionString= ConfigurationManager.ConnectionStrings[connectionStringName].ToString();
        }

        protected SqlConnection DBConnection
        {
  
            get { return new SqlConnection(_connectionString); }
            private set { }
        }


        public virtual IEnumerable<T> GetAll()
        {
            using (var con = DBConnection)
            {
                con.Open();
                var lista = con.GetList<T>().ToList();                
                con.Close();
                return lista;
            }
                
        }

        public virtual T FindById<TI>(TI id)
        {
            return DBConnection.Get<T>(id);
        }

        public virtual void Add(T entity)
        {
            using (var con = DBConnection)
            {
                con.Open();
                con.Insert<T>(entity);
                
                con.Close();
            }
        }

        public virtual void Delete(T entity)
        {
            using (var con = DBConnection)
            {
                con.Open();
                con.Delete<T>(entity);
                con.Close();
            }
        }

        public virtual void Edit(T entity)
        {
            using (var con = DBConnection)
            {
                con.Open();
                con.Update<T>(entity);
                con.Close();
            }
        }

        protected void LoadProfileMapper(Type type)
        {
            DapperExtensions.DapperExtensions.SetMappingAssemblies(new[] { type.Assembly });
        }

    }
}
