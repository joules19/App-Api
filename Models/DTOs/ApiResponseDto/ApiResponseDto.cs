using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel;

namespace api_app.Models.DTOs.ApiResponseDto
{
    public enum ResponseDataFormat
    {
        [Description("String")]
        String = 1,
        [Description("Object")]
        Object = 2,
        [Description("Object-List")]
        ObjectList
    }

    public enum ResponseStatus
    {
        [Description("Successful")]
        Success,
        [Description("Failed")]
        Failure,
        [Description("Processing")]
        Processing
    }



    public class ApiResponseDto
    {

        public ApiResponseDto(string message, ResponseStatus status, List<string> error = null)
        {
            Status = status.ToString();
            Message = message;
            Errors = error == null ? new List<string>() : error;
        }

        //[JsonConverter(typeof(StringEnumConverter))]
        public string Status { get; set; }

        public string Message { get; set; }

        //[JsonConverter(typeof(StringEnumConverter))]
        public string DataFormat { get; set; }

        public List<string> Errors { get; set; }


    }

    public class ApiErrorResponseDto : ApiResponseDto
    {

        public ApiErrorResponseDto(string message, List<string> error = null, ResponseStatus status = ResponseStatus.Failure)
            : base(message, status, error)
        {
        }

        public List<string> Data { get; set; }


    }


    public class ApiClassResponseDto<T> : ApiResponseDto where T : class
    {
        public ApiClassResponseDto(string message, T data, ResponseStatus status = ResponseStatus.Success)
        : base(message, status)
        {
            DataFormat = ResponseDataFormat.Object.ToString();
            Data = data;
        }
        public T Data { get; set; }
    }

    public class ApiSingleResponseDto : ApiResponseDto
    {
        public ApiSingleResponseDto(string message, ResponseStatus status = ResponseStatus.Success)
        : base(message, status)
        {
            DataFormat = ResponseDataFormat.Object.ToString();
        }
    }

    public class ApiListResponseDto<T> : ApiResponseDto where T : class
    {
        public ApiListResponseDto(string message, List<T> data, ResponseStatus status = ResponseStatus.Success)
        : base(message, status)
        {
            DataFormat = ResponseDataFormat.ObjectList.ToString();
            Data = data == null ? new List<T>() : data;
        }
        public List<T> Data { get; set; }
    }



    public class ApiScalarResponseDto : ApiResponseDto
    {
        public ApiScalarResponseDto(string message, string data, ResponseStatus status = ResponseStatus.Success)
        : base(message, status)
        {
            DataFormat = ResponseDataFormat.String.ToString();
            Data = data;
        }
        public string Data { get; set; }
    }
}
