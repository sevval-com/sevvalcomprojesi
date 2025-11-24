using Sevval.Application.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Features.Common
{

    public class ApiResponse<T>
    {
        public bool IsSuccessfull { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
        public T Data { get; set; }

        public MetaData Meta { get; set; }

        public List<ApiError> Errors { get; set; }

        public ApiResponse()
        {
            Errors = new List<ApiError>();
        }
    }
    public class ApiResponse: ErrorStatusCode
    {
        public bool IsSuccessfull { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
        public MetaData Meta { get; set; }

        public List<ApiError> Errors { get; set; }

        public ApiResponse()
        {
            Errors = new List<ApiError>();
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class ApiError
    {
        public string Field { get; set; }
        public string Message { get; set; }
    }

    public class MetaData
    {
        public string RequestId { get; set; }
        public DateTime Timestamp { get; set; }
        public Pagination Pagination { get; set; }
    }

    public class Pagination
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalPage { get; set; }
        public int TotalItem { get; set; }
    }
}
