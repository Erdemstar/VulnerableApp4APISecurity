using System;
using VulnerableApp4APISecurity.Core.Interfaces.DTO.Others.Response;

namespace VulnerableApp4APISecurity.Core.DTO.Others.Response.Base
{
	public class BaseResponse: IBaseResponse
	{
        public string? Message { get; set; }
    }
}

