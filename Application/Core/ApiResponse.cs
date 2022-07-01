using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Core
{

    //  Class that will embed al;l the failure and success message to result.
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }

        public T Value { get; set; }

        public string Error { get; set; }

        public static ApiResponse<T> Success(T value) => new ApiResponse<T> { IsSuccess = true, Value = value };

        public static ApiResponse<T> Failure(string error) => new ApiResponse<T> { IsSuccess = false, Error = error };


    }
}