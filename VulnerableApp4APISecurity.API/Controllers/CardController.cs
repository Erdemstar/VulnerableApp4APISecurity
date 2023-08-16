using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VulnerableApp4APISecurity.Core.DTO.Card;
using VulnerableApp4APISecurity.Core.DTO.Others.Response.Failed;
using VulnerableApp4APISecurity.Core.DTO.Others.Response.Success;
using VulnerableApp4APISecurity.Core.Entities.Card;
using VulnerableApp4APISecurity.Infrastructure.Repositories.Card;
using VulnerableApp4APISecurity.Infrastructure.Utility.JWT;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VulnerableApp4APISecurity.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CardController : Controller
    {
        private readonly CardRepository _cardRepository;
        private readonly JWTAuthManager _jwtAuthManager;

        public CardController(CardRepository cardRepository, JWTAuthManager jWTAuthManager)
        {
            _cardRepository = cardRepository;
            _jwtAuthManager = jWTAuthManager;
        }

        [HttpGet]
        public ActionResult<string> GetCard()
        {
            try
            {
                string? msg = null;
                msg.ToCharArray();
            }
            catch (Exception e)
            {
                return BadRequest(new FailedResponse { Message = e.ToString() });
            }

            return "";

        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> GetCardv2()
        {
            var UserId = _jwtAuthManager.TakeUserIdFromJWT(Request.Headers["Authorization"].ToString().Split(" ")[1]);
            var card = await _cardRepository.GetCard(UserId);
            if (card is null)
            {
                return Ok(new SuccessResponse { Message = "There is no card" });
            }
            return Ok(card);
        }


        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> CreateCard(CardCreateRequest request)
        {
            var UserId = _jwtAuthManager.TakeUserIdFromJWT(Request.Headers["Authorization"].ToString().Split(" ")[1]);
            CardEntity entity = new CardEntity
            {
                UserId = UserId,
                Nickname = request.Nickname,
                Number = request.Number,
                ExpireDate = request.ExpireDate,
                Cve = request.Cve,
                Password = request.Password
            };

            await _cardRepository.CreateAsync(entity);

            return Ok(new SuccessResponse { Message = "Card is created" });
        }


        [HttpDelete]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult> DeleteCard(DeleteCardRequest request)
        {
            if (request.CardId != null) {
                var result = await _cardRepository.DeleteCard(request.CardId);

                if (result)
                {
                    return Ok(new SuccessResponse { Message = "Card is created" });
                }

                return BadRequest(new FailedResponse { Message = "There is a problem while card delete" });
            }

            return BadRequest(new FailedResponse { Message = "CardId is empty. Please enter CardId and try again your request." });

        }
    }
}

