using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public static class CommonExtensions
    {

        public static async Task<TResult> TimeoutAfter<TResult>(this Func<TResult> func, TimeSpan timeout)
        {
            var task = Task.Run(func);
            return await TimeoutAfterAsync(task, timeout);
        }

        private static async Task<TResult> TimeoutAfterAsync<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            var result = await Task.WhenAny(task, Task.Delay(timeout));
            if (result == task)
            {
                // Task completed within timeout.
                return task.GetAwaiter().GetResult();
            }
            else
            {
                // Task timed out.
                throw new TimeoutException();
            }
        }
        //public static IPrincipal sdgsdg(this IPrincipal gf)
        //{
        //    gf.IsInRole()
        //}
        public static int ToDeptCode(this int groupcode)
        {
            switch (groupcode)
            {
                case 1:
                    return 1;
                case 3:
                    return 2;
                case 5:
                    return 3;
                case 7:
                    return 4;
                case 9:
                    return 5;

            }

            return 0;
        }

        public static int ToDeptToGroupCode(this int deptcode)
        {
            switch (deptcode)
            {
                case 1:
                    return 1;
                case 2:
                    return 3;
                case 3:
                    return 5;
                case 4:
                    return 7;
                case 5:
                    return 9;
            }

            return 0;
        }

        public static string ToDeptToGroupName(this int deptcode)
        {
            switch (deptcode)
            {
                case 1:
                    return "ریاضی";
                case 2:
                    return "تجربی";
                case 3:
                    return "انسانی";
                case 4:
                    return "هنر";
                case 5:
                    return "زبان";
                default:
                    return "";
            }


        }

        //public static string ToTitleInUrl(this string title)
        //{
        //    var regexSpaces = new Regex(RegexPatterns.Spaces, RegexOptions.Multiline | RegexOptions.IgnoreCase);
        //    title = regexSpaces.Replace(title, " ");

        //    var regexSymbol = new Regex(RegexPatterns.Symbols, RegexOptions.Multiline | RegexOptions.IgnoreCase);

        //    var titleArticle = regexSymbol.Replace(title, "-").Replace(" ", "-");


        //    titleArticle = new Regex(@"(.)\1{2,}", RegexOptions.IgnoreCase).Replace(titleArticle, "$1");
        //    titleArticle = new Regex(@"(-)\1{1,}", RegexOptions.IgnoreCase).Replace(titleArticle, "$1");

        //    return titleArticle.Trim().TrimEnd('-').TrimStart('-');
        //}

        public static int PageCount(this int totalCount, int pageSize)
        {

            if (pageSize <= 0 || totalCount <= 0)
            {
                new Exception("PageSize zirooo");
            }

            int totalPage = totalCount / pageSize;

            //add the last page, ugly
            if (totalCount % pageSize != 0) totalPage++;

            return totalPage;
        }


        //public static IEnumerable<ListItem> GetSelectedItems(this ListControl checkBoxList)
        //{
        //    return from ListItem li in checkBoxList.Items where li.Selected select li;
        //}



        public static PersianDateTime ToPersianDateTime(this DateTime datetime)
        {
            return new PersianDateTime(datetime);
        }

        public static PersianDateTime ToPersianDateTime(this int date)
        {

            return new PersianDateTime(date);
        }


        /// <summary>
        /// converts one type to another
        /// Example:
        /// var age = "28";
        /// var intAge = age.To<int>();
        /// var doubleAge = intAge.To<double>();
        /// var decimalAge = doubleAge.To<decimal>();
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T To<T>(this IConvertible value)
        {
            try
            {
                Type t = typeof(T);
                Type u = Nullable.GetUnderlyingType(t);

                if (u != null)
                {
                    if (value == null || value.Equals(""))
                        return default(T);

                    return (T)Convert.ChangeType(value, u);
                }
                else
                {
                    if (value == null || value.Equals(""))
                        return default(T);

                    return (T)Convert.ChangeType(value, t);
                }
            }

            catch (Exception ex)
            {
                ErrorLog.Error(ex);
                return default(T);
            }
        }




        public static T To<T>(this IConvertible value, IConvertible ifError)
        {
            try
            {
                Type t = typeof(T);
                Type u = Nullable.GetUnderlyingType(t);

                if (u != null)
                {
                    if (value == null || value.Equals(""))
                        return (T)ifError;

                    return (T)Convert.ChangeType(value, u);
                }
                else
                {
                    if (value == null || value.Equals(""))
                        return (T)(ifError.To<T>());

                    return (T)Convert.ChangeType(value, t);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Error(ex);
                if (ifError == null)
                {
                    return default(T);
                }
                return (T)Convert.ChangeType(ifError, typeof(T));
            }

        }

        /// <summary>
        /// c# version of "Between" clause of sql query.
        /// Example:
        /// DateTime today = DateTime.Now;
        /// var from = new DateTime(2012, 2, 1);
        /// var to = new DateTime(2012, 12, 20);
        ///
        /// bool isBetween = today.Between(from, to);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="from">Min</param>
        /// <param name="to">Max</param>
        /// <returns></returns>
        public static bool Between<T>(this T value, T from, T to) where T : IComparable<T>
        {
            return value.CompareTo(from) >= 0 && value.CompareTo(to) <= 0;
        }

        /// <summary>
        /// C# version of In clause of sql query.
        /// Example:
        /// string value = "net";
        ///    bool isIn = value.In("dot", "net", "languages"); //true
        ///    isIn = value.In("dot", "note", "languages"); //false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool In<T>(this T value, params T[] list)
        {
            return list.Contains(value);
        }


        /// <summary>
        /// Converts any type to another.
        /// Example:
        /// string a = "1234";
        /// int b = a.ChangeType<int>(); //Successful conversion to int (b=1234)
        /// string c = b.ChangeType<string>(); //Successful conversion to string (c="1234")
        /// string d = "foo";
        /// int e = d.ChangeType<int>(); //Exception System.InvalidCastException
        /// int f = d.ChangeType(0); //Successful conversion to int (f=0)
        /// </summary>
        /// <typeparam name="TU"></typeparam>
        /// <param name="source"></param>
        /// <param name="returnValueIfException"></param>
        /// <returns></returns>
        public static TU ChangeType<TU>(this object source, TU returnValueIfException)
        {
            try
            {
                return source.ChangeType<TU>();
            }
            catch (Exception ex)
            {
                ErrorLog.Error(ex);
                return returnValueIfException;
            }
        }


        /// <summary>
        /// Extension method that tries to parse the string, if parsing faild it returns the default value (specified default value or implicit default value).
        /// </summary>
        /// <param name="this"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string @this, Guid defaultValue = default(Guid))
        {
            Guid x;
            if (Guid.TryParse(@this, out x))
                return x;
            else
                return defaultValue;
        }

        /// <summary>
        /// Converts any type to another.
        /// Example:
        /// string a = "1234";
        /// int b = a.ChangeType<int>(); //Successful conversion to int (b=1234)
        /// string c = b.ChangeType<string>(); //Successful conversion to string (c="1234")
        /// string d = "foo";
        /// int e = d.ChangeType<int>(); //Exception System.InvalidCastException
        /// int f = d.ChangeType(0); //Successful conversion to int (f=0)
        /// </summary>
        /// <typeparam name="TU"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TU ChangeType<TU>(this object source)
        {
            if (source is TU)
                return (TU)source;

            var destinationType = typeof(TU);
            if (destinationType.IsGenericType && destinationType.GetGenericTypeDefinition() == typeof(Nullable<>))
                destinationType = new NullableConverter(destinationType).UnderlyingType;

            return (TU)Convert.ChangeType(source, destinationType);
        }


        /// <summary>
        /// get one , or more random get
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IList<T> Shuffle<T>(this IList<T> list, int size = 1)
        {
            var r = new Random();
            var shuffledList =
                list.
                    Select(x => new { Number = r.Next(), Item = x }).
                    OrderBy(x => x.Number).
                    Select(x => x.Item).
                    Take(size); // Assume first @size items is fine

            return shuffledList.ToList();
        }
        public static T Shuffle<T>(this IList<T> list)
        {
            if (list == null) throw new ArgumentNullException("list");

            var r = new Random();
            var shuffledList =
                list.
                    Select(x => new { Number = r.Next(), Item = x }).
                    OrderBy(x => x.Number).
                    Select(x => x.Item).
                    Take(1); // Assume first @size items is fine

            return shuffledList.First();
        }

        /// <summary>
        ///  OrderBy Random
        /// </summary>
        /// <typeparam name="t"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IEnumerable<t> Randomize<t>(this IEnumerable<t> target)
        {
            Random r = new Random();

            return target.OrderBy(x => (r.Next()));
        }



    }
}
