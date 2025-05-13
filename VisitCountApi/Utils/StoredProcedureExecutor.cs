using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace KanoonIr.Infrastructure.DataBase
{
    public class StoredProcedureExecutor 
    {
        private readonly DbContext _context;

        public StoredProcedureExecutor(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// اجرای یک Stored Procedure که لیست نتایج برمی‌گردونه.
        /// </summary>
        /// <typeparam name="T">کلاس مدل مورد نظر</typeparam>
        /// <param name="storedProcName">نام SP</param>
        /// <param name="parameters">پارامترهای ورودی</param>
        /// <returns>لیست نتایج به صورت مدل T</returns>
        public async Task<List<T>> ExecuteStoredProcedureAsync<T>(string storedProcName, IEnumerable<SqlParameter>? parameters = null) where T : class
        {
            if (string.IsNullOrWhiteSpace(storedProcName))
                throw new ArgumentException("نام Stored Procedure باید وارد شود.", nameof(storedProcName));

            var sql = BuildSqlCommand(storedProcName, parameters);
            var sqlParameters = parameters?.ToArray() ?? Array.Empty<SqlParameter>();

            return await _context.Set<T>().FromSqlRaw(sql, sqlParameters).ToListAsync();
        }

        /// <summary>
        /// اجرای یک Stored Procedure که تغییراتی در دیتابیس ایجاد می‌کند (بدون بازگشت نتیجه).
        /// </summary>
        /// <param name="storedProcName">نام SP</param>
        /// <param name="parameters">پارامترهای ورودی</param>
        /// <returns>تعداد ردیف‌های تحت تاثیر عملیات</returns>
        public async Task<int> ExecuteNonQueryAsync(string storedProcName, IEnumerable<SqlParameter>? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(storedProcName))
                throw new ArgumentException("نام Stored Procedure باید وارد شود.", nameof(storedProcName));

            var sql = BuildSqlCommand(storedProcName, parameters);
            var sqlParameters = parameters?.ToArray() ?? Array.Empty<SqlParameter>();

            return await _context.Database.ExecuteSqlRawAsync(sql, sqlParameters);
        }

        /// <summary>
        /// اجرای یک Stored Procedure جهت دریافت مقدار اسکالر.
        /// </summary>
        /// <param name="storedProcName">نام SP</param>
        /// <param name="parameters">پارامترهای ورودی (می‌تواند شامل پارامترهای خروجی هم باشد)</param>
        /// <returns>نتیجه اسکالر</returns>
        public async Task<object?> ExecuteScalarAsync(string storedProcName, IEnumerable<SqlParameter>? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(storedProcName))
                throw new ArgumentException("نام Stored Procedure باید وارد شود.", nameof(storedProcName));

            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = storedProcName;
            command.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }
            }

            if (command.Connection.State != ConnectionState.Open)
                await command.Connection.OpenAsync();

            return await command.ExecuteScalarAsync();
        }

        /// <summary>
        /// ایجاد رشته SQL مناسب جهت اجرای Stored Procedure در EF Core.
        /// در این متد تنها برای متدهای ExecuteStoredProcedureAsync و ExecuteNonQueryAsync استفاده می‌شود.
        /// </summary>
        /// <param name="storedProcName">نام SP</param>
        /// <param name="parameters">پارامترهای ورودی</param>
        /// <returns>رشته SQL برای فرمان EXEC</returns>
        private static string BuildSqlCommand(string storedProcName, IEnumerable<SqlParameter>? parameters)
        {
            if (parameters == null || !parameters.Any())
                return $"EXEC {storedProcName}";

            var paramNames = parameters.Select(p => p.ParameterName);
            return $"EXEC {storedProcName} {string.Join(", ", paramNames)}";
        }
    }
}
