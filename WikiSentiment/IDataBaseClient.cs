using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiSentiment
{
    public interface IDataBaseClient
    {
        Task Upload(DateTime date, string content);
        Task<string> Load(DateTime _date);
    }
}
